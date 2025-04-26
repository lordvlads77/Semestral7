using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

/// <summary>
/// TODO : IMPLEMENTAR ESTO EN OTRO LUGARES 
/// </summary>
public sealed class Loading : MonoBehaviour
{
    enum LOAD_PART : byte
    {
        NONE = 0,
        LOADING_SCENE,
        UNLOAD_SCENE,
        LOADING_NEXT,
        FINISHED,
    }

    private GameStates gameStates;
    [Header("Scene to load into")]
    public string sceneToLoad = "";
    public int sceneBuildIndex = -1337;

    /// <summary>
    /// DO NOT MOVE THE SCENE LOCATED IN THIS STRING PLEASE
    /// </summary>
    private const string testingLoadingSceneName = "Scenes/Yhaliff/Test_Loading_Scene";

    LOAD_PART nextLoad = LOAD_PART.NONE;

    [Header("visuals for loading scene")]
    public Animation loadScreen;


    [field: Header("event")]
    public event Action OnStartChangeScene;

    private int currentSceneIndex = 0;

    private void Awake()
    {
        nextLoad = LOAD_PART.NONE;
    }

    public void ChangeToLoad()
    {
        DontDestroyOnLoad(gameObject);
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        loadScreen.gameObject.SetActive(true);
        loadScreen.Play("Loading"); // loading Screen Into
    }

    public void OnLoadingScreenSet()// Termino de cargar la ecena 
    {
        nextLoad = LOAD_PART.LOADING_SCENE;
        OnStartChangeScene?.Invoke();

        SceneManager.LoadSceneAsync(testingLoadingSceneName, LoadSceneMode.Additive);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode _loadSceneMode) // llamada por el scene manager
    {
        subscribeAndCallGameManager();
        StartCoroutine(WaitOnLevelLoaded());
    }

    IEnumerator WaitOnLevelLoaded()
    {
        switch (nextLoad)
        {
            case LOAD_PART.LOADING_SCENE:
                nextLoad = LOAD_PART.UNLOAD_SCENE;
                while (SceneManager.sceneCount < 2)
                {
                    yield return new WaitForEndOfFrame();
                }

                SceneManager.UnloadSceneAsync(currentSceneIndex);
                break;

            case LOAD_PART.LOADING_NEXT:
                nextLoad = LOAD_PART.NONE;
                while (SceneManager.sceneCount < 2)
                {
                    yield return new WaitForEndOfFrame();
                }

                loadScreen.Play("Loading_Screen_Out"); //
                SceneManager.UnloadSceneAsync(testingLoadingSceneName);
                break;
            default:
                Debug.LogError("Unhanded case while loading level", this);
                break;

        }

    }

    private void OnSceneUnloaded(Scene scene)
    {
        GameManager.TryGetInstance()?.Unsubscribe(OnGameStateChange);
        nextLoad = LOAD_PART.LOADING_NEXT;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        if (sceneBuildIndex >= 0)
        {
            SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive);
        }
        else
        {
            SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        }
    }

    public void OnLoadingScreenOff()
    {
        Destroy(gameObject);
    }

    #region GameManagerBoilerPlate

    private void OnEnable()
    {
        subscribeAndCallGameManager();
    }

    private void OnDisable()
    {
        GameManager.TryGetInstance()?.Unsubscribe(OnGameStateChange);
    }

    private void OnGameStateChange(GameStates _gameStates)
    {
        gameStates = _gameStates;
    }

    private void subscribeAndCallGameManager()
    {
        GameManager.Instance.Subscribe(OnGameStateChange);
        OnGameStateChange(GameManager.Instance.GameState);
    }

    #endregion

}
