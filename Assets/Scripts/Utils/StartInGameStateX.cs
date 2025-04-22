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

        void Start()
        {
            if (isOn)
            {
                GameManager.Instance.SetGameState(gameStateToSetOnStart);
            }
        }

    }
}
