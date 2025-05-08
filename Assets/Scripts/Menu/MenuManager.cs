using System.Collections;
using System.Collections.Generic;
using Input;
using UI;
using UnityEngine;
using Utils;

public sealed class MenuManager : MonoBehaviour
{

    //Animator animator;
    [SerializeField] RectTransform selector;
    [SerializeField] Transform[] menuItems;
    [SerializeField] Transform[] optionsItems;
    [SerializeField] Transform[] saveFileItems;
    Transform[] currentArrayInUse;
    [SerializeField] int currentSelection = 0;
    [SerializeField] CURRENT_MENU_STATE currentState = CURRENT_MENU_STATE.INTRO;
    CURRENT_MENU_STATE desiredState = CURRENT_MENU_STATE.INTRO;
    [SerializeField] GameObject menuInicio;
    [SerializeField] GameObject menuOptions;
    [SerializeField] GameObject SaveFileOptions;
    Input.Actions cosa;

    [Header("BlockySlider")]
    [SerializeField] private BlockySlider SFX;
    [SerializeField] private BlockySlider Music;
    [SerializeField] private BlockySlider Master;

    [Header("TextSwitcher")]
    [SerializeField] private UI.TextSwitcher textSwitcher;
    [SerializeField] private UI.TextSwitcher resolutionSwitcher;

    [Header("Confirmation Menu")]
    [SerializeField] private UI.ConformationMenu conformationMenu;

    private Coroutine _verticalInputBlock;
    private Coroutine _horizontalInputBlock;
    private Coroutine _acceptedInputBlock;

    [Header("Input related")]
    [Tooltip("Controls how long to wait until the next 'up' or 'down' is accepted")]
    [SerializeField] float verticalInputDelay = 1.0f / 30.0f;

    [Tooltip("Controls how long to wait until the next 'left' or 'right' is accepted")]
    [SerializeField] float horizontalInputDelay = 1.0f / 30.0f;

    [Tooltip("Controls how long to wait until the next 'accept' is accepted")]
    [SerializeField] float acceptedInputDelay = 1.0f / 15.0f;

    MenuInputType currentInputInUse = MenuInputType.NONE;
    MenuInputType blockedInput = MenuInputType.NONE;

    private bool _downClicked = false;
    private bool _upClicked = false;

    //  bool isVerticalInputBlocked = false;
    //  bool isHorizontalInputBlocked = false;
    bool isAcceptedInputBlocked = false;
    bool hasVolumeValuesBeenLoaded = false;

    enum CURRENT_MENU_STATE
    {
        INTRO,
        MAIN_MENU,
        OPTIONS,
        SAVE_FILE_SELECT,
    }

    private void Awake()
    {
        cosa = GameManager.Instance.GetComponent<Actions>(); // Actions.Instance;
    }

    void Start()
    {
        currentArrayInUse = saveFileItems;
        currentState = CURRENT_MENU_STATE.MAIN_MENU;
        desiredState = CURRENT_MENU_STATE.SAVE_FILE_SELECT;
        //selector.gameObject.SetActive(false);
        currentSelection = 0;
        ChangeSelectorPosition();
        EDebug.Assert(textSwitcher != null, $"El script necesita un {nameof(TextSwitcher)}", this);
        EDebug.Assert(conformationMenu != null, $"El script necesita un {nameof(UI.ConformationMenu)}", this);
    }

    private void Update()
    {
        currentInputInUse = MenuInputType.NONE;
        UpdateStates();

        switch (currentState)
        {
            case CURRENT_MENU_STATE.MAIN_MENU:
                ProcessMainMenu();
                break;

            case CURRENT_MENU_STATE.OPTIONS:
                ProcessOptions();
                break;
            case CURRENT_MENU_STATE.SAVE_FILE_SELECT:
                ProcessSaveFileSelect();
                break;
        }

    }

