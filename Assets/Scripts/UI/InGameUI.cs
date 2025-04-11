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
        private int selectedElement;
        private int currentMenuLevel;

        [Header("Other")]
        [SerializeField] private Input.Actions inputReciver;
        [SerializeField] private float distanceFromElement = -73.0f;

        private void Awake()
        {
            inputReciver = GameManager.Instance.GetComponent<Input.Actions>();
        }

        void Start()
        {
            selectedElement = 0;
            currentMenuLevel = 0;
            Debug.Assert(selector != null, "Necesitamos una imagen para apuntar a los elementos", this);
        }

        void Update()
        {
            if (currentMenuLevel != currentMenu.menuLevel)
            {
                ActivateValidMenus();
            }


            if (inputReciver.Movement.y > 0.1)
            {
                EDebug.Log("DOWN");
                selectDownUntilFoundActiveElement();
            }

            else if (inputReciver.Movement.y < -0.1)
            {
                EDebug.Log("UP");
                selectUpUntilFoundActiveElement();
            }

            if (inputReciver.Jump)
            {
                ExecuteButtonFunction();
            }


            for (int i = 0; i < currentMenu.elements.Length; ++i)
            {
                if (currentMenu.elements[i].button.isPointerOverButton)
                {
                    selectedElement = i;
                    break;
                }
            }

            AjustSelector();

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

        /// <summary>
        /// Se llama esta funcion cuando el button precionado no tiene funcion
        /// </summary>
        public void ButtonWithNoAssignedFunctionFunction()
        {
            EDebug.Log("<color=orange> Please assign a function to the unity event if you did not what it to be empty </color>", this);
        }


        private void OnStateChange(Utils.GameStates new_state)
        {
            currentGameState = new_state;
            ActivateValidMenus();
            selectedElement = 0;
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
                    _menu.elements[i].rectTransform.gameObject.SetActive(true);
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
            int PersistentEventCount = currentMenu.elements[selectedElement].button.button.onClick.GetPersistentEventCount();

            bool hasFunction = false;

            if (PersistentEventCount > 0)
            {
                string methodName = currentMenu.elements[selectedElement].button.button.onClick.GetPersistentMethodName(0);

                hasFunction = methodName != "";
                EDebug.Log($"hasFunction = {hasFunction}", this);
                EDebug.Log($"methodName = |{methodName}|", this);
            }

            if (hasFunction)
            {
                currentMenu.elements[selectedElement].button.button.onClick.Invoke();
            }
            else
            {
                ButtonWithNoAssignedFunctionFunction();
            }

        }

        public void IncreaseMenuLevel()
        {
            currentMenuLevel += 1;
        }

        public void DecreaseMenuLevel()
        {
            currentMenuLevel -= 1;
        }

        public void SetMenuLevel(int _new_menu_level)
        {
            currentMenuLevel = _new_menu_level;
        }

        private void ActivateValidMenus()
        {
            for (int i = 0; i < menus.Length; i++)
            {
                if (menus[i].associatedGameStates == currentGameState && menus[i].menuLevel == currentMenuLevel)
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

        public ButtonInfo button;
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
        /// 0 = el menu principal y solo esta activo cuando el menuLevel es 0
        /// 1 = el sub menu que solo eat activo cuando el menuLevel es 1
        /// 2 = el sub sub menu que solo eata activo cuando el menuLevel es 2 
        /// 3 = el sub sub sub menu que solo eata activo cuando el menuLevel es 3  etc.
        /// </summary>
        public int menuLevel;

    }

}
