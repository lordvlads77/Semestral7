using System;
using System.Collections;
using Controllers;
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
        private static readonly int DogeAnim = Animator.StringToHash("Doge");
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
        [SerializeField, Range(5f, 15f)] private float gravity = 9.81f;
        [SerializeField] private float jumpHeight = 2f;
        [HideInInspector] public Vector3 dir;
        [HideInInspector] public float horizontalInput, verticalInput;
    
        [SerializeField] public CharacterController controller;
        private Vector3 _spherePos;
        private Vector3 _velocity;
    
        [SerializeField] private StateManager stateManager;
        public Animator anim;
        
        private Camera _cam;
        private ThirdPersonCamera _cm;
        private Coroutine _attackRoutine = null;
        [SerializeField] private Weapon [] weapon;
        
        [Header("Dodge Settings")]
        [SerializeField] private float dodgeSpeed = 10f; // Fuerza de impulso
        [SerializeField] private float dodgeDuration = 0.5f; // Duración de la esquiva
        [SerializeField] private float dodgeCooldown = 1f; // Tiempo de espera entre esquivas

        private CharacterController characterController;
        private Vector3 dodgeDirection;
        private bool isDodging = false;
        private bool canDodge = true;
        private Rigidbody rb;
        
        protected override void OnAwoken()
        {
            anim = GetComponent<Animator>();
            controller = GetComponent<CharacterController>();
            stateManager.EnterMovementState(MovementState.Walk, this);
            _cam = Camera.main;
            IInput = (Input.Actions.Instance != null)? Input.Actions.Instance : MiscUtils.GetOrCreateGameManager().gameObject.GetComponent<Input.Actions>();
            if (_cam) _cm = _cam.GetComponent<ThirdPersonCamera>();
            if (!_cm) _cm = GetComponent<ThirdPersonCamera>(); // You had the script here, right
        }
        
        private void OnEnable()
        {
            IInput.OnCrouchToggledEvent += ToggleCrouch;
            IInput.OnAttackTriggeredEvent += Punch;
            GameManager.Instance.RegisterUnsubscribeAction(UnSubscribe);
        }
        
        private void UnSubscribe()
        {
            IInput.OnCrouchToggledEvent -= ToggleCrouch;
            GameManager.TryGetInstance()?.Unsubscribe(OnStateChange);
        }
        
        private void Update()
        {
            if(gameState != GameStates.Playing) { return; }
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
        private IEnumerator Dodge()
        {
            isDodging = true;
            canDodge = false;
            canTakeDamage = false;

            Vector3 inputDir = new Vector3(IInput.Movement.x, 0f, IInput.Movement.y);

            if (inputDir.sqrMagnitude > 0.1f)
            {
                Vector3 camForward = _cam.transform.forward;
                Vector3 camRight = _cam.transform.right;
                camForward.y = 0;
                camRight.y = 0;
                camForward.Normalize();
                camRight.Normalize();

                dodgeDirection = (camForward * inputDir.z + camRight * inputDir.x).normalized;

                // Determinar dirección para animación (adelante o atrás)
                float dot = Vector3.Dot(dodgeDirection, transform.forward);
                int animDir = dot > 0 ? 1 : -1;
                anim.SetInteger("DodgeDirection", animDir);
            }
            else
            {
                dodgeDirection = transform.forward;
                anim.SetInteger("DodgeDirection", 1); // Default a adelante si no hay input
            }

            anim.SetTrigger(DogeAnim);

            _velocity = dodgeDirection * dodgeSpeed;
            float elapsedTime = 0f;
            while (elapsedTime < dodgeDuration)
            {
                controller.Move(_velocity * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            isDodging = false;
            canTakeDamage = true;

            float decelerationTime = 0.2f;
            float elapsedDecel = 0f;
            while (elapsedDecel < decelerationTime)
            {
                _velocity = Vector3.Lerp(_velocity, Vector3.zero, elapsedDecel / decelerationTime);
                controller.Move(_velocity * Time.deltaTime);
                elapsedDecel += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(dodgeCooldown);
            canDodge = true;
        }
        public void SwitchMovementState(MovementState state)
        {
            stateManager.EnterMovementState(state, this);
        }
        
        private void HandleActions() // This method will be refactored later (Inputs n shit)
        {
            if (!isDodging)
            {
                if (IInput.Jump && IsGrounded())
                {
                    Jump();
                }
            }
            if (IInput.Doge && canDodge && !isDodging)
            {
                StartCoroutine(Dodge());
            }
        }
        
private void GetDirectionAndMove()
{
    horizontalInput = IInput.Movement.x;
    verticalInput = IInput.Movement.y;
    anim.SetFloat(AnimVInput, verticalInput);
    anim.SetFloat(AnimHInput, horizontalInput);

    bool isZLock = _cm != null && _cm.type == CameraTypes.Locked && _cm.lockTarget != null;

    if (isZLock)
    {
        // Si estamos en Z-Lock, mover hacia el objetivo
        Vector3 toTarget = (_cm.lockTarget.position - transform.position);
        toTarget.y = 0; // Mantener solo el movimiento horizontal
        Vector3 forward = toTarget.normalized;
        Vector3 right = Vector3.Cross(Vector3.up, forward);

        // Movimiento orbital y radial
        dir = (right * horizontalInput + forward * verticalInput).normalized;

        // Girar el jugador hacia el objetivo
        if (toTarget.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(forward),
                Time.deltaTime * 10f
            );
        }
    }
    else
    {
        // Si no estamos en Z-Lock, movimiento normal basado en la cámara
        Vector3[] camVec = MathUtils.CanonBasis(_cam.transform);
        dir = (camVec[0] * verticalInput + camVec[1] * horizontalInput).normalized;

        // Girar el jugador hacia la dirección del movimiento
        if (dir.magnitude > 0)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(camVec[0]),
                Time.deltaTime * 10f
            );
        }
    }

    // Seleccionar la velocidad apropiada
    Vector3 speeds = walkSpeeds;
    if (IInput.RightBumper)
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

    // Mover el personaje
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
                return;
            }
            if (_attackRoutine == null) _attackRoutine = StartCoroutine(PerformAttack());
        }
        private IEnumerator PerformAttack()
        {
            Animator.SetTrigger(AnimAttack);
            int num = (int) Weapon;
            if (num < 0 || num >= weapon.Length) {
                EDebug.LogError($"Invalid Weapon Index: {num}. Make sure it's within the limits of the array.");
                yield break;
            }
            Collider weaponCollider = weapon[num].GetComponent<Collider>();
            weapon[num].inUse = true;
            if (weaponCollider != null) weaponCollider.enabled = true;
            bool damageApplied = false;
            controller.enabled = false;
            EDebug.Log("MoveController Disabled");
            while (Animator.GetCurrentAnimatorStateInfo(0).IsName("UnarmedCombat_Patadon") || 
                   Animator.GetCurrentAnimatorStateInfo(0).IsName("1HStandingMeleeAttackDownguard") ||
                   Animator.GetCurrentAnimatorStateInfo(0).IsName("2HWeaponSwing"))
            { if (!damageApplied && Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f) {
                    damageApplied = true;
                    weapon[num].inUse = false;
                }
                yield return null;
            }
            weapon[num].inUse = false;
            if (weaponCollider != null) weaponCollider.enabled = false;
            yield return new WaitForSeconds(1.25f);
            controller.enabled = true;
            _attackRoutine = null;
        }
        public void IncreaseMaxHealth(float amount)
        {
            maxHealth += amount;
        }

        private bool IsGrounded()
        {
            Vector3 vec = this.transform.position;
            _spherePos = new Vector3(vec.x, vec.y - groundYOffset, vec.z);
            return Physics.CheckSphere(_spherePos, controller.radius - 0.05f, groundLayers);
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
        
        protected override void OnStateChange(GameStates state)
        {
            gameState = state;
            anim.enabled = (state == GameStates.Playing);
        }
        
    }
}
