using System;
using System.Collections;
using System.Collections.Generic;
using Entity;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Scriptables;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

[DefaultExecutionOrder(-25)]
public sealed class GameManager : Singleton<GameManager>
{
    private Action<GameStates> _eventHandler;
    public GameStates GameState { get; private set; }
    
    [Header("Shared Scriptables")]
    public WeaponStats weaponStats;
    public RandomNames randomNames;
    public CanvasPrefabs canvasPrefabs;
    public Canvas NpcCanvas { get; private set; }
    public GameObject NpcPrompt { get; private set; }
    public GameObject EnemySpawnHolder { get; private set; }
    
    [Header("Other Settings")]
    [SerializeField, Range(0.1f, 60f), Tooltip("Set in minutes")] private float saveDataWarningTime = 5f;
    private Coroutine _saveDataWarningCoroutine;
    [SerializeField, Range(0.1f, 5f)] private float npcRange = 1.5f;
    public Language CurrentLanguage { get; private set; } = Language.En;
    
    private List<LivingEntity> _nearbyNpc = new List<LivingEntity>();
    public bool SavedData { get; private set; }
    public bool LoadedData { get; private set; }
    public GameObject player;
    public SoundManager SoundManager { get; private set; }
    
    private readonly List<Action> _globalUnsubscribeActions = new List<Action>();
    private WindowMode _windowMode; 
    private WindowResolution _windowRes;
    
    public void RegisterUnsubscribeAction(Action unsubscribeAction)
    {
        if (unsubscribeAction != null && !_globalUnsubscribeActions.Contains(unsubscribeAction))
            _globalUnsubscribeActions.Add(unsubscribeAction);
    }
    
    private Dialog Dialog {
        get => Dialog.Instance;
        set => throw new NotImplementedException();
    }
    private Input.Actions Actions {
        get => Input.Actions.Instance;
        set => throw new NotImplementedException();
    }

    protected override void OnAwake()
    {
        _windowMode = (WindowMode)Math.Min(Enum.GetValues(typeof(WindowMode)).Length, PlayerPrefs.GetInt("WindowMode", 0));
        _windowRes = (WindowResolution)PlayerPrefs.GetInt("WindowResolution", 0);
        SetGameWindowAndResolution();
        LoadFModBank();
        SetGameState(GameStates.Joining);
        Localization.LoadLanguage(CurrentLanguage);
        CheckForMissingScripts();
        if (EnemySpawnHolder == null) GetOrCreateEnemySpawnHolder();
        InvokeRepeating(nameof(LazyUpdate), 1f, 1f);
        EDebug.Log("GameManager Awake");
    }

    private WindowResolution ValidatedWindowRes(WindowResolution targetRes)
    {
        Resolution[] supportedResolutions = Screen.resolutions;
        int targetWidth = GetWidthFromResolution(targetRes);
        int targetHeight = GetHeightFromResolution(targetRes);
        Resolution closestResolution = supportedResolutions[0];
        int closestDifference = int.MaxValue;
        foreach (Resolution res in supportedResolutions) {
            int widthDiff = Math.Abs(res.width - targetWidth);
            int heightDiff = Math.Abs(res.height - targetHeight);
            int totalDiff = widthDiff + heightDiff;
            if (res.width == targetWidth && res.height == targetHeight)
                return targetRes;
            if (totalDiff < closestDifference || 
                (totalDiff == closestDifference && res.width > closestResolution.width)) {
                closestResolution = res;
                closestDifference = totalDiff;
            }
        }
        foreach (WindowResolution resEnum in Enum.GetValues(typeof(WindowResolution))) {
            if (GetWidthFromResolution(resEnum) == closestResolution.width &&
                GetHeightFromResolution(resEnum) == closestResolution.height)
                return resEnum;
        }
        return WindowResolution.R1920X1080;
    }

    public void SetGameWindowType(WindowMode windowMode)
    {
        switch (windowMode) {
            default:
            case WindowMode.Fullscreen:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case WindowMode.Windowed:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case WindowMode.Borderless:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case WindowMode.Maximized:
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                break;
        }
    }

    public void SetGameWindowAndResolution()
    {
        var validatedRes = ValidatedWindowRes(_windowRes);
        Screen.SetResolution(
            GetWidthFromResolution(validatedRes), 
            GetHeightFromResolution(validatedRes), 
            _windowMode == WindowMode.Fullscreen || _windowMode == WindowMode.Borderless
        );
        SetGameWindowType(_windowMode);
        EDebug.Log($"Window mode set to {_windowMode} with resolution {validatedRes}");
    }

    private int GetWidthFromResolution(WindowResolution resolution) {
        return int.Parse(resolution.ToString().Substring(1).Split('X')[0]);
    }
    private int GetHeightFromResolution(WindowResolution resolution) {
        return int.Parse(resolution.ToString().Substring(1).Split('X')[1]);
    }
    