    private void UpdateStates()
    {
        if (currentState == desiredState)
        { return; }

        currentState = desiredState;

        switch (currentState)
        {
            case CURRENT_MENU_STATE.INTRO:
                menuOptions.SetActive(false);
                menuInicio.SetActive(true);
                SaveFileOptions.SetActive(false);
                break;

            case CURRENT_MENU_STATE.OPTIONS:
                currentSelection = 0;
                currentArrayInUse = optionsItems;
                menuOptions.SetActive(true);
                menuInicio.SetActive(false);
                SaveFileOptions.SetActive(false);

                // si no existe un elemento zero buscar hasta encontar uno 
                ChangeCurrentSelectionUntilObjectIsFound();
                ChangeCurrentSelectionUntilObjectIsFound(false);

                ChangeSelectorPosition();

                break;

            case CURRENT_MENU_STATE.MAIN_MENU:

                currentSelection = 0;
                currentArrayInUse = menuItems;
                menuOptions.SetActive(false);
                menuInicio.SetActive(true);
                SaveFileOptions.SetActive(false);
                // si no existe un elemento zero buscar hasta encontar uno 
                ChangeCurrentSelectionUntilObjectIsFound();
                ChangeCurrentSelectionUntilObjectIsFound(false);

                ChangeSelectorPosition();
                hasVolumeValuesBeenLoaded = false;
                break;

            case CURRENT_MENU_STATE.SAVE_FILE_SELECT:
                currentSelection = 0;
                currentArrayInUse = saveFileItems;

                menuOptions.SetActive(false);
                menuInicio.SetActive(false);
                SaveFileOptions.SetActive(true);

                ChangeSelectorPosition();
                hasVolumeValuesBeenLoaded = false;
                break;

            default:
                EDebug.LogError("Falta evaluar condicion", this);
                break;
        }

    }

    #region LanguageManagerBoilerPlate

    private void OnEnable()
    {
        textSwitcher.textChanged += this.OnLanguageChange;
        Actions.Instance.OnWeaponDownToggledEvent += OnWeaponDown;
        Actions.Instance.OnWeaponUpToggledEvent += OnWeaponUp;
        Actions.Instance.OnWeaponRightToggledEvent += OnWeaponRight;
        Actions.Instance.OnWeaponLeftToggledEvent += OnWeaponLeft;
        //Actions.Instance.OnAttackTriggeredEvent += OnJump;
        if (SFX != null)
        {
            SFX.OnBlockChangeAction += SFXVolumeChange;
        }

        if (Music != null)
        {
            Music.OnBlockChangeAction += MusicVolumeChange;
        }

        if (Master != null)
        {
            Master.OnBlockChangeAction += MasterVolumeChange;
        }

    }

    private void OnDisable()
    {
        Actions local = Actions.TryGetInstance();
        if (local != null)
        {
            Actions.Instance.OnWeaponDownToggledEvent -= OnWeaponDown;
            Actions.Instance.OnWeaponUpToggledEvent -= OnWeaponUp;
            ///Actions.Instance.OnAttackTriggeredEvent -= OnJump;
        }

        Master.OnBlockChangeAction -= MasterVolumeChange;
        Music.OnBlockChangeAction -= MusicVolumeChange;

        SFX.OnBlockChangeAction -= SFXVolumeChange;

        textSwitcher.textChanged -= this.OnLanguageChange;
    }

    #endregion


    #region VolumeChangeFunctions

    private void MasterVolumeChange(float val)
    {
        SaveSystem.SaveSystem.SaveVolume(SoundType.Master, val);
    }

    private void SFXVolumeChange(float val)
    {
        SaveSystem.SaveSystem.SaveVolume(SoundType.SFX, val);
    }

    private void MusicVolumeChange(float val)
    {
        SaveSystem.SaveSystem.SaveVolume(SoundType.Music, val);
    }

    #endregion

    #region INPUT_EVENTS

    private void OnWeaponDown()
    {
        _downClicked = !_downClicked;
        EDebug.Log(StringUtils.AddColorToString($"{nameof(OnWeaponDown)}", Color.cyan));
        if (MenuInputTypeUtils.haveAnyMatchingBits(MenuInputType.ANY_VERTICAL, blockedInput)) { return; }
        currentInputInUse |= MenuInputType.VERTICAL_DOWN;
        ChangeCurrentSelectionUntilObjectIsFound(true);

        // DONDE CORESPONDA
        // InvokeRepeating (0.5s a 0.75s para iniciar, 0.15s para repetir)
        // (Rutina, basicamente lo que tenías en update pa' que se mueva solo si lo mantienes presionado)
        // La rutina debe checar el estado de "pressed" y matarse sola 
    }

