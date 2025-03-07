using UnityEngine;
using Utils;

namespace test
{
    /// <summary>
    /// This script only to test the game manager
    /// </summary>
    public sealed class TestGameManager : MonoBehaviour
    {
        [SerializeField] private GameStates gameStates = GameStates.Joining;

        private void OnGameStateChange(GameStates state)
        {
            gameStates = state;
        }

        private void OnEnable()
        {
            GameManager.Instance.Subscribe(OnGameStateChange);
            OnGameStateChange(GameManager.Instance.GameState);
        }

        private void OnDisable()
        {
            GameManager.TryGetInstance()?.Unsubscribe(OnGameStateChange);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                #if UNITY_EDITOR
                Debug.Log("Pressed R");
                #endif
                GameManager.Instance.SetGameState(GameStates.Playing);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                #if UNITY_EDITOR
                Debug.Log("Pressed E");
                #endif 
                GameManager.Instance.SetGameState(GameStates.Paused);
            }

        }
    }
}
