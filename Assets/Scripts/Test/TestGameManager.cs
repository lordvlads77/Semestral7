using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace test
{
    using consts;
    /// <summary>
    /// This script only to test the game manager
    /// </summary>
    public sealed class TestGameManager : MonoBehaviour
    {
        [SerializeField] private GameStates gameStates = GameStates.IDLE;

        private void onGameStateChange(GameStates state)
        {
            gameStates = state;

        }

        // Start is called before the first frame update
        void Start()
        {
        }

        private void OnEnable()
        {
            GameManager.Instance.subscirbe(onGameStateChange);
            onGameStateChange(GameManager.Instance.gameState);
        }


        private void OnDisable()
        {
            GameManager.TryGetInstance()?.unsubscirbe(onGameStateChange);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("Pressed R");
                GameManager.Instance.setGameState(GameStates.IDLE);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Pressed E");
                GameManager.Instance.setGameState(GameStates.PAUSE);
            }

        }
    }
}
