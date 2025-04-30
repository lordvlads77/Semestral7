using System.Collections;
using System.Collections.Generic;
using Input;
using UI;
using UnityEngine;
using Utils;

public sealed class MenuManager : MonoBehaviour
{
    Animator animator;
    [SerializeField] RectTransform selector;
    [SerializeField] Transform[] menuItems;
    [SerializeField] Transform[] optionsItems;
    Transform[] currentArrayInUse;
    [SerializeField] int currentSelection = 0;
    [SerializeField] CURRENT_MENU_STATE currentState = CURRENT_MENU_STATE.INTRO;
    CURRENT_MENU_STATE desiredState = CURRENT_MENU_STATE.INTRO;
    [SerializeField] GameObject menuInicio;
    [SerializeField] GameObject menuOptions;
    Input.Actions cosa;

    [Header("BlockySlider")]
    [SerializeField] private BlockySlider SFX;
    [SerializeField] private BlockySlider Music;
    [SerializeField] private BlockySlider Master;

    [Header("TextSwitcher")]
    [SerializeField] private UI.TextSwitcher textSwitcher;

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
    bool isVerticalInputBlocked = false;
    bool isHorizontalInputBlocked = false;
    bool isAcceptedInputBlocked = false;

    enum CURRENT_MENU_STATE
    {
        INTRO,
        MAIN_MENU,
        OPTIONS
    }

    #region LanguageManagerBoilerPlate

    private void OnEnable()
    {
        textSwitcher.textChanged += this.OnLanguageChange;
        /*
        Actions.Instance.OnWeaponDownToggledEvent += OnWeaponDown;
        Actions.Instance.OnWeaponUpToggledEvent += OnWeaponUp;
        Actions.Instance.OnAttackTriggeredEvent += OnJump;
        */
        if (SFX != null)
            SFX.OnBlockChangeAction += VolumeChanged;
    }

    private void OnDisable()
    {
        Actions local = Actions.TryGetInstance();
        if (local != null)
        {
            /*
            Actions.Instance.OnWeaponDownToggledEvent -= OnWeaponDown;
            Actions.Instance.OnWeaponUpToggledEvent -= OnWeaponUp;
            Actions.Instance.OnAttackTriggeredEvent -= OnJump;
            */
        }

        textSwitcher.textChanged -= this.OnLanguageChange;
    }

    private void VolumeChanged(float val)
    {

    }

    #endregion

    #region INPUT_EVENTS

    private void OnWeaponDown()
    {
        ChangeCurrentSelectionUntilObjectIsFound();
    }

    private void OnWeaponUp()
    {
        ChangeCurrentSelectionUntilObjectIsFound(false);
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

    private void Awake()
    {
        cosa = Actions.Instance;
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        currentArrayInUse = menuItems;
        currentState = CURRENT_MENU_STATE.MAIN_MENU;
        desiredState = CURRENT_MENU_STATE.MAIN_MENU;
        //selector.gameObject.SetActive(false);
        currentSelection = 0;
        ChangeSelectorPosition();
        EDebug.Assert(textSwitcher != null, $"El script necesita un {nameof(TextSwitcher)}", this);
    }


    #region MOVE_SELECTOR

    void ChangeSelectorPosition()
    {
        if (currentState == CURRENT_MENU_STATE.INTRO || currentState == CURRENT_MENU_STATE.MAIN_MENU)
        {
            selector.SetParent(currentArrayInUse[currentSelection]);
            selector.anchoredPosition = new Vector2(-73, 0);
        }
        else if (currentState == CURRENT_MENU_STATE.OPTIONS)
        {
            selector.SetParent(optionsItems[currentSelection]);
            selector.anchoredPosition = new Vector2(-73, 0);
        }
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


    private void Update()
    {
        UpdateStates();

        switch (currentState)
        {
            case CURRENT_MENU_STATE.MAIN_MENU:
                ProcessCursorMovement();

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
                break;

            case CURRENT_MENU_STATE.OPTIONS:
                ProcessCursorMovement();
                if (isHorizontalInputBlocked) { return; }

                bool rightKeyPressed = cosa.Movement.x > 0.1f;
                bool leftKeyPressed = cosa.Movement.x < -0.1f;

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


                if (cosa.Jump && !isAcceptedInputBlocked)
                {
                    _acceptedInputBlock = StartCoroutine(BlockAcceptedInput());
                    if (currentSelection == 4)
                    {
                        desiredState = CURRENT_MENU_STATE.MAIN_MENU;
                    }
                }
                break;
        }
    }

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

    /*
     
    if (_menuMovementCoroutine != null)
                        StopCoroutine(_menuMovementCoroutine );
                    _menuMovementCoroutine = StartCoroutine(MenuMoveInput()); 
private IEnumerator MenuMoveInput()
        {
            yield return new WaitForSeconds(menuInputCd);
            // Logic for moving menu stuff
        } 
private Coroutine _menuMovementCoroutine; 
     
     */

    private void ProcessCursorMovement()
    {
        if (isVerticalInputBlocked) { return; }


        if (cosa.Movement.y > 0.1f)
        {
            ChangeCurrentSelectionUntilObjectIsFound(false);
            StopCoroutine(BlockVerticalInput());
            _verticalInputBlock = StartCoroutine(BlockVerticalInput());
        }
        else if (cosa.Movement.y < -0.1f)
        {
            ChangeCurrentSelectionUntilObjectIsFound();
            StopCoroutine(BlockVerticalInput());
            _verticalInputBlock = StartCoroutine(BlockVerticalInput());
        }

        else if (cosa.WeaponDown)
        {
            Debug.Log("WeaponDown ", this);
        }

        else if (cosa.WeaponUp)
        {
            Debug.Log("WeaponDown ", this);
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
                break;

            case CURRENT_MENU_STATE.OPTIONS:
                currentSelection = 0;
                currentArrayInUse = optionsItems;
                menuOptions.SetActive(true);
                menuInicio.SetActive(false);
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
                // si no existe un elemento zero buscar hasta encontar uno 
                ChangeCurrentSelectionUntilObjectIsFound();
                ChangeCurrentSelectionUntilObjectIsFound(false);

                ChangeSelectorPosition();
                break;

            default:
                EDebug.LogError("Falta evaluar condicion", this);
                break;
        }

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
        isVerticalInputBlocked = true;
        yield return new WaitForSeconds(verticalInputDelay);
        isVerticalInputBlocked = false;
    }

    private IEnumerator BlockHorizontalInput()
    {
        isHorizontalInputBlocked = true;
        yield return new WaitForSeconds(horizontalInputDelay);
        isHorizontalInputBlocked = false;
    }

    private IEnumerator BlockAcceptedInput()
    {
        isAcceptedInputBlocked = true;
        yield return new WaitForSeconds(acceptedInputDelay);
        isAcceptedInputBlocked = false;
    }

    #endregion
}

