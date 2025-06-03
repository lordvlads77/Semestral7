using System.Collections;
using System.Collections.Generic;
using Input;
using UI;
using UnityEngine;
using Utils;
using SaveSystem;
using UnityEditor;

public sealed class MenuManager : MonoBehaviour
{
    Input.Actions inputReceiver;
    [Header("Menu Selector")]
    [SerializeField] RectTransform selector;
    [SerializeField] int currentSelection = 0;

    [Header("Menu Elements For Menu Items")]
    [SerializeField] Transform[] menuItems;
    [SerializeField] Transform[] optionsItems;
    [SerializeField] Transform[] saveFileItems;
    [SerializeField] Transform[] deleteFileItems;
    Transform[] currentArrayInUse;

    [Header("Menu State")]
    [SerializeField] CURRENT_MENU_STATE currentState = CURRENT_MENU_STATE.INTRO;
    CURRENT_MENU_STATE desiredState = CURRENT_MENU_STATE.INTRO;

    [Header("Menu Parent Objects")]
    [SerializeField] GameObject menuInicio;
    [SerializeField] GameObject menuOptions;
    [SerializeField] GameObject SaveFileOptions;
    [SerializeField] GameObject DeleteFileOptions;

    [Header("BlockySlider")]
    [SerializeField] private BlockySlider SFX;
    [SerializeField] private BlockySlider Music;
    [SerializeField] private BlockySlider Master;

