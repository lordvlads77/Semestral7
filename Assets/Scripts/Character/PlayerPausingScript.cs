using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Character
{

    /// <summary>
    /// This script let's the player invoke the pause menu
    /// </summary>
    public sealed class PlayerPausingScript : MonoBehaviour
    {
        private Input.Actions inputReceiver;
        private GameStates currentGameState;

        #region GameManagerBoilerPlate
        private void OnEnable()
        {
            if (inputReceiver == null)
            {
                inputReceiver = GameManager.Instance.GetComponent<Input.Actions>();
            }

            GameManager.Instance.RegisterUnsubscribeAction(UnSubscribe);
            GameManager.Instance.Subscribe(OnGameStateChange);
            inputReceiver.OnPauseEvent += OnPause;
        }

        private void OnDisable()
        {
            inputReceiver.OnPauseEvent -= OnPause;
        }

        #endregion

        #region EventMethods

        private void OnPause(bool _)
        {
            if (currentGameState == GameStates.Playing)
            {
                GameManager.Instance.SetGameState(GameStates.Paused);
            }
            else if (currentGameState == GameStates.Paused)
            {
                GameManager.Instance.SetGameState(GameStates.Playing);
            }
        }

        private void OnGameStateChange(GameStates _states)
        {
            this.currentGameState = _states;
        }

        private void UnSubscribe()
        {
            GameManager.Instance.Unsubscribe(OnGameStateChange);
        }

        #endregion


    }

}
