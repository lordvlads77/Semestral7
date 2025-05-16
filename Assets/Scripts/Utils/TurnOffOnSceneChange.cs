using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public sealed class TurnOffOnSceneChange : MonoBehaviour
    {

        [SerializeField] GameObject ObjectToTurnOff;

        private void Awake()
        {
            if(ObjectToTurnOff == null)
            {
                ObjectToTurnOff = GetComponent<GameObject>();
            }

            EDebug.Assert(ObjectToTurnOff != null, $"Need to attach a game object to the variable |{nameof(ObjectToTurnOff)}| make this script work", this);
            SceneManager.activeSceneChanged += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene _current, Scene _next)
        {
            ObjectToTurnOff.SetActive(false);
        }

    }

}