    [Header("TextSwitcher")]
    [SerializeField] private UI.TextSwitcher languageSwitcher;
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
    private bool _rightClick = false;
    private bool _leftClick = false;

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
        SAVE_FILE_DELETE,
    }

    private void Awake()
    {
        inputReceiver = GameManager.Instance.GetComponent<Actions>(); // Actions.Instance;
    }

    void Start()
    {
        currentArrayInUse = saveFileItems;
        currentState = CURRENT_MENU_STATE.MAIN_MENU;
        desiredState = CURRENT_MENU_STATE.SAVE_FILE_SELECT;
        //selector.gameObject.SetActive(false);
        currentSelection = 0;
        ChangeSelectorPosition();
        EDebug.Assert(languageSwitcher != null, $"El script necesita un {nameof(TextSwitcher)}", this);
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
            case CURRENT_MENU_STATE.SAVE_FILE_DELETE:
                ProcessDeleteSaveFileSelect();
//                EDebug.LogError($"non handled case |{currentState}|", this);
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
                DeleteFileOptions.SetActive(false);
                break;

            case CURRENT_MENU_STATE.OPTIONS:
                currentSelection = 0;
                currentArrayInUse = optionsItems;
                menuOptions.SetActive(true);
                menuInicio.SetActive(false);
                SaveFileOptions.SetActive(false);
                DeleteFileOptions.SetActive(false);
                Language lang = LanguageManager.Instance.currentLanguage;

                switch (lang)
                {
                    case Language.En:
                        languageSwitcher.setIndex(0);
                        break;

                    case Language.Es:
                        languageSwitcher.setIndex(1);
                        break;
                }

                StartCoroutine(UpdateSettingNextFrame());

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

                int file_index = SaveSystem.SaveSystem.CurrentSaveFileIndex;
                if (SaveSystem.SaveSystem.isSaveFileEmpty(file_index))
                {
                    currentArrayInUse[2].gameObject.SetActive(false);
                }

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
                DeleteFileOptions.SetActive(false);

                ChangeSelectorPosition();
                hasVolumeValuesBeenLoaded = false;
                StartCoroutine(UpdateTextNextFrame());
                break;
            case CURRENT_MENU_STATE.SAVE_FILE_DELETE:
                currentSelection = 0;
                currentArrayInUse = deleteFileItems;

                menuOptions.SetActive(false);
                menuInicio.SetActive(false);
                SaveFileOptions.SetActive(false);
                DeleteFileOptions.SetActive(true);

                ChangeSelectorPosition();
                hasVolumeValuesBeenLoaded = false;
                break;

            default:
                EDebug.LogError($"Falta evaluar condicion = |{currentState}|", this);
                break;
        }

    }

    #region LanguageManagerBoilerPlate

    private void OnEnable()
    {
        languageSwitcher.textChanged += this.OnLanguageChange;
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

        languageSwitcher.textChanged -= this.OnLanguageChange;
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


        // DONDE CORESPONDA
        // InvokeRepeating (0.5s a 0.75s para iniciar, 0.15s para repetir)
        // (Rutina, basicamente lo que tenías en update pa' que se mueva solo si lo mantienes presionado)
        // La rutina debe checar el estado de "pressed" y matarse sola 

        InvokeRepeating(nameof(ReapeatingOnWeaponRight), 0.5f, 1.0f);
    }

    private void OnWeaponLeft()
    {
        EDebug.Log(StringUtils.AddColorToString($"{nameof(OnWeaponLeft)}", Color.cyan));
        if (MenuInputTypeUtils.haveAnyMatchingBits(MenuInputType.ANY_HORIZONTAL, blockedInput)) { return; }
        currentInputInUse |= MenuInputType.HORIZONTAL_LEFT;

        InvokeRepeating(nameof(ReapeatingOnWeaponLeft), 0.5f, 1.0f);
    }
    /// <summary>
    /// TODO : REMOVE THIS FUNCTION
    /// </summary>

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

    #region RepeatingFunctions

    private void ReapeatingOnWeaponRight()
    {
        if (!MenuInputTypeUtils.haveAnyMatchingBits(MenuInputType.ANY_HORIZONTAL, blockedInput)) 
        { 
            CancelInvoke(nameof(ReapeatingOnWeaponRight));
            return; 
        }
        currentInputInUse |= MenuInputType.HORIZONTAL_RIGHT;
    }

    private void ReapeatingOnWeaponLeft()
    {

        if (!MenuInputTypeUtils.haveAnyMatchingBits(MenuInputType.ANY_HORIZONTAL, blockedInput)) 
        { 
            CancelInvoke(nameof(ReapeatingOnWeaponLeft));
            return; 
        }
        currentInputInUse |= MenuInputType.HORIZONTAL_LEFT;

    }


    #endregion

    #region MOVE_SELECTOR

    void ChangeSelectorPosition()
    {
        selector.SetParent(currentArrayInUse[currentSelection]);
        selector.anchoredPosition = new Vector2(-90, 0);
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

    public void Tutorial()
    {
        EDebug.Log("<color=orange>Tutorial</color>");

        LoadingManager.Instance.LoadSceneByName("Scenes/TutorialLevel Nuevo");
    }

    public void Cargar()
    {
        EDebug.Log("<color=orange>Cargar</color>");
        int current_index = SaveSystem.SaveSystem.CurrentSaveFileIndex;
        if (!SaveSystem.SaveSystem.isSaveFileEmpty(current_index))
        {
            SaveSystem.SaveSystem.LoadPlayerAndLevel(current_index);
            //StartCoroutine(SaveSystem.SaveSystem.LoadEverything2(current_index));
        }

    }

    public void Options()
    {
        EDebug.Log("<color=orange>Opciones</color>");
        desiredState = CURRENT_MENU_STATE.OPTIONS;
    }

    public void Credits()
    {
        LoadingManager.Instance.LoadSceneByName("Creditos");
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


        if (inputReceiver.Movement.y > 0.1f)
        {
            currentInputInUse |= MenuInputType.VERTICAL_UP;
            ChangeCurrentSelectionUntilObjectIsFound(false);
            StopCoroutine(BlockVerticalInput());
            _verticalInputBlock = StartCoroutine(BlockVerticalInput());
        }
        else if (inputReceiver.Movement.y < -0.1f)
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

        if (inputReceiver.Movement.x > 0.1f)
        {
            currentInputInUse |= MenuInputType.HORIZONTAL_RIGHT;
            StopCoroutine(BlockHorizontalInput());
            _horizontalInputBlock = StartCoroutine(BlockHorizontalInput());
        }
        else if (inputReceiver.Movement.x < -0.1f)
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

        if (inputReceiver.Jump && !isAcceptedInputBlocked)
        {
            _acceptedInputBlock = StartCoroutine(BlockAcceptedInput());


            switch (currentSelection)
            {
                case 0:
                    Inicio();
                    break;

                case 1:
                    Tutorial();
                    break;

                case 2:
                    Cargar();
                    break;

                case 3:
                    Options();
                    break;

                case 4:
                    Credits();
                    break;

                case 5:
                    Salir();
                    break;
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
            languageSwitcher.IncreaseIndex();
            _horizontalInputBlock = StartCoroutine(BlockHorizontalInput());
        }

        if (currentSelection == 3 && leftKeyPressed)
        {
            languageSwitcher.DecreaseIndex();
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



        if (inputReceiver.Jump && !isAcceptedInputBlocked)
        {

            _acceptedInputBlock = StartCoroutine(BlockAcceptedInput());
            if (currentSelection == 5)
            {
                conformationMenu.gameObject.SetActive(true);
                conformationMenu.acceptedInputEvent += SaveSettingsOnTrue;
                //desiredState = CURRENT_MENU_STATE.MAIN_MENU;
            }
        }

    }

    private void ProcessSaveFileSelect()
    {

        ProcessVerticalMovement();
        if (inputReceiver.Jump && !isAcceptedInputBlocked)
        {
            _acceptedInputBlock = StartCoroutine(BlockAcceptedInput());

            if (currentSelection > -1 && currentSelection < 4)
            {
                SaveFileSelectable saveFile = currentArrayInUse[currentSelection].GetComponent<SaveFileSelectable>();
                int save_file_index = saveFile.saveFileIndex;
                EDebug.Log($"save file index = {save_file_index}");
                SaveSystem.SaveSystem.setSaveFileIndex(save_file_index);
                bool makeEmpySaveFile = !SaveSystem.SaveSystem.DoesSaveFileExist(save_file_index);
                if (makeEmpySaveFile)
                {
                    SaveSystem.SaveSystem.CreateEmptySaveFile(save_file_index);
                }
                desiredState = CURRENT_MENU_STATE.MAIN_MENU;
            }

            else if (currentSelection == 4)
            {

                desiredState = CURRENT_MENU_STATE.SAVE_FILE_DELETE;
            }

            else if (currentSelection == 5)
            {
                conformationMenu.gameObject.SetActive(true);
                conformationMenu.acceptedInputEvent += DeleteAllDataOnTrue;
            }

        }

    }

    private void ProcessDeleteSaveFileSelect()
    {
        ProcessVerticalMovement();
        if (inputReceiver.Jump && !isAcceptedInputBlocked)
        {
            _acceptedInputBlock = StartCoroutine(BlockAcceptedInput());
            if (currentSelection > -1 && currentSelection < 4) 
            { 
                conformationMenu.gameObject.SetActive(true);
                conformationMenu.acceptedInputEvent += DeleteSaveFileOnTrue;
            }
            else if(currentSelection == 4)
            {
                desiredState = CURRENT_MENU_STATE.SAVE_FILE_SELECT;
            }

        }

    }

    #endregion


    #region ConfirmationMenuEvents

    private void DeleteAllDataOnTrue(bool shouldDeleteEverthing)
    {
        if (!shouldDeleteEverthing) { return; }

        for (int i = 0; i < currentArrayInUse.Length; i++)
        {
            SaveSystem.SaveSystem.DeleteData(i);
        }
        Language current_lang = LanguageManager.Instance.currentLanguage;
        LanguageManager.Instance.ForceLanguageChange(current_lang);
    }

    private void DeleteSaveFileOnTrue(bool shouldDeleteFile)
    {
        if(!shouldDeleteFile) { return; }
        SaveSystem.SaveSystem.DeleteData(currentSelection);
    }

    private void SaveSettingsOnTrue(bool shouldSaveSetting)
    {
        desiredState = CURRENT_MENU_STATE.MAIN_MENU;
        if (!shouldSaveSetting) { return; }

        SaveSystem.SaveSystem.SaveLanguageSelection(LanguageManager.Instance.currentLanguage);
        //SaveSystem.SaveSystem.SaveWindowResolution()
        WindowResolution winResult = WindowResolution.R640X480;
        WindowResolution temp = WindowResolution.R640X480;
        if (WindowEnumUtils.strToWin.TryGetValue(resolutionSwitcher.getCurrentString, out temp))
        {
            winResult = temp;
        }

        SaveSystem.SaveSystem.SaveWindowResolution(winResult);
        GameManager.Instance.ChangeResolution(winResult);
    }

    #endregion

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

    private IEnumerator UpdateSettingNextFrame()
    {
        yield return new WaitForEndOfFrame();

        WindowResolution winRes = SaveSystem.SaveSystem.GetWindowResolution();
        string resolutionString = "640X480";

        if (Utils.WindowEnumUtils.winToStr.TryGetValue(winRes, out resolutionString))
        {
            resolutionSwitcher.setIndex(0);
            for (int i = 0; resolutionSwitcher.indexCount > i; ++i)
            {
                if (resolutionString.ToLower() == resolutionSwitcher.getStringAtIndex((uint)i).ToLower())
                {
                    resolutionSwitcher.ManualUpdate();
                    break;
                }

                resolutionSwitcher.IncreaseIndex();

            }

        }
        yield return new WaitForEndOfFrame();

    }

    private IEnumerator UpdateTextNextFrame()
    {
        yield return new WaitForEndOfFrame();
        Language lang = LanguageManager.Instance.currentLanguage;
        LanguageManager.Instance.ForceLanguageChange(lang);
        yield return new WaitForEndOfFrame();
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

