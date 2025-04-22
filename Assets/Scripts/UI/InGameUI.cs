using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] float menuDeadZone = 0.1f;
        private int selectedElement;
        [SerializeField] private int currentMenuID;
        private int previousMenuID;

        [Header("Other")]
        [SerializeField] private Input.Actions inputReciver;
        [SerializeField] private float distanceFromElement = -73.0f;
        private bool hasInitMenusIDs = false;

        void InitMenusIDs()
        {
            const int FIRST_ID = 2_000_000;
            for (int i = 0; i < menus.Length; i++)
            {
                /// override the default value
                if (menus[i].menuID == 0)
                {
                    menus[i].menuID = FIRST_ID - i;
                }

            }

        }

        void Start()
        {
            selectedElement = 0;
            Debug.Assert(selector != null, "Necesitamos una imagen para apuntar a los elementos", this);
        }

        void Update()
        {
            if (currentMenuID != currentMenu.menuID)
            {
                ActivateMenusOfSameMenuID();
                selectedElement = 0;
                if (!currentMenu.elements[selectedElement].rectTransform.gameObject.activeInHierarchy)
                {
                    selectDownUntilFoundActiveElement();
                }


                AjustSelector();
            }


            if (inputReciver.Movement.y > menuDeadZone)
            {
                EDebug.Log("DOWN");
                selectDownUntilFoundActiveElement();
            }

            else if (inputReciver.Movement.y < -menuDeadZone)
            {
                EDebug.Log("UP");
                selectUpUntilFoundActiveElement();
            }

            if (inputReciver.Jump)
            {
                ExecuteButtonFunction();
            }


            AjustSelector();

        }


        private void OnEnable()
        {
            /*Input.Actions.Instance.OnWeaponUpToggledEvent += selectUp;
            Input.Actions.Instance.OnWeaponDownToggledEvent += selectDown;*/

            if (inputReciver == null)
            {
                inputReciver = GameManager.Instance.GetComponent<Input.Actions>();
            }
            if (!hasInitMenusIDs)
            {
                InitMenusIDs();
                hasInitMenusIDs = true;
            }

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

        /// <summary>
        /// Se llama esta funcion cuando el button precionado no tiene funcion
        /// </summary>
        public void ButtonWithNoAssignedFunctionFunction()
        {
            EDebug.Log("<color=orange> Please assign a function to the unity event if you did not what it to be empty </color>", this);
        }


        private void OnStateChange(Utils.GameStates new_state)
        {
            selectedElement = 0;
            currentGameState = new_state;
            ActivateMenusOfSameGameState();
            selectedElement = 0;
            AjustSelector();
        }

        private void ActivateMenuUiElements(ref Menu _menu)
        {
            if (_menu.background != null)
            {
                _menu.background.gameObject.SetActive(true);
            }


            for (int i = 0; i < _menu.elements.Length; i++)
            {
                if (_menu.elements[i].rectTransform != null)
                {
                    bool shouldTurnOn = !_menu.elements[i].turnOff;
                    _menu.elements[i].rectTransform.gameObject.SetActive(shouldTurnOn);
                }
            }
            currentMenu = _menu;
        }

        private void DeactivateMenuUiElements(ref Menu _menu)
        {
            if (_menu.background != null)
            {
                _menu.background.gameObject.SetActive(false);
            }

            for (int i = 0; i < _menu.elements.Length; i++)
            {
                if (_menu.elements[i].rectTransform != null)
                {
                    _menu.elements[i].rectTransform.gameObject.SetActive(false);
                }
            }

        }

        #region INPUT_EVENTS

        private void selectUpUntilFoundActiveElement()
        {
            int safety_var = 10_000;
            selectUp();
            while (safety_var > 1 &&
                !currentMenu.elements[selectedElement].rectTransform.gameObject.activeInHierarchy
                )
            {
                selectUp();
                safety_var -= 1;
            }

        }

        private void selectUp()
        {
            EDebug.Log("selectUp");
            selectedElement += 1;
            if (selectedElement > currentMenu.elements.Length - 1)
            {
                selectedElement = 0;
            }
        }


        private void selectDown()
        {
            EDebug.Log("selectDown");
            selectedElement -= 1;
            if (selectedElement < 0)
            {
                selectedElement = currentMenu.elements.Length - 1;
            }
        }

        private void selectDownUntilFoundActiveElement()
        {
            int safety_var = 10_000;
            selectDown();
            while (safety_var > 1 &&
                !currentMenu.elements[selectedElement].rectTransform.gameObject.activeInHierarchy
                )
            {
                selectDown();
                safety_var -= 1;
                if (safety_var < 1)
                {
                    break;
                }
            }

        }

        #endregion

        private void AjustSelector()
        {
            selector.transform.SetParent(currentMenu.elements[selectedElement].rectTransform);
            selector.rectTransform.anchoredPosition = new Vector2(distanceFromElement, 0);
        }

        private void ExecuteButtonFunction()
        {
            int PersistentEventCount = currentMenu.elements[selectedElement].eventForUi.GetPersistentEventCount();

            bool hasFunction = false;

            if (PersistentEventCount > 0)
            {
                string methodName = currentMenu.elements[selectedElement].eventForUi.GetPersistentMethodName(0);

                hasFunction = methodName != "";
                EDebug.Log($"<color=green>methodName = |{methodName}|</color>", this);
            }

            if (hasFunction)
            {
                currentMenu.elements[selectedElement].eventForUi.Invoke();
            }
            else
            {
                ButtonWithNoAssignedFunctionFunction();
            }

        }

        public void SetMenuID(int _new_menu_ID)
        {
            previousMenuID = currentMenuID;
            currentMenuID = _new_menu_ID;
        }

        public void SetPreviousMenuID()
        {
            currentMenuID = previousMenuID;
        }

        private void ActivateMenusOfSameGameState()
        {
            previousMenuID = currentMenuID;
            for (int i = 0; i < menus.Length; i++)
            {
                if (menus[i].associatedGameStates == currentGameState)
                {
                    this.ActivateMenuUiElements(ref menus[i]);
                    currentMenuID = menus[i].menuID;
                }
                else
                {
                    this.DeactivateMenuUiElements(ref menus[i]);
                }
            }
        }

        private void ActivateMenusOfSameMenuID()
        {
            for (int i = 0; i < menus.Length; i++)
            {
                if (menus[i].menuID == currentMenuID)
                {
                    this.ActivateMenuUiElements(ref menus[i]);
                }
                else
                {
                    this.DeactivateMenuUiElements(ref menus[i]);
                }

            }

        }


    }


    [Serializable]
    public struct UIElement
    {
        public RectTransform rectTransform;

        public UnityEvent eventForUi;

        /// <summary>
        /// Controla si un UIElement deberia esta apagado o no
        /// </summary>
        public bool turnOff;
    }

    [Serializable]
    struct Menu
    {
        public UIElement[] elements;
        /// <summary>
        /// OPCIONAL : El fondo del menu en question
        /// </summary
        public Image background;
        /// <summary>
        /// El menu solo es visible si es del mismo game State
        /// </summary>
        public GameStates associatedGameStates;

        /// <summary>
        /// el numero indentificador del Menu
        /// </summary>
        public int menuID;

    }

}
