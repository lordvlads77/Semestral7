using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils;



namespace UI
{
    public sealed class InGameUI : MonoBehaviour
    {
        [Tooltip("Todos los menus que controla el script")]
        [SerializeField] private Menu[] menus;

        [Header("Para controlar menus")]
        [SerializeField] private Menu currentMenu;
        [SerializeField] private Image selector;
        [SerializeField] Utils.GameStates currentGameState;
        private int selectedElement;
        [SerializeField] private Input.Actions inputReciver;

        private void Awake()
        {
            inputReciver = GameManager.Instance.GetComponent<Input.Actions>();
        }

        void Start()
        {
            GameManager.Instance.SetGameState(GameStates.Paused);
            selectedElement = 0;
            Debug.Assert(selector != null, "Necesitamos una imagen para apuntar a los elementos", this);
            assigneDefaultFunction();
        }

        void Update()
        {
            if (inputReciver.Movement.y > 0.1)
            {
                EDebug.Log("UP");
                selectUp();
            }

            else if (inputReciver.Movement.y < -0.1)
            {
                EDebug.Log("DOWN");
                selectDown();
            }
        }


        private void OnEnable()
        {
            /*Input.Actions.Instance.OnWeaponUpToggledEvent += selectUp;
            Input.Actions.Instance.OnWeaponDownToggledEvent += selectDown;
*/
            GameManager.Instance.Subscribe(OnStateChange);

            GameManager.Instance.SetGameState(GameStates.Paused);
            OnStateChange(GameManager.Instance.GameState);
        }

        private void OnDisable()
        {
            GameManager.TryGetInstance()?.Unsubscribe(OnStateChange);

            var localAction = Input.Actions.TryGetInstance();
            if (localAction != null)
            {
                /*
                Input.Actions.Instance.OnWeaponUpToggledEvent -= selectUp;
                Input.Actions.Instance.OnWeaponDownToggledEvent -= selectDown;*/
            }
        }

        private void DefaultAssignedFunction()
        {
            EDebug.Log("<color=orange> Please assign a function to the unity event if you did not what it to be empty </color>", this);
        }


        private void OnStateChange(Utils.GameStates new_state)
        {
            currentGameState = new_state;

            for (int i = 0; i < menus.Length; i++)
            {
                if (menus[i].associatedGameStates == currentGameState)
                {
                    this.ActivateMenuUiElements(ref menus[i]);
                }
                else
                {
                    this.DeactivateMenuUiElements(ref menus[i]);
                }
            }
        }

        private void ActivateMenuUiElements(ref Menu _menu)
        {
            for (int i = 0; i < _menu.elements.Length; i++)
            {
                _menu.elements[i].transform.gameObject.SetActive(true);
            }
        }

        private void DeactivateMenuUiElements(ref Menu _menu)
        {
            for (int i = 0; i < _menu.elements.Length; i++)
            {
                _menu.elements[i].transform.gameObject.SetActive(false);
            }

        }

        private void assigneDefaultFunction()
        {
            for (int i = 0; i < menus.Length; i++)
            {
                for (int j = 0; j < menus[i].elements.Length; j++)
                {
                    if (menus[i].elements[j].function.GetPersistentEventCount() < 1)
                    {
                        menus[i].elements[j].function.AddListener(DefaultAssignedFunction);
                    }
                }
            }

        }

        #region INPUT_EVENTS

        private void selectUp()
        {
            EDebug.Log("selectUp");
            selectedElement += 1;
        }

        private void selectDown()
        {
            EDebug.Log("selectDown");
            selectedElement -= 1;
        }

        #endregion

    }


    [Serializable]
    struct UIElement
    {
        public RectTransform transform;
        /// <summary>
        /// la funcion que se tomara al hacer click sobre el elemento de UI
        /// </summary>
        public UnityEvent function;
    }

    [Serializable]
    struct Menu
    {
        public UIElement[] elements;
        /// <summary>
        /// El menu solo es visible si es del mismo game State
        /// </summary>
        public GameStates associatedGameStates;
    }

}
