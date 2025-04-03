using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
   /* Animator animator;
    [SerializeField] RectTransform selector;
    [SerializeField] Transform[] menuItems;
    [SerializeField] Transform[] optionsItems;
    Transform[] currentArrayInUse;
    int currentSelection = 0;
    CURRENT_MENU_STATE currentState = CURRENT_MENU_STATE.INTRO;
    [SerializeField] GameObject Menuoptions;

    enum CURRENT_MENU_STATE
    {
        INTRO,
        MAIN_MENU,
        OPTIONS
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        currentArrayInUse = menuItems;
        currentState = CURRENT_MENU_STATE.INTRO;
        selector.gameObject.SetActive(false);
        currentSelection = 0;
        ChangeSelectorPosition();
    }
    public void OnIntroFiniched()
    {
        selector.gameObject.SetActive(true);
        currentState = CURRENT_MENU_STATE.MAIN_MENU;
    }
    void ChangeSelectorPosition()
    {
        if(currentState == CURRENT_MENU_STATE.INTRO || currentState == CURRENT_MENU_STATE.MAIN_MENU)
        {
            selector.SetParent(currentArrayInUse[currentSelection]);
            selector.anchoredPosition = new Vector2(-45, 0);
        }
        else if (currentState == CURRENT_MENU_STATE.OPTIONS)
        {
            selector.SetParent(optionsItems[currentSelection]);
            selector.anchoredPosition = new Vector2(-15, 0);
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

    void SkipAnimation() 
    {
        OnIntroFiniched();
        animator.Play(0,0, animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
    }
    void Update()
    {
        switch (currentState)
        {
            case CURRENT_MENU_STATE.INTRO:
                if ()
                {
                    SkipAnimation();
                }
                break;

                case CURRENT_MENU_STATE.MAIN_MENU:
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    ChangeCurrentSelectionUntilObjectIsFound();
                }
                else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    ChangeCurrentSelectionUntilObjectIsFound(false);
                }
                else if(Input.GetKeyDown(KeyCode.Space))
                {
                  if(currentSelection == 3)
                    {
                        Menuoptions.SetActive(true);
                        currentState = CURRENT_MENU_STATE.OPTIONS;
                        currentSelection = 0;
                        currentArrayInUse = optionsItems;
                        ChangeSelectorPosition();
                    }
                }
                break;
                case CURRENT_MENU_STATE.OPTIONS:
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    ChangeCurrentSelectionUntilObjectIsFound();
                }
                else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    ChangeCurrentSelectionUntilObjectIsFound(false);
                }
                else if (Input.GetKeyDown(KeyCode.Space))
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
    }*/
}
