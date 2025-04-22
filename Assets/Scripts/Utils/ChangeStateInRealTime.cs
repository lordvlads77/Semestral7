using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Cambiar el GameStates en tiempo real
    /// </summary>
    public class ChangeStateInRealTime : MonoBehaviour
    {

        [Tooltip("The state to change into")]
        [SerializeField] GameStates desiredGameState;
        [Tooltip("The Current Game State")]
        [SerializeField] GameStates currentGameState;

        [Tooltip("Set to true to change states")]
        [SerializeField] bool shouldChangeState = true;

        void Update()
        {
            if (shouldChangeState && desiredGameState != currentGameState)
            {
                GameManager.Instance.SetGameState(desiredGameState);
            }
        }

        private void OnEnable()
        {
            GameManager.Instance.Subscribe(OnStateChange);
            OnStateChange(GameManager.Instance.GameState);
        }


        private void OnDisable()
        {
            GameManager.TryGetInstance()?.Unsubscribe(OnStateChange);
        }

        private void OnStateChange(GameStates newState)
        {
            currentGameState = newState;
        }

    }

}
