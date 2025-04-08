using System.Threading;
using UnityEngine;

namespace Utils
{
    [DefaultExecutionOrder(-50)]
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        private static readonly object _lock = new object();
        private static bool _applicationIsQuitting = false;
        private static bool _isCreating = false;
        
        public static bool HasInstance => _applicationIsQuitting == false && _instance != null;
        public static T TryGetInstance() => HasInstance ? Instance : null;

        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    EDebug.LogError("[Singleton] Instance '" + typeof(T) +
                        "' already destroyed on application quit. Won't create again - returning null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        if (_isCreating) // If creation is already in progress, wait until it's done.
                        {
                            while (_instance == null)
                            {
                                Monitor.Wait(_lock);
                            }
                            return _instance;
                        }

                        _isCreating = true;
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            EDebug.LogError("[Singleton] Something went really wrong " + typeof(T));
                            _isCreating = false;
                            Monitor.PulseAll(_lock);
                            return _instance;
                        }

                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();
                            DontDestroyOnLoad(singleton);
                            EDebug.Log("[Singleton] An instance of " + typeof(T) +
                                       " is needed in the scene, so '" + singleton +
                                       "' was created with DontDestroyOnLoad.");
                        }
                        else
                        {
                            EDebug.Log("[Singleton] Using instance already created: " +
                                       _instance.gameObject.name);
                        }

                        _isCreating = false;
                        Monitor.PulseAll(_lock);
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
        
        protected virtual void OnAwake()
        {
            EDebug.Log(gameObject.name + " ► Initialized");
        }

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
