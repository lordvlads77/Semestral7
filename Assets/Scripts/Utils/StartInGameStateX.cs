using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{

    [DefaultExecutionOrder(-1)]
    public class StartInGameStateX : MonoBehaviour
    {
        public GameStates gameStateToSetOnStart = GameStates.Idle;
        public bool isOn = true;
        private Coroutine _coroutine = null;

        private void Start()
        {
            _coroutine??= StartCoroutine(WaitAndSetGameState());
        }

        private IEnumerator WaitAndSetGameState()
        {
            while (!GameManager.Instance)
            {
                yield return null;
            }
            if (isOn)
            {
                GameManager.Instance.SetGameState(gameStateToSetOnStart);
            }
            _coroutine = null;
        }

    }
}