    private void OnWeaponUp()
    {
        _upClicked = !_upClicked;
        EDebug.Log(StringUtils.AddColorToString($"{nameof(OnWeaponUp)}", Color.cyan));
        if (MenuInputTypeUtils.haveAnyMatchingBits(MenuInputType.ANY_VERTICAL, blockedInput)) { return; }
        currentInputInUse |= MenuInputType.VERTICAL_UP;
        ChangeCurrentSelectionUntilObjectIsFound(false);
    }

    private void OnWeaponRight()
    {
        EDebug.Log(StringUtils.AddColorToString($"{nameof(OnWeaponRight)}", Color.cyan));
        if (MenuInputTypeUtils.haveAnyMatchingBits(MenuInputType.ANY_HORIZONTAL, blockedInput)) { return; }
        currentInputInUse |= MenuInputType.HORIZONTAL_RIGHT;
    }

    private void OnWeaponLeft()
    {
        EDebug.Log(StringUtils.AddColorToString($"{nameof(OnWeaponLeft)}", Color.cyan));
        if (MenuInputTypeUtils.haveAnyMatchingBits(MenuInputType.ANY_HORIZONTAL, blockedInput)) { return; }
        currentInputInUse |= MenuInputType.HORIZONTAL_LEFT;
    }

    private void OnJump()
    {
        if (currentState == CURRENT_MENU_STATE.MAIN_MENU && currentSelection == 3)
        {
            menuOptions.SetActive(true);
            currentState = CURRENT_MENU_STATE.OPTIONS;
            currentSelection = 0;
            currentArrayInUse = optionsItems;
            ChangeSelectorPosition();
        }
        else if (currentState == CURRENT_MENU_STATE.OPTIONS && currentSelection == 3)
        {
            menuOptions.SetActive(false);
            currentState = CURRENT_MENU_STATE.MAIN_MENU;
            currentSelection = 0;
            currentArrayInUse = menuItems;
            ChangeSelectorPosition();
        }
    }

    #endregion

    #region MOVE_SELECTOR

    void ChangeSelectorPosition()
    {

        selector.SetParent(currentArrayInUse[currentSelection]);
        selector.anchoredPosition = new Vector2(-90, 0);
        /*if (currentState == CURRENT_MENU_STATE.INTRO || currentState == CURRENT_MENU_STATE.MAIN_MENU)
        {
            selector.SetParent(currentArrayInUse[currentSelection]);
            selector.anchoredPosition = new Vector2(-73, 0);
        }
        else if (currentState == CURRENT_MENU_STATE.OPTIONS)
        {
            selector.SetParent(optionsItems[currentSelection]);
            selector.anchoredPosition = new Vector2(-73, 0);
        }*/
    }

    void ChangeCurrentSelection(bool _add = true)
    {
        if (_add)
        {
            currentSelection++;
            if (currentSelection >= currentArrayInUse.Length)
            {
                currentSelection = 0;
            }
        }
        else
        {
            currentSelection--;
            if (currentSelection < 0)
            {
                currentSelection = currentArrayInUse.Length - 1;
            }
        }
    }

    void ChangeCurrentSelectionUntilObjectIsFound(bool _add = true)
    {
        ChangeCurrentSelection(_add);
        int safey_var = 100_000;
        while (!currentArrayInUse[currentSelection].gameObject.activeInHierarchy)
        {
            ChangeCurrentSelection(_add);
            safey_var -= 1;
            if (safey_var < 0)
            {
                EDebug.LogError("Ended Up in a infinite loop", this);
                break;
            }
        }
        ChangeSelectorPosition();
    }

    #endregion

    public void Inicio()
    {
        EDebug.Log("<color=orange>Inicio</color>");
        LoadingManager.Instance.LoadSceneByName("Scenes/GameLevel");
    }

    public void Cargar()
    {
        EDebug.Log("<color=orange>Cargar</color>");
        SaveSystem.SaveSystem.LoadEverything();
    }

    public void Options()
    {
        EDebug.Log("<color=orange>Opciones</color>");
        desiredState = CURRENT_MENU_STATE.OPTIONS;
    }

