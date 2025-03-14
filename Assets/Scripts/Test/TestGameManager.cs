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

        public void Start()
        {
            GameManager.Instance.SetGameState(stateToChangeTo);
        }

        private void OnEnable()
        {
            GameManager.Instance.Subscribe(OnGameStateChange);
            //OnGameStateChange(GameManager.Instance.GameState);
        }

        private void OnDisable()
        {
            GameManager.TryGetInstance()?.Unsubscribe(OnGameStateChange);
        }

        public void ChangeState(GameStates state)
        {
            GameManager.Instance.SetGameState(state);
        }

        private void OnGameStateChange(GameStates state)
        {
            gameStates = state;
        }

        [Header("Context Menu Item change game state")]
        [ContextMenuItem("Change game state", nameof(EditorStateChange))]
        [SerializeField] public GameStates stateToChangeTo = GameStates.Idle;

        public void EditorStateChange()
        {
            gameStates = stateToChangeTo;
        }

        /*void Update()
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

        }*/
    }
}
