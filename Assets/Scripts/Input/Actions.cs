using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Input
{
    public enum WeaponType
    {
        NamePending1,
        NamePending2,
        NamePending3,
        NamePending4,
        Unarmed
    }
        
    public class Actions : MonoBehaviour //: Singleton<IInput> // Input Interface
    {
        private PlaInputActions _inputActions;
        
        public Vector2 Movement { get; private set; }
        public Vector2 Camera { get; private set; }
        
        public WeaponType CurrentWeapon { get; private set; }
        
        public bool UpButton { get; private set; }
        public bool Jump { get; private set; }
        public bool LeftButton { get; private set; }
        
        public bool Attack { get; private set; }
        public bool Crouch { get; private set; }
        public event Action OnCrouchToggledEvent;
        public bool RightStickButton { get; private set; }
        public bool LeftBumper { get; private set; }
        public bool RightBumper { get; private set; }
        public bool ZTarget { get; private set; }
        public bool RightTrigger { get; private set; }
        public bool Pause { get; private set; }
        
        protected void Awake()
        {
            _inputActions = new PlaInputActions();
            Crouch = false;
            Pause = false;
            CurrentWeapon = WeaponType.Unarmed;
        }

        private void OnEnable()
        {
            _inputActions.Enable();
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
            //_inputActions.Player.RB.performed += context => RightBumper = context.ReadValueAsButton();
            
            _inputActions.Player.ZTarget.started += OnZTargetStarted;
            _inputActions.Player.ZTarget.performed += OnZTargetHeld;
            _inputActions.Player.ZTarget.canceled += OnZTargetCanceled;
            
            //_inputActions.Player.RT.performed += context => RightTrigger = context.ReadValueAsButton();
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
            Attack = true;
        }
        private void OnAttackHeld(InputAction.CallbackContext context)
        {
            Attack = true;
        }
        private void OnAttackCanceled(InputAction.CallbackContext context)
        {
            Attack = false;
        }
        #endregion
        
        #region ZTargetInput
        private void OnZTargetStarted(InputAction.CallbackContext context)
        {
            ZTarget = true;
        }
        private void OnZTargetHeld(InputAction.CallbackContext context)
        {
            ZTarget = true;
        }
        private void OnZTargetCanceled(InputAction.CallbackContext context)
        {
            ZTarget = false;
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
            CurrentWeapon = (CurrentWeapon==WeaponType.NamePending1)? WeaponType.Unarmed : WeaponType.NamePending1;
        }
        private void OnWeaponLeftToggled(InputAction.CallbackContext context)
        {
            CurrentWeapon = (CurrentWeapon==WeaponType.NamePending2)? WeaponType.Unarmed : WeaponType.NamePending2;
        }
        private void OnWeaponRightToggled(InputAction.CallbackContext context)
        {
            CurrentWeapon = (CurrentWeapon==WeaponType.NamePending3)? WeaponType.Unarmed : WeaponType.NamePending3;
        }
        private void OnWeaponDownToggled(InputAction.CallbackContext context)
        {
            CurrentWeapon = (CurrentWeapon==WeaponType.NamePending4)? WeaponType.Unarmed : WeaponType.NamePending4;
        }
        #endregion
        
    }
}