    public void Salir()
    {
        EDebug.Log("<color=orange>Quitting</color>");
        Application.Quit();
    }

    #region MOVEMENT_PROCESSING

    private void ProcessVerticalMovement()
    {
        if (MenuInputTypeUtils.haveAnyMatchingBits(MenuInputType.ANY_VERTICAL, blockedInput)) { return; }


        if (cosa.Movement.y > 0.1f)
        {
            currentInputInUse |= MenuInputType.VERTICAL_UP;
            ChangeCurrentSelectionUntilObjectIsFound(false);
            StopCoroutine(BlockVerticalInput());
            _verticalInputBlock = StartCoroutine(BlockVerticalInput());
        }
        else if (cosa.Movement.y < -0.1f)
        {

            currentInputInUse |= MenuInputType.VERTICAL_DOWN;
            ChangeCurrentSelectionUntilObjectIsFound();
            StopCoroutine(BlockVerticalInput());
            _verticalInputBlock = StartCoroutine(BlockVerticalInput());
        }
    }

    private void ProcessHorizontalMovement()
    {
        if (MenuInputTypeUtils.haveAnyMatchingBits(MenuInputType.ANY_HORIZONTAL, blockedInput)) { return; }

        if (cosa.Movement.x > 0.1f)
        {
            currentInputInUse |= MenuInputType.HORIZONTAL_RIGHT;
            StopCoroutine(BlockHorizontalInput());
            _horizontalInputBlock = StartCoroutine(BlockHorizontalInput());
        }
        else if (cosa.Movement.x < -0.1f)
        {
            currentInputInUse |= MenuInputType.HORIZONTAL_LEFT;
            StopCoroutine(BlockHorizontalInput());
            _horizontalInputBlock = StartCoroutine(BlockHorizontalInput());
        }

    }

    #endregion


    #region MenuLogicMethods

    private void ProcessMainMenu()
    {
        ProcessVerticalMovement();

        if (cosa.Jump && !isAcceptedInputBlocked)
        {
            _acceptedInputBlock = StartCoroutine(BlockAcceptedInput());

            if (currentSelection == 0)
            {
                Inicio();
            }

            if (currentSelection == 1)
            {
                Cargar();
            }

            if (currentSelection == 2)
            {
                Options();
            }

            if (currentSelection == 3)
            {
                Salir();
            }
        }

    }

    private void ProcessOptions()
    {
        ProcessVerticalMovement();
        ProcessHorizontalMovement();

        bool rightKeyPressed = MenuInputTypeUtils.haveAnyMatchingBits(MenuInputType.HORIZONTAL_RIGHT, currentInputInUse);
        bool leftKeyPressed = MenuInputTypeUtils.haveAnyMatchingBits(MenuInputType.HORIZONTAL_LEFT, currentInputInUse);

        loadVolumeValues();

        if (currentSelection == 0 && rightKeyPressed)
        {
            SFX.increaseBlocks();
            _horizontalInputBlock = StartCoroutine(BlockHorizontalInput());
        }

        if (currentSelection == 0 && leftKeyPressed)
        {
            SFX.decreaseBlocks();
            _horizontalInputBlock = StartCoroutine(BlockHorizontalInput());
        }

        if (currentSelection == 1 && rightKeyPressed)
        {
            Music.increaseBlocks();
            _horizontalInputBlock = StartCoroutine(BlockHorizontalInput());
        }

        if (currentSelection == 1 && leftKeyPressed)
        {
            Music.decreaseBlocks();
            _horizontalInputBlock = StartCoroutine(BlockHorizontalInput());
        }

        if (currentSelection == 2 && rightKeyPressed)
        {
            Master.increaseBlocks();
            _horizontalInputBlock = StartCoroutine(BlockHorizontalInput());
        }

        if (currentSelection == 2 && leftKeyPressed)
        {
            Master.decreaseBlocks();
            _horizontalInputBlock = StartCoroutine(BlockHorizontalInput());
        }

        if (currentSelection == 3 && rightKeyPressed)
        {
            textSwitcher.IncreaseIndex();
            _horizontalInputBlock = StartCoroutine(BlockHorizontalInput());
        }

        if (currentSelection == 3 && leftKeyPressed)
        {
            textSwitcher.DecreaseIndex();
            _horizontalInputBlock = StartCoroutine(BlockHorizontalInput());
        }


        if (currentSelection == 4 && rightKeyPressed)
        {
            resolutionSwitcher.IncreaseIndex();
            _horizontalInputBlock = StartCoroutine(BlockHorizontalInput());
        }

        if (currentSelection == 4 && leftKeyPressed)
        {
            resolutionSwitcher.DecreaseIndex();
            _horizontalInputBlock = StartCoroutine(BlockHorizontalInput());
        }



        if (cosa.Jump && !isAcceptedInputBlocked)
        {
            _acceptedInputBlock = StartCoroutine(BlockAcceptedInput());
            if (currentSelection == 5)
            {
                desiredState = CURRENT_MENU_STATE.MAIN_MENU;
            }
        }

    }

