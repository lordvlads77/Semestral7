using System;
using UnityEngine;
using Utils;

namespace Character
{
    public class MovementManager : LivingEntity
    {
        [Header("Movement Settings")]
        [Tooltip("The current speed, it changes based on the character state (Not hidden in inspector for debugging)")]
        public float currentMovementSpeed;
        [Tooltip("X= Forward, Y= Backward, Z= Sideways")]
        public Vector3 WalkSpeeds = new Vector3(4f, 2f, 2f);
        [Tooltip("X= Forward, Y= Backward, Z= Sideways")]
        public Vector3 RunSpeeds = new Vector3(8f, 4f, 6f);
        [Tooltip("X= Forward, Y= Backward, Z= Sideways")]
        public Vector3 SprintSpeeds = new Vector3(10f, 6f, 8f);
        [Tooltip("X= Forward, Y= Backward, Z= Sideways")]
        public Vector3 CrouchSpeeds = new Vector3(1f, 0.75f, 0.75f);
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

        #region Camera
        private Camera _cam;
        private ThirdPersonCameraMovement _cm;
        private Vector3 camFwd;
        #endregion

        private void Awake()
        {
            anim = GetComponent<Animator>();
            controller = GetComponent<CharacterController>();
            stateManager.EnterMovementState(MovementState.Walk, this);
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
            camFwd = Vector3.Scale(_cam.transform.forward, new Vector3(1,1,1)).normalized;
            Vector3 canFlatFwd = Vector3.Scale(_cam.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 flatRight = new Vector3(_cam.transform.right.x, 0, _cam.transform.right.z);
            
            Vector3 m_CharForward = Vector3.Scale(canFlatFwd, new Vector3(1,0,1)).normalized;
            Vector3 m_charRight = Vector3.Scale(flatRight, new Vector3(1, 0, 1)).normalized;

            if (_cm.type == ThirdPersonCameraMovement.CAMERA_TYPE.FREE_LOOK)
            {
                _cam.transform.position += _velocity * Time.deltaTime;
            }
        }

        public void SwitchMovementState(MovementState state)
        {
            stateManager.EnterMovementState(state, this);
        }
        
        private void HandleActions() // This method will be refactored later
        {
            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            {
                Jump();
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                ToggleCrouch();
            }

            if (Input.GetMouseButtonDown(0))
            {
                Punch();
            }
        }
        
        private void GetDirectionAndMove()
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            anim.SetFloat("VInput", verticalInput);
            anim.SetFloat("HInput", horizontalInput);
            Transform vec = this.transform;
            dir = (vec.forward * verticalInput + vec.right * horizontalInput).normalized;

            Vector3 speeds = WalkSpeeds;
            if (Input.GetKey(KeyCode.LeftShift))
                speeds = RunSpeeds;
            else if (stateManager.CurrentMovementState == MovementState.Crouch)
                speeds = CrouchSpeeds;
            else if (stateManager.CurrentMovementState == MovementState.Sprint)
                speeds = SprintSpeeds;
            
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
            
            controller.Move(dir * currentMovementSpeed * Time.deltaTime);
        }
        
        private void Jump()
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
            anim.SetTrigger("Jump");
        }
        
        private void ToggleCrouch()
        {
            if (stateManager.CurrentMovementState == MovementState.Crouch)
            {
                stateManager.EnterMovementState(MovementState.Walk, this);
            }
            else
            {
                stateManager.EnterMovementState(MovementState.Crouch, this);
            }
        }
        
        private void Punch()
        {
            anim.SetTrigger("Attack");
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
                anim.SetBool("Grounded", false);
            }
            else if (_velocity.y < 0)
            {
                _velocity.y = -2f;
            }
            anim.SetBool("Grounded", true);
            controller.Move(_velocity * Time.deltaTime);
        }
        
        private void OnDrawGizmos()
        {
            if (controller == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_spherePos, controller.radius - 0.05f);
        }

        public void StartUnarmedCombat()
        {
            
        }
        
        public void StartSwordAndShieldCombat()
        {
            
        }
        
    }
}
