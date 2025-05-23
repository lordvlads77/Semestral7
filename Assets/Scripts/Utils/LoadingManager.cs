using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public sealed class LoadingManager : RegulatorSingleton<LoadingManager>
    {

        public int currentSceneIndex { get; private set; } = -1337;
        private bool loadByIndex = false;
        [Header("La ecena que vamos a cargar")]
        public string sceneNameToLoad = "";
        public int sceneIndexToLoad = -1;
        public float sceneProgress = 0.0f;
        public float fadeOutTime = 0.1f;


        private Coroutine progressCoroutine = null;
        private Coroutine fadeOutCoroutine = null;

        private NewLoadingScreen prefabCanvas;
        private NewLoadingScreen instantiatedCanvas;


        protected override void OnAwake()
        {
            base.OnAwake();
            prefabCanvas = Resources.Load<NewLoadingScreen>("Prefabs/Loading_Screens/_Exploding_knight_Loading_Scene");
        }


        public void LoadSceneByName(string scene_name, float _fadeOutTime = 1.0f)
        {
            loadByIndex = false;
            sceneNameToLoad = scene_name;
            fadeOutTime = _fadeOutTime;
            Prepare();
            loadLevel();
        }

        public void LoadSceneByIndex(int scene_index, float _fadeOutTime = 1.0f)
        {
            loadByIndex = true;
            sceneIndexToLoad = scene_index;
            fadeOutTime = _fadeOutTime;
            Prepare();
            loadLevel();
        }

        private async void loadLevel()
        {
            if (instantiatedCanvas != null)
            {
                if (fadeOutCoroutine != null)
                {
                    StopCoroutine(fadeOutCoroutine);
                }
                Destroy(instantiatedCanvas.gameObject);
                instantiatedCanvas = null;
            }

            instantiatedCanvas = Instantiate(prefabCanvas);
            DontDestroyOnLoad(instantiatedCanvas);
            AsyncOperation async;

            if (loadByIndex)
            {
                async = SceneManager.LoadSceneAsync(sceneIndexToLoad);
            }
            else
            {
                async = SceneManager.LoadSceneAsync(sceneNameToLoad);
            }

            progressCoroutine = StartCoroutine(captureLoadingValue(async));

        }

        private void Prepare()
        {
            currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            //loadStage = LOAD_STAGES.LoadLoadingScreen;
        }

        #region Coroutines

        private IEnumerator captureLoadingValue(AsyncOperation op)
        {
            while (!op.isDone)
            {
                sceneProgress = op.progress;
                instantiatedCanvas.PlayAnimationByPercentage(sceneProgress);
                yield return new WaitForEndOfFrame();
            }
            yield return null;
        }

        private IEnumerator playFadeOutAnimationThenDestroy(float time)
        {
            instantiatedCanvas.PlayFadeOut(time);
            while (instantiatedCanvas.isPlayingFadeAnimation)
            {
                yield return new WaitForEndOfFrame();
            }
            Destroy(instantiatedCanvas.gameObject);
            instantiatedCanvas = null;
        }
        #endregion

        #region DelegeteMethods
        private void OnActiveSceneChanged(Scene current, Scene next)
        {
            string currentName = current.name;

            //Mathf.PingPong()
            if (currentName == null)
            {
                // Scene1 has been removed
                currentName = "Replaced";
            }

            EDebug.Log("Scenes: " + currentName + ", " + next.name);
        }


        private void OnSceneLoaded(Scene newScene, LoadSceneMode loadSceneMode)
        {
            //StopCoroutine(progressCoroutine);
            StopAllCoroutines();
            if (instantiatedCanvas != null)
            {
                fadeOutCoroutine = StartCoroutine(playFadeOutAnimationThenDestroy(fadeOutTime));
            }


            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            //Destroy(instantiatedCanvas.gameObject);
        }

        private void OnSceneUnloaded(Scene newScene)
        {
            EDebug.Log($"<color=cyan> Scene UN-loaded = |{newScene.name}| </color>");
        }
        #endregion

    }
}
