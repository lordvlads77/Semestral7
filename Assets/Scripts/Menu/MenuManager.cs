using System.Collections;
using System.Collections.Generic;
using Input;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    Animator animator;
    [SerializeField] RectTransform selector;
    [SerializeField] Transform[] menuItems;
    [SerializeField] Transform[] optionsItems;
    Transform[] currentArrayInUse;
    [SerializeField] int currentSelection = 0;
    [SerializeField] CURRENT_MENU_STATE currentState = CURRENT_MENU_STATE.INTRO;
    [SerializeField] GameObject Menuoptions;
    Input.Actions cosa;

    enum CURRENT_MENU_STATE
    {
        INTRO,
        MAIN_MENU,
        OPTIONS
    }

    private void OnEnable()
    {
        /*
        Actions.Instance.OnWeaponDownToggledEvent += OnWeaponDown;
        Actions.Instance.OnWeaponUpToggledEvent += OnWeaponUp;
        Actions.Instance.OnAttackTriggeredEvent += OnJump;
        */
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
    }

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
            Menuoptions.SetActive(true);
            currentState = CURRENT_MENU_STATE.OPTIONS;
            currentSelection = 0;
            currentArrayInUse = optionsItems;
            ChangeSelectorPosition();
        }
        else if (currentState == CURRENT_MENU_STATE.OPTIONS && currentSelection == 3)
        {
            Menuoptions.SetActive(false);
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
        //selector.gameObject.SetActive(false);
        currentSelection = 0;
        ChangeSelectorPosition();
    }
    /*  public void OnIntroFiniched()
      {
          selector.gameObject.SetActive(true);
          currentState = CURRENT_MENU_STATE.MAIN_MENU;
      }*/
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
            if (currentSelection >= menuItems.Length)
            {
                currentSelection = 0;
            }
        }
        else
        {
            currentSelection--;
            if (currentSelection < 0)
            {
                currentSelection = menuItems.Length - 1;
            }
        }
    }

    void ChangeCurrentSelectionUntilObjectIsFound(bool _add = true)
    {
        ChangeCurrentSelection(_add);
        while (!menuItems[currentSelection].gameObject.activeInHierarchy)
        {
            ChangeCurrentSelection(_add);
        }
        ChangeSelectorPosition();
    }

    /* void SkipAnimation() 
     {
         OnIntroFiniched();
         animator.Play(0,0, animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
     }*/
    [SerializeField] private float inputCooldown = 0.2f; // Tiempo de espera entre inputs
    private float lastInputTime = 0f;

    private bool isWeaponDownPressed = false;
    private bool isWeaponUpPressed = false;

    private void Update()
    {
        switch (currentState)
        {
            case CURRENT_MENU_STATE.MAIN_MENU:
                ProcessCursorMovement();

                if (cosa.Jump)
                {

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

                if (cosa.Jump)
                {
                    if (currentSelection == 3)
                    {
                        Menuoptions.SetActive(false);
                        currentState = CURRENT_MENU_STATE.MAIN_MENU;
                        currentSelection = 0;
                        currentArrayInUse = menuItems;
                        ChangeSelectorPosition();
                    }
                }
                break;
        }
    }

    public void Inicio()
    {
        EDebug.Log("<color=orange>Inicio</color>");
        SceneManager.LoadScene("Scenes/GameLevel");
    }

    public void Cargar()
    {
        EDebug.Log("<color=orange>Cargar</color>");
    }

    public void Options()
    {
        EDebug.Log("<color=orange>Opciones</color>");
    }

    public void Salir()
    {
        EDebug.Log("<color=orange>Quitting</color>");
        Application.Quit();
    }

    private void ProcessCursorMovement()
    {
        if (cosa.Movement.y > 0.1f)
        {
            ChangeCurrentSelectionUntilObjectIsFound(false);
        }
        else if (cosa.Movement.y < -0.1f)
        {
            ChangeCurrentSelectionUntilObjectIsFound();
        }

    }
}
