using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Input
{
    //[DefaultExecutionOrder(-10)]
    public class Actions : Singleton<Actions> // Input Interface
    {
        // If possible, subscribe to the events, if not, use the public properties
        private PlaInputActions _inputActions;

        public Vector2 Movement { get; private set; }
        public Vector2 Camera { get; private set; }

        [Tooltip(
            "Use this if you want your script to check for this in a specific function (Not recommended for updating or iterating methods)")]
        public WeaponType CurrentWeapon { get; private set; }

        public bool WeaponUp { get; private set; }
        public event Action OnWeaponUpToggledEvent;
        public bool WeaponLeft { get; private set; }
        public event Action OnWeaponLeftToggledEvent;
        public bool WeaponRight { get; private set; }
        public event Action OnWeaponRightToggledEvent;
        public bool WeaponDown { get; private set; }
        public event Action OnWeaponDownToggledEvent;

        public bool UpButton { get; private set; }
        public bool Jump { get; private set; }
        public bool LeftButton { get; private set; }

        public bool Attack { get; private set; }
        public event Action OnAttackTriggeredEvent;
        public bool Crouch { get; private set; }
        public event Action OnCrouchToggledEvent;
        public bool RightStickButton { get; private set; }
        public bool LeftBumper { get; private set; }
        public bool RightBumper { get; private set; }
        public bool ZTarget { get; private set; }
        public bool RightTrigger { get; private set; }
        public bool Pause { get; private set; }

        public bool AttackHeavy { get; private set; }
        public event Action OnAttackHeavySwing;

        public bool Doge { get; private set; }
        public event Action OnDoge;


        protected override void OnAwake()
        {
            if (_inputActions == null) InitStuff();
            EDebug.Log("Input Actions ► Awake");
        }

        private PlaInputActions InitStuff()
        {
            if(_inputActions != null) 
                return _inputActions;
            _inputActions = new PlaInputActions();
            Crouch = false;
            Pause = false;
            WeaponUp = false;
            WeaponLeft = false;
            WeaponRight = false;
            WeaponDown = false;
            AttackHeavy = false;
            CurrentWeapon = WeaponType.Unarmed;
            EDebug.Log("Input Actions ► Initialized");
            return _inputActions;
        }

        private void OnEnable()
        {
            InitStuff().Enable();
            _inputActions.Player.Pause.started += OnPauseToggled;

            _inputActions.Player.MoveVec.started += OnMoveVecStarted;
            _inputActions.Player.MoveVec.performed += OnMoveVecUpdated;
            _inputActions.Player.MoveVec.canceled += OnMoveVecCanceled;

            _inputActions.Player.Crouch.started += OnCrouchToggled;

            _inputActions.Player.CamVec.started += OnCamVecStarted;
            _inputActions.Player.CamVec.performed += OnCamVecUpdated;
            _inputActions.Player.CamVec.canceled += OnCamVecCanceled;

            _inputActions.Player.WeaponUp.started += OnWeaponUpToggled;
            _inputActions.Player.WeaponLeft.started += OnWeaponLeftToggled;
            _inputActions.Player.WeaponRight.started += OnWeaponRightToggled;
            _inputActions.Player.WeaponDown.started += OnWeaponDownToggled;

            //_inputActions.Player.UpButton.performed += context => UpButton = context.ReadValueAsButton();

            _inputActions.Player.Jump.started += OnJumpStarted;
            _inputActions.Player.Jump.performed += OnJumpHeld;
            _inputActions.Player.Jump.canceled += OnJumpCanceled;

            //_inputActions.Player.LeftButton.performed += context => LeftButton = context.ReadValueAsButton();

            _inputActions.Player.Attack.started += OnAttackStarted;
            _inputActions.Player.Attack.performed += OnAttackHeld;
            _inputActions.Player.Attack.canceled += OnAttackCanceled;

            //_inputActions.Player.LB.performed += context => LeftBumper = context.ReadValueAsButton();
            _inputActions.Player.RB.performed += context => RightBumper = context.ReadValueAsButton();

            _inputActions.Player.ZTarget.started += OnZTargetStarted;
            _inputActions.Player.ZTarget.performed += OnZTargetHeld;
            _inputActions.Player.ZTarget.canceled += OnZTargetCanceled;

            //_inputActions.Player.RT.performed += context => RightTrigger = context.ReadValueAsButton();

            _inputActions.Player.UpButton.started += OnAttackHeavySwingToggle;
            _inputActions.Player.UpButton.performed += OnAttackHeavySwingToggle;
            _inputActions.Player.UpButton.canceled += OnAttackHeavySwingToggle;

            _inputActions.Player.LeftButton.started += OnDogeStarted;
            _inputActions.Player.LeftButton.performed += OnDogeHeld;
            _inputActions.Player.LeftButton.canceled += OnDogeCanceled;
            
            EDebug.Log("Input Actions ► Enabled");
        }

        

        private void OnDisable()
        {
            // Unsubscribe from all events because of memory leaks (TDL)
            _inputActions?.Disable();
        }

        #region MovementInput

        private void OnMoveVecStarted(InputAction.CallbackContext context)
        {
            Movement = context.ReadValue<Vector2>();
        }

        private void OnMoveVecUpdated(InputAction.CallbackContext context)
        {
            Movement = context.ReadValue<Vector2>();
        }

        private void OnMoveVecCanceled(InputAction.CallbackContext context)
        {
            Movement = Vector2.zero;
        }

        #endregion

        #region CameraInput

        private void OnCamVecStarted(InputAction.CallbackContext context)
        {
            Vector2 mouseDelta = context.ReadValue<Vector2>();
            //Vector2 normalizedMousePosition = new Vector2(mousePosition.x / Screen.width, mousePosition.y / Screen.height);
            //int activeDisplay = Display.displays.Length > 1 ? Display.displays[0].renderingWidth : Screen.width;
            //Vector2 normalizedMousePosition = new Vector2(mousePosition.x / activeDisplay, mousePosition.y / Screen.height);
            Camera = mouseDelta;
        }

        private void OnCamVecUpdated(InputAction.CallbackContext context)
        {
            Vector2 mouseDelta = context.ReadValue<Vector2>();
            //Vector2 normalizedMousePosition = new Vector2(mousePosition.x / Screen.width, mousePosition.y / Screen.height);
            //int activeDisplay = Display.displays.Length > 1 ? Display.displays[0].renderingWidth : Screen.width;
            //Vector2 normalizedMousePosition = new Vector2(mousePosition.x / activeDisplay, mousePosition.y / Screen.height);
            Camera = mouseDelta;
        } // Ngl, I got tired af and just changed it for the mouse delta instead of the mouse position

        private void OnCamVecCanceled(InputAction.CallbackContext context)
        {
            Camera = Vector2.zero;
        }

        #endregion

        #region JumpInput

        private void OnJumpStarted(InputAction.CallbackContext context)
        {
            Jump = true;
        }

        private void OnJumpHeld(InputAction.CallbackContext context)
        {
            Jump = true;
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            Jump = false;
        }

        #endregion

        #region AttackInput

        private void OnAttackStarted(InputAction.CallbackContext context)
        {
            Attack = context.ReadValueAsButton();
            OnAttackTriggeredEvent?.Invoke();
        }

        private void OnAttackHeld(InputAction.CallbackContext context)
        {
            Attack = context.ReadValueAsButton();
        }

        private void OnAttackCanceled(InputAction.CallbackContext context)
        {
            Attack = context.ReadValueAsButton();
        }

        #endregion

        #region ZTargetInput

        private void OnZTargetStarted(InputAction.CallbackContext context)
        {
            ZTarget = context.ReadValueAsButton();
        }

        private void OnZTargetHeld(InputAction.CallbackContext context)
        {
            ZTarget = context.ReadValueAsButton();
        }

        private void OnZTargetCanceled(InputAction.CallbackContext context)
        {
            ZTarget = context.ReadValueAsButton();
        }

        #endregion

        #region Crouch

        private void OnCrouchToggled(InputAction.CallbackContext context)
        {
            Crouch = !Crouch;
            OnCrouchToggledEvent?.Invoke();
        }

        #endregion

        #region Pause

        private void OnPauseToggled(InputAction.CallbackContext context)
        {
            Pause = !Pause;
        }

        #endregion

        #region WeaponInput

        private void OnWeaponUpToggled(InputAction.CallbackContext context)
        {
            CurrentWeapon = (CurrentWeapon == WeaponType.LightSword) ? WeaponType.Unarmed : WeaponType.LightSword;
            WeaponUp = !WeaponUp;
            OnWeaponUpToggledEvent?.Invoke();
        }

        private void OnWeaponLeftToggled(InputAction.CallbackContext context)
        {
            CurrentWeapon = (CurrentWeapon == WeaponType.GreatSword) ? WeaponType.Unarmed : WeaponType.GreatSword;
            WeaponLeft = !WeaponLeft;
            OnWeaponLeftToggledEvent?.Invoke();
        }

        private void OnWeaponRightToggled(InputAction.CallbackContext context)
        {
            CurrentWeapon = (CurrentWeapon == WeaponType.NamePending3) ? WeaponType.Unarmed : WeaponType.NamePending3;
            WeaponRight = !WeaponRight;
            OnWeaponRightToggledEvent?.Invoke();
        }

        private void OnWeaponDownToggled(InputAction.CallbackContext context)
        {
            CurrentWeapon = (CurrentWeapon == WeaponType.NamePending4) ? WeaponType.Unarmed : WeaponType.NamePending4;
            WeaponDown = !WeaponDown;
            OnWeaponDownToggledEvent?.Invoke();
        }

        #endregion

        private void OnAttackHeavySwingToggle(InputAction.CallbackContext context)
        {
            CurrentWeapon = (CurrentWeapon == WeaponType.GreatSword) ? WeaponType.Unarmed : WeaponType.GreatSword;
            AttackHeavy = !AttackHeavy;
            OnAttackHeavySwing?.Invoke();
        }
        
        #region DogeInput

        private void OnDogeStarted(InputAction.CallbackContext context)
        {
            Doge = true;
            OnDoge?.Invoke();
        }

        private void OnDogeHeld(InputAction.CallbackContext context)
        {
            Doge = true;
        }

        private void OnDogeCanceled(InputAction.CallbackContext context)
        {
            Doge = false;
        }

        #endregion
    }
}