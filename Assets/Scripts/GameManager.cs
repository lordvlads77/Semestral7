using System;
using System.Collections.Generic;
using Entity;
using Scriptables;
using UnityEngine;
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
    
    [Header("Other Settings")]
    [SerializeField, Range(0.1f, 5f)] private float npcRange = 1.5f;
    
    private List<LivingEntity> _nearbyNpc = new List<LivingEntity>();
    public GameObject player;
    
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
        EDebug.Log("GameManager Awake");
        SetGameState(GameStates.Joining);
       // CheckForMissingScripts();
        InvokeRepeating(nameof(LazyUpdate), 1f, 1f);
    }
    
    private void LazyUpdate() // This updates only once per second
    {
        if (player != null && NpcCloseBy(player.transform.position))
            Dialog.DisplayNpcPrompt(_nearbyNpc);
        else Dialog.RemoveNpcPrompt(); // Show or hide chat prompt based on proximity
    }
    
    public bool NpcCloseBy(Vector3 pos)
    {
        Collider[] hitColliders = Physics.OverlapSphere(pos, npcRange);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("NPC"))
            {
                LivingEntity npc = hitCollider.GetComponent<LivingEntity>();
                if (npc != null && !npc.isDead) _nearbyNpc.Add(npc);
            }
        }
        return _nearbyNpc.Count > 0;
    }

    private void CheckForMissingScripts() // Add more as needed :o
    {
        if (weaponStats == null) EDebug.LogError("WeaponStats is null.");
        if (randomNames == null) EDebug.LogError("RandomNames is null.");
        if (canvasPrefabs == null) EDebug.LogError("CanvasPrefabs can NOT be null! \n Make sure to add it before playing!!");
        if (Dialog == null) Dialog = gameObject.AddComponent<Dialog>();
        if (Actions == null) Actions = gameObject.AddComponent<Input.Actions>();
    }

    public void Subscribe(Action<GameStates> function)
    {
        _eventHandler += function;
    }

    public void Unsubscribe(Action<GameStates> function)
    {
        _eventHandler -= function;
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
    
    
}
