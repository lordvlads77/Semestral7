using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    using UnityEngine.UI;
    using TMPro;
    using System;

    /// <summary>
    /// Para usar el script (assumiendo que ya tengas el prefab)
    /// <para> 1. Asegurar que esta apagado en la ecena donde esta <c> gameObject.SetActive(false); </c>  </para>
    /// <para> 2. Cuando necesitas usarlo Activalo <c> gameObject.SetActive(false); </c> </para> 
    /// <para> 3. Asignar el evento <c> conformationMenu.acceptedInputEvent += otraFuncion </c> y tienes que hacer esto cada vez que lo usas </para>
    /// </summary>
    public sealed class ConformationMenu : MonoBehaviour
    {
        [SerializeField] Image background;
        [SerializeField] TextMeshProUGUI confirmationMessage;
        [SerializeField] TextMeshProUGUI yesText;
        [SerializeField] TextMeshProUGUI noText;
        [SerializeField] RectTransform selector;
        [SerializeField] Input.Actions inputReceiver;
        [SerializeField] float inputDelay = .15f;
        [SerializeField] float selectorDistanceFromParent = -80.0f;

        MenuInputType currentInput;
        MenuInputType blockedInput;

        /*Coroutine horizontalCo;
        Coroutine acceptedCo;*/

        /// <summary>
        /// True = el usario preciono 'Yes', false = el usario precino 'NO'
        /// </summary>
        public Action<bool> acceptedInputEvent;

        void Start()
        {
            inputReceiver = GameManager.Instance.GetComponent<Input.Actions>();
        }

        void Update()
        {
            ProcessInput();
            DoActionFromInput();
            currentInput = MenuInputType.NONE;
            //GameManager.Instance.SetGameWindowAndResolution()
        }

        private void OnEnable()
        {
            StartCoroutine(MenuInputTypeUtils.setWaitThenUnsetBit(MenuInputType.ACCEPTED, inputDelay, new Utils.Ref<MenuInputType>(ref blockedInput)));
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            blockedInput = MenuInputType.NONE;
            foreach (var d in acceptedInputEvent.GetInvocationList())
            {
                acceptedInputEvent -= (Action<bool>)d;
            }
        }

        #region InputProcessing

        private void ProcessInput()
        {
            bool should_deny_horizontal_input = MenuInputTypeUtils.haveAnyMatchingBits(MenuInputType.ANY_HORIZONTAL, blockedInput);
            bool should_deny_accepted = MenuInputTypeUtils.haveAnyMatchingBits(MenuInputType.ACCEPTED, blockedInput);

            if (!should_deny_horizontal_input)
            {
                ProcessHorizontalInput();
            }

            if (!should_deny_accepted)
            {
                ProcessAcceptedInput();
            }


        }

        private void ProcessHorizontalInput()
        {

            if (inputReceiver.Movement.x > 0.1f)
            {
                StartCoroutine(MenuInputTypeUtils.setWaitThenUnsetBit(MenuInputType.ANY_HORIZONTAL, inputDelay, new Utils.Ref<MenuInputType>(ref blockedInput)));
                currentInput |= MenuInputType.HORIZONTAL_RIGHT;
            }
            else if (inputReceiver.Movement.x < -0.1f)
            {
                StartCoroutine(MenuInputTypeUtils.setWaitThenUnsetBit(MenuInputType.ANY_HORIZONTAL, inputDelay, new Utils.Ref<MenuInputType>(ref blockedInput)));
                currentInput |= MenuInputType.HORIZONTAL_LEFT;
            }
        }

        private void ProcessAcceptedInput()
        {
            if (inputReceiver.Jump)
            {
                StartCoroutine(MenuInputTypeUtils.setWaitThenUnsetBit(MenuInputType.ACCEPTED, inputDelay, new Utils.Ref<MenuInputType>(ref blockedInput)));
                currentInput |= MenuInputType.ACCEPTED;
            }
        }

        #endregion

        #region DoAcction

        private void DoActionFromInput()
        {

            switch (currentInput)
            {
                case MenuInputType.ACCEPTED:
                    bool hasPressedYes = selector.transform.parent.Equals(yesText.transform);
                    acceptedInputEvent?.Invoke(hasPressedYes);
                    gameObject.SetActive(false);
                    break;
                case MenuInputType.HORIZONTAL_LEFT:
                    MoveSelector(true);
                    break;
                case MenuInputType.HORIZONTAL_RIGHT:
                    MoveSelector(false);
                    break;
                case MenuInputType.NONE:
                    break;
                default:
                    EDebug.LogWarning($"Un handled case {currentInput} in {nameof(DoActionFromInput)}", this);
                    break;
            }

        }

        #endregion

        private void MoveSelector(bool moveToYes)
        {
            if (moveToYes)
            {
                selector.SetParent(yesText.transform);
                selector.anchoredPosition = new Vector2(selectorDistanceFromParent, 0);
            }
            else
            {
                selector.SetParent(noText.transform);
                selector.anchoredPosition = new Vector2(selectorDistanceFromParent, 0);
            }

        }

        #region Coroutines

        IEnumerator blockHorizontalInput()
        {
            blockedInput |= MenuInputType.ANY_HORIZONTAL;
            yield return new WaitForSeconds(inputDelay);
            MenuInputTypeUtils.unsetBit(MenuInputType.ANY_HORIZONTAL, ref blockedInput);
        }

        #endregion

    }



}

