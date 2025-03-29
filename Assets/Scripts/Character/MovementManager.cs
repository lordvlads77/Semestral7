using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Character
{
    public class MovementManager : LivingEntity
    {
        private static readonly int AnimVInput = Animator.StringToHash("VInput");
        private static readonly int AnimHInput = Animator.StringToHash("HInput");
        private static readonly int AnimJump = Animator.StringToHash("Jump");
        private static readonly int AnimAttack = Animator.StringToHash("Attack");
        private static readonly int AnimGrounded = Animator.StringToHash("Grounded");
        // Added those fields to avoid string lookups in the animator (Slightly better performance)

        [Header("Movement Settings")]
        [Tooltip("The current speed, it changes based on the character state (Not hidden in inspector for debugging)")]
        public float currentMovementSpeed;
        [Tooltip("X= Forward, Y= Backward, Z= Sideways")]
        public Vector3 walkSpeeds = new Vector3(4f, 2f, 2f);
        [Tooltip("X= Forward, Y= Backward, Z= Sideways")]
        public Vector3 runSpeeds = new Vector3(8f, 4f, 6f);
        [Tooltip("X= Forward, Y= Backward, Z= Sideways")]
        public Vector3 sprintSpeeds = new Vector3(10f, 6f, 8f);
        [Tooltip("X= Forward, Y= Backward, Z= Sideways")]
        public Vector3 crouchSpeeds = new Vector3(1f, 0.75f, 0.75f);
        [SerializeField] private float groundYOffset;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField, Range(5f, 15f)] private float gravity = 9.81f;
        [SerializeField] private float jumpHeight = 2f;
        [HideInInspector] public Vector3 dir;
        [HideInInspector] public float horizontalInput, verticalInput;
    
        [SerializeField] private CharacterController controller;
        private Vector3 _spherePos;
        private Vector3 _velocity;
    
        [SerializeField] private StateManager stateManager;
        public Animator anim;
        
        private Camera _cam;
        private ThirdPersonCamera _cm;
        
        private void Awake()
        {
            anim = GetComponent<Animator>();
            controller = GetComponent<CharacterController>();
            stateManager.EnterMovementState(MovementState.Walk, this);
            _cam = Camera.main;
            IInput = (Input.Actions.Instance != null)? Input.Actions.Instance : MiscUtils.GetOrCreateGameManager().gameObject.GetComponent<Input.Actions>();
            if (_cam) _cm = _cam.GetComponent<ThirdPersonCamera>();
            if (!_cm) _cm = GetComponent<ThirdPersonCamera>(); // You had the script here, right
            SetHealth(GetMaxHealth());
        }
        
        private void OnEnable()
        {
            IInput.OnCrouchToggledEvent += ToggleCrouch;
            IInput.OnAttackTriggeredEvent += Punch;
        }
        private void OnDestroy()
        {
            UnSubscribe();
        }
        private void OnDisable()
        {
            UnSubscribe();
        }
        
        private void UnSubscribe()
        {
            IInput.OnCrouchToggledEvent -= ToggleCrouch;
        }
        
        private void Update()
        {
            if (isDead) {
                stateManager.EnterMovementState(MovementState.Dead, this);
                return;
            }
            GetDirectionAndMove();
            ApplyGravity();
            HandleActions();
            stateManager.UpdateMovementState(this);
        }

        private void FixedUpdate()
        {
            if (_cm.type == CameraTypes.FreeLook)
                _cam.transform.position += _velocity * Time.deltaTime;
        }

        public void SwitchMovementState(MovementState state)
        {
            stateManager.EnterMovementState(state, this);
        }
        
        private void HandleActions() // This method will be refactored later (Inputs n shit)
        {
            if (IInput.Jump && IsGrounded())
            {
                Jump();
            }
        }
        
        private void GetDirectionAndMove()
        {
            horizontalInput = IInput.Movement.x;
            verticalInput = IInput.Movement.y;
            anim.SetFloat(AnimVInput, verticalInput);
            anim.SetFloat(AnimHInput, horizontalInput);
            Vector3[] camVec = MathUtils.CanonBasis(_cam.transform);
            dir = (camVec[0] * verticalInput + camVec[1] * horizontalInput).normalized;

            Vector3 speeds = walkSpeeds;
            if (IInput.LeftBumper)
                speeds = runSpeeds;
            else if (stateManager.CurrentMovementState == MovementState.Crouch)
                speeds = crouchSpeeds;
            else if (stateManager.CurrentMovementState == MovementState.Sprint)
                speeds = sprintSpeeds;
            
            float forwardSpeed = Mathf.Lerp(0, speeds.x, Mathf.Abs(verticalInput));
            float backwardSpeed = Mathf.Lerp(0, speeds.y, Mathf.Abs(verticalInput));
            float sideSpeed = Mathf.Lerp(0, speeds.z, Mathf.Abs(horizontalInput));
            
            if (verticalInput > 0)
                currentMovementSpeed = forwardSpeed;
            else if (verticalInput < 0)
                currentMovementSpeed = backwardSpeed;
            else
                currentMovementSpeed = sideSpeed;
            if (verticalInput != 0 && horizontalInput != 0)
                currentMovementSpeed = Mathf.Lerp(currentMovementSpeed, sideSpeed, 0.5f);
            
            if (dir.magnitude > 0)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(camVec[0]), Time.deltaTime * 10f);
            
            controller.Move(dir * (currentMovementSpeed * Time.deltaTime));
        }
        
        private void Jump()
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
            anim.SetTrigger(AnimJump);
        }
        
        private void ToggleCrouch()
        {
            stateManager.EnterMovementState(stateManager.CurrentMovementState == MovementState.Crouch? MovementState.Walk : MovementState.Crouch, this);
        }
        
        private void Punch()
        {
            if (stateManager.CurrentFightingState == FightingState.NonCombat && GameManager.Instance.NpcCloseBy(transform.position))
            {
                // Start the dialogue thing
            }

            anim.SetTrigger(AnimAttack);
        }
        
        private bool IsGrounded()
        {
            Vector3 vec = this.transform.position;
            _spherePos = new Vector3(vec.x, vec.y - groundYOffset, vec.z);
            return Physics.CheckSphere(_spherePos, controller.radius - 0.05f, groundLayer);
        }
        
        private void ApplyGravity()
        {
            if (!IsGrounded())
            {
                _velocity.y -= gravity * Time.deltaTime;
                anim.SetBool(AnimGrounded, false);
            }
            else if (_velocity.y < 0)
            {
                _velocity.y = -2f;
            }
            anim.SetBool(AnimGrounded, true);
            controller.Move(_velocity * Time.deltaTime);
        }
        
        private readonly Color _groundCheck = new Color(1f, 0.1f, 0.1f, 0.25f);
        private void OnDrawGizmos()
        {
            if (controller == null) return;
            Gizmos.color = _groundCheck;
            Gizmos.DrawSphere(_spherePos, controller.radius - 0.05f);
        }

        public void StartUnarmedCombat()
        {
            // Should put away the weapons, change stance 
        }
        
        public void StartSwordAndShieldCombat()
        {
            // Should take out the weapons, change stance 
        }
        
    }
}
