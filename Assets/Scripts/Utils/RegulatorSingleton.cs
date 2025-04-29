using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Utils
{
    /// <summary>
    ///  if 2 regulator singletons are in the same Scene the youngest RegulatorSingleton will
    ///  Remove any older instances in the scene making it the only one
    /// </summary>
    /// <typeparam name="T"></typeparam>

    [DefaultExecutionOrder(-50)]
    public class RegulatorSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        private static readonly object _lock = new object();
        private static bool _applicationIsQuitting = false;
        private static bool _isCreating = false;
        public float creationTime { get; private set; }


        public static bool HasInstance => _applicationIsQuitting == false && _instance != null;
        public static T TryGetInstance() => HasInstance ? Instance : null;

        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    EDebug.LogError("[RegulatorSingleton] Instance '" + typeof(T) +
                        "' already destroyed on application quit. Won't create again - returning null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        if (_isCreating)
                        {
                            while (_instance == null)
                            {
                                Monitor.Wait(_lock);
                            }
                            return _instance;
                        }

                        _isCreating = true;
                        _instance = FindAnyObjectByType<T>();

                        T[] allInstances = FindObjectsByType<T>(FindObjectsSortMode.None);

                        if (allInstances.Length > 1)
                        {
                            RemoveDuplacates(allInstances, out _instance);

                            return _instance;
                        }

                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            _instance.GetComponent<RegulatorSingleton<T>>().creationTime = Time.time;
                            singleton.name = "(RegulatorSingleton) " + typeof(T).ToString();
                            DontDestroyOnLoad(singleton);
                            EDebug.Log("[RegulatorSingleton] An instance of " + typeof(T) +
                                       " is needed in the scene, so '" + singleton +
                                       "' was created with DontDestroyOnLoad.");
                        }
                        else
                        {
                            //EDebug.Log("[RegulatorSingleton] Using instance already created: " +
                            //          _instance.gameObject.name);
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
            else if (_instance != this)
            {
                T[] allInstances = FindObjectsByType<T>(FindObjectsSortMode.None);
                RemoveDuplacates(allInstances, out _instance);
                //EDebug.LogError($"[RegulatorSingleton] Duplicate instance of {typeof(T)} found. Destroying: {gameObject.name}");
                //Destroy(gameObject);
            }
            else OnAwake();
        }

        protected virtual void OnAwake()
        {
            EDebug.Log(gameObject.name + " ► Initialized");
        }

        private void OnDestroy()
        {
        }

        private void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_allInstances"></param>
        /// <param name="remainigInstance">The only version of the instance that will remain </param>
        private static void RemoveDuplacates(T[] _allInstances, out T remainigInstance)
        {
            float oldest_time = float.MinValue;
            int oldest_index = 0;

            for (int i = 0; i < _allInstances.Length; i++)
            {
                float creation_time = _allInstances[i].GetComponent<RegulatorSingleton<T>>().creationTime;
                if (oldest_time < creation_time)
                {
                    oldest_time = creation_time;
                    oldest_index = i;
                }
            }

            remainigInstance = _allInstances[oldest_index];

            for (int i = _allInstances.Length - 1; i >= 0; --i)
            {
                if (i != oldest_index)
                {
                    Destroy(_allInstances[i].gameObject);
                }
            }



        }


    }

}
