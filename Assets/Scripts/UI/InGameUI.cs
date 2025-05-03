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

        [Header("Input Related")]
        [SerializeField] MenuInputType currentInput;
        [SerializeField] MenuInputType blockedInput;
        [SerializeField] float verticalInputDelay = 0.15f;
        [SerializeField] float horizontalInputDelay = 0.15f;
        [SerializeField] float acceptedInputDelay = 0.15f;

        Coroutine _verticalInputCoroutine = null;
        Coroutine _horizontalInputCoroutine = null;
        Coroutine _acceptedInputCoroutine = null;

        [Header("Blocky Sliders")]
        [SerializeField] BlockySlider SFX;
        [SerializeField] BlockySlider Music;
        [SerializeField] BlockySlider Master;

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
            SaveSystem.SaveSystem.LoadVolumePrefs();
            LoadVolumeValues();
        }

        void Update()
        {
            ElementSelectionType selectionType = currentMenu.elements[selectedElement].elementSelectionType;


            if (currentMenuID != currentMenu.menuID)
            {
                ActivateMenusOfSameMenuID();
                selectedElement = 0;
                if (!currentMenu.elements[selectedElement].rectTransform.gameObject.activeInHierarchy)
                {
                    selectDownUntilFoundActiveElement();
                }


                AjustSelector();
                LoadVolumeValues();
            }


            ProcessInput();

            if (MenuInputTypeUtils.haveAnyMatchingBits(MenuInputType.VERTICAL_UP, currentInput))
            {
                selectUpUntilFoundActiveElement();
            }

            else if (MenuInputTypeUtils.haveAnyMatchingBits(MenuInputType.VERTICAL_DOWN, currentInput))
            {
                selectDownUntilFoundActiveElement();
            }

            if (MenuInputTypeUtils.haveAnyMatchingBits(MenuInputType.ACCEPTED, currentInput))
            {
                ExecuteFunctionForUIElement();
            }

            if (selectionType == ElementSelectionType.BLOCKY_SLIDER)
            {
                twoSidedLogic();
            }

            AjustSelector();

            currentInput = MenuInputType.NONE;

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

            if (SFX != null)
            { SFX.OnBlockChangeAction += OnSfxChange; }
            if (Music != null)
            { Music.OnBlockChangeAction += OnMusicChange; }
            if (Master != null)
            { Master.OnBlockChangeAction += OnMasterChange; }

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


            if (SFX != null)
            { SFX.OnBlockChangeAction -= OnSfxChange; }
            if (Music != null)
            { Music.OnBlockChangeAction -= OnMusicChange; }
            if (Master != null)
            { Master.OnBlockChangeAction -= OnMasterChange; }
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

        private void ExecuteFunctionForUIElement()
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
                if (menus[i].associatedGameStates != GameStates.Chatting && menus[i].associatedGameStates == currentGameState)
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


        private void twoSidedLogic()
        {
            if (MenuInputTypeUtils.haveAnyMatchingBits(MenuInputType.HORIZONTAL_RIGHT, currentInput))
            {
                BlockySlider slider = currentMenu.elements[selectedElement].blockySlider;
                if (slider == null)
                {
                    EDebug.LogError($"This UIElement does not have a {nameof(BlockySlider)} attach one please", this);
                    return;
                }
                slider.increaseBlocks();
            }
            else if (MenuInputTypeUtils.haveAnyMatchingBits(MenuInputType.HORIZONTAL_LEFT, currentInput))
            {
                BlockySlider slider = currentMenu.elements[selectedElement].blockySlider;
                if (slider == null)
                {
                    EDebug.LogError($"This UIElement does not have a {nameof(BlockySlider)} attach one please", this);
                    return;
                }
                slider.decreaseBlocks();
            }

        }

        private void ProcessInput()
        {
            bool isVerticalBlocked = MenuInputTypeUtils.haveAnyMatchingBits(MenuInputType.ANY_VERTICAL, blockedInput);
            bool isHorizontalBlocked = MenuInputTypeUtils.haveAnyMatchingBits(MenuInputType.ANY_HORIZONTAL, blockedInput);
            bool isAcceptedBlocked = MenuInputTypeUtils.haveAnyMatchingBits(MenuInputType.ACCEPTED, blockedInput);

            if (!isVerticalBlocked && inputReciver.Movement.y > this.menuDeadZone)
            {
                currentInput |= MenuInputType.VERTICAL_DOWN;
                _verticalInputCoroutine = StartCoroutine(blockVerticalInput());
            }
            else if (!isVerticalBlocked && inputReciver.Movement.y < -this.menuDeadZone)
            {
                currentInput |= MenuInputType.VERTICAL_UP;
                _verticalInputCoroutine = StartCoroutine(blockVerticalInput());
            }

            if (!isHorizontalBlocked && inputReciver.Movement.x > this.menuDeadZone)
            {
                currentInput |= MenuInputType.HORIZONTAL_RIGHT;
                _horizontalInputCoroutine = StartCoroutine(blockHorizontalInput());
            }
            else if (!isHorizontalBlocked && inputReciver.Movement.x < -this.menuDeadZone)
            {
                currentInput |= MenuInputType.HORIZONTAL_LEFT;
                _horizontalInputCoroutine = StartCoroutine(blockHorizontalInput());
            }

            if (!isAcceptedBlocked && inputReciver.Jump)
            {
                currentInput |= MenuInputType.ACCEPTED;
                _acceptedInputCoroutine = StartCoroutine(blockAcceptedInput());

            }

        }

        #region Coroutines

        IEnumerator blockVerticalInput()
        {
            MenuInputTypeUtils.setBit(MenuInputType.ANY_VERTICAL, ref blockedInput);
            yield return new WaitForSeconds(verticalInputDelay);
            MenuInputTypeUtils.unsetBit(MenuInputType.ANY_VERTICAL, ref blockedInput);
        }

        IEnumerator blockHorizontalInput()
        {
            MenuInputTypeUtils.setBit(MenuInputType.ANY_HORIZONTAL, ref blockedInput);
            yield return new WaitForSeconds(horizontalInputDelay);
            MenuInputTypeUtils.unsetBit(MenuInputType.ANY_HORIZONTAL, ref blockedInput);
        }

        IEnumerator blockAcceptedInput()
        {
            MenuInputTypeUtils.setBit(MenuInputType.ACCEPTED, ref blockedInput);
            yield return new WaitForSeconds(acceptedInputDelay);
            MenuInputTypeUtils.unsetBit(MenuInputType.ACCEPTED, ref blockedInput);
        }

        IEnumerator LoadVolumeLevelOnNextFrame()
        {
            yield return new WaitForEndOfFrame();

            if (SFX != null)
            { SFX.setPercent(SaveSystem.SaveSystem.GetVolume(SoundType.SFX)); }
            if (Music != null)
            { Music.setPercent(SaveSystem.SaveSystem.GetVolume(SoundType.Music)); }
            if (Master != null)
            { Master.setPercent(SaveSystem.SaveSystem.GetVolume(SoundType.Master)); }

        }

        #endregion

        #region OnVolumeChangeEvents

        private void OnSfxChange(float _percent)
        {
            SaveSystem.SaveSystem.SaveVolume(SoundType.SFX, _percent);
        }

        private void OnMusicChange(float _percent)
        {
            SaveSystem.SaveSystem.SaveVolume(SoundType.Music, _percent);
        }

        private void OnMasterChange(float _percent)
        {
            SaveSystem.SaveSystem.SaveVolume(SoundType.Master, _percent);
        }

        #endregion


        /// <summary>
        /// Carga los valores para el volumen
        /// </summary>
        private void LoadVolumeValues()
        {
            StartCoroutine(LoadVolumeLevelOnNextFrame());
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

        /// <summary>
        ///  OPCIONAL : es para los de tipo ElementSelectionType.BLOCKY_SLIDER
        /// </summary>
        public BlockySlider blockySlider;


        /// <summary>
        ///  OPCIONAL : es para los de tipo ElementSelectionType.TEXT_SWITCHER 
        /// </summary>
        public TextSwitcher textSwitcher;

        public ElementSelectionType elementSelectionType;
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

    [Serializable]
    public enum ElementSelectionType
    {
        REGULAR = 0, //  preciona la barrar de enter o espacio el button 'A' para hacer algo
        BLOCKY_SLIDER = 1, // precionar le para moverte de lada a lada en otras palabras 'D' y 'A'
        TEXT_SWITCHER = 2,
    }

}