    protected override void OnApplicationQuit()
    {
        applicationIsQuitting = true;
        foreach (var unsubscribeAction in _globalUnsubscribeActions)
        {
            try {
                EDebug.Log($"Unsubscribing from action: {unsubscribeAction.Method.Name} in script: {unsubscribeAction.Target.GetType().Name}");
                unsubscribeAction?.Invoke();
            }
            catch (Exception ex) {
                EDebug.LogError($"Error while unsubscribing action: {unsubscribeAction.Method.Name}. Details: {ex.Message}");
            }
        }
        _globalUnsubscribeActions.Clear();
        base.OnApplicationQuit();
        EDebug.Log("GameManager is quitting.");
    }

    private void LoadFModBank()
    {
        EDebug.Log("Loading FMOD Bank...");
        try {
            RuntimeManager.LoadBank("Master", true);
            EDebug.Log("FMOD Bank loaded successfully.");
        }
        catch (Exception ex) { EDebug.LogError($"Error loading FMOD Bank: {ex.Message}"); }
    }

    public GameObject GetOrCreateEnemySpawnHolder()
    {
        if (EnemySpawnHolder != null) return EnemySpawnHolder;
        EnemySpawnHolder = Instantiate(new GameObject());
        EnemySpawnHolder.name = "EnemySpawnHolder";
        EnemySpawnHolder.transform.position = new Vector3(0, 0, 0);
        return EnemySpawnHolder;
    }
    
    public void SetLanguage(Language language)
    {
        if (CurrentLanguage == language) return;
        CurrentLanguage = language;
        Localization.LoadLanguage(language);
        EDebug.Log($"Language changed to: {language}");
    }
    
    private void LazyUpdate() // This updates only once per second
    {
        if (player != null && NpcCloseBy(player.transform.position))
            Dialog.DisplayNpcPrompt(_nearbyNpc);
        else Dialog.RemoveNpcPrompt(); // Show or hide chat prompt based on proximity
    }
    
    public bool NpcCloseBy(Vector3 pos)
    {
        _nearbyNpc.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(pos, npcRange);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("NPC")) {
                LivingEntity npc = hitCollider.GetComponent<LivingEntity>();
                if (npc != null && !npc.isDead) _nearbyNpc.Add(npc);
            }
        }
        return _nearbyNpc.Count > 0;
    }

    private void CheckForMissingScripts() // Add more as needed :o
    {
        if (!Application.isPlaying || applicationIsQuitting) return;
        if (weaponStats == null) EDebug.LogError("WeaponStats is null.");
        if (randomNames == null) EDebug.LogError("RandomNames is null.");
        if (canvasPrefabs == null) EDebug.LogError("CanvasPrefabs can NOT be null! \n Make sure to add it before playing!!");
        if (Dialog == null) Dialog = gameObject.AddComponent<Dialog>();
        if (Actions == null) Actions = gameObject.AddComponent<Input.Actions>();
        if (SoundManager != null) return;
        SoundManager = GetComponent<SoundManager>();
        if (SoundManager == null) SoundManager = gameObject.AddComponent<SoundManager>();
    }

    public void Subscribe(Action<GameStates> function)
    {
        _eventHandler += function;
        SaveSystem.SaveSystem.OnSaveData += OnDataSaved;
        SaveSystem.SaveSystem.OnLoadData += OnDataLoaded;
    }

    public void Unsubscribe(Action<GameStates> function)
    {
        _eventHandler -= function;
        SaveSystem.SaveSystem.OnSaveData -= OnDataSaved;
        SaveSystem.SaveSystem.OnLoadData -= OnDataLoaded;
    }

    public void OnDataSaved()
    {
        SavedData = true;
        if (_saveDataWarningCoroutine != null)
            StopCoroutine(_saveDataWarningCoroutine);
        _saveDataWarningCoroutine = StartCoroutine(SaveDataWarn());
        EDebug.Log("Saved Data");
    }
    
    public void OnDataLoaded()
    {
        LoadedData = true;
        EDebug.Log("Loaded Data");
    }
    
    private IEnumerator SaveDataWarn()
    {
        yield return new WaitForSeconds(saveDataWarningTime*60f);
        SavedData = false;
    }
    
    public Canvas GetOrCreateNpcCanvas()
    {
        if (NpcCanvas == null) 
            NpcCanvas = Instantiate(canvasPrefabs.npcCanvas);
        return NpcCanvas;
    }
    
    public GameObject GetOrCreateNpcPromptCanvas()
    {
        if (NpcPrompt == null) 
            NpcPrompt = Instantiate(canvasPrefabs.dialogPrompt);
        return NpcPrompt;
    }

    public void SetGameState(GameStates state) // Left public for use in a Canvas or what not
    {
        if (GameState == state) return;
        GameState = state;
        _eventHandler.Invoke(GameState);
    }

    public Sprite[] GetRandomSprites()
    {
        var sprite = new List<Sprite>();
        foreach (RandomSprite randSprite in canvasPrefabs.canvasSprites)
        {
            sprite.Add(randSprite.GetRandomSprite());
        }
        return sprite.ToArray();
    }
    
    [ContextMenu("Print current language")] public void PrintDebugLanguage()
    {
        EDebug.Log(Localization.Translate("log.debug_lang"));
    }
    
    [ContextMenu("Set to Play")] public void SetGamestateToPlay()
    {
        SetGameState(GameStates.Playing);
        EDebug.Log("Done! You're playing now");
    }
    
}