    private void ProcessSaveFileSelect()
    {

        ProcessVerticalMovement();
        if (cosa.Jump && !isAcceptedInputBlocked)
        {
            _acceptedInputBlock = StartCoroutine(BlockAcceptedInput());

            if (currentSelection > -1 && currentSelection < 4)
            {
                int save_file_index = currentArrayInUse[currentSelection].GetComponent<SaveFileSelectable>().saveFileIndex;
                EDebug.Log($"save file index = {save_file_index}");
                SaveSystem.SaveSystem.CreateKeyIfOneDoesNotExist(save_file_index);
                desiredState = CURRENT_MENU_STATE.MAIN_MENU;
            }

            else if (currentSelection == 4)
            {
                conformationMenu.gameObject.SetActive(true);
                conformationMenu.acceptedInputEvent += DeleteAllDataOnTrue;
            }

        }


    }


    #endregion

    private void DeleteAllDataOnTrue(bool shouldDeleteEverthing)
    {
        EDebug.Log(shouldDeleteEverthing ? "deleted" : "not deleted");
        if (!shouldDeleteEverthing) { return; }

        for (int i = 0; i < currentArrayInUse.Length; i++)
        {
            SaveSystem.SaveSystem.DeleteData(i);
        }
    }

    private void SaveSettingsOnTrue(bool shouldSaveSetting)
    {
        SaveSystem.SaveSystem.SaveLanguageSelection(LanguageManager.Instance.currentLanguage);
        WindowResolution newRes = (WindowResolution)resolutionSwitcher.currentIndex;

    }

    private void OnLanguageChange(string language)
    {
        //EDebug.Log($"<color=orange> language chosen |{language}|</color>", this);

        switch (language)
        {
            case "EN":
                LanguageManager.Instance.setLanguage(Utils.Language.En);
                break;
            case "ES":
                LanguageManager.Instance.setLanguage(Utils.Language.Es);
                break;
        }

    }

    #region Coroutines

    private IEnumerator BlockVerticalInput()
    {
        blockedInput |= MenuInputType.ANY_VERTICAL;
        yield return new WaitForSeconds(verticalInputDelay);
        blockedInput &= ~MenuInputType.ANY_VERTICAL;
    }

    private IEnumerator BlockHorizontalInput()
    {
        blockedInput |= MenuInputType.ANY_HORIZONTAL;
        yield return new WaitForSeconds(horizontalInputDelay);
        blockedInput &= ~MenuInputType.ANY_HORIZONTAL;
    }

    private IEnumerator BlockAcceptedInput()
    {
        isAcceptedInputBlocked = true;
        yield return new WaitForSeconds(acceptedInputDelay);
        isAcceptedInputBlocked = false;
    }

    #endregion

    private void loadVolumeValues()
    {
        if (!hasVolumeValuesBeenLoaded) { hasVolumeValuesBeenLoaded = true; }

        if (SFX != null)
        {
            SFX.setPercent(SaveSystem.SaveSystem.GetVolume(SoundType.SFX));
        }

        if (Music != null)
        {
            Music.setPercent(SaveSystem.SaveSystem.GetVolume(SoundType.Music));
        }

        if (Master != null)
        {
            Master.setPercent(SaveSystem.SaveSystem.GetVolume(SoundType.Master));
        }


    }

}

