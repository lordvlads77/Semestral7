using UnityEngine;

namespace Utils
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        private static readonly object _lock = new object();
        private static bool _applicationIsQuitting = false;
        public static bool HasInstance => _applicationIsQuitting == false && _instance != null;
        public static T TryGetInstance() => HasInstance ? Instance : null;
        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                        "' already destroyed on application quit. Won't create again - returning null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError("[Singleton] Something went really wrong " + typeof(T));
                            return _instance;
                        }
                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();
                            DontDestroyOnLoad(singleton);
                            
                            Debug.Log("[Singleton] An instance of " + typeof(T) +
                                      " is needed in the scene, so '" + singleton +
                                      "' was created with DontDestroyOnLoad.");
                        }
                        else
                        {
                            Debug.Log("[Singleton] Using instance already created: " +
                                      _instance.gameObject.name);
                        }
                    }

                    return _instance;
                }
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
                OnAwake();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        protected virtual void OnAwake() { EDebug.Log(gameObject.name + " ► Initialized"); }

        private void OnDestroy()
        {
            _applicationIsQuitting = true;
        }

        private void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }
        
    }
}
