using Controllers;
using UnityEngine;

namespace Character
{
    public enum MovementState
    {
        Walk,
        Crouch,
        Run,
        Sprint,
        Dead
    }
    
    public enum FightingState
    {
        NonCombat,
        UnarmedFighting,
        SwordAndShield,
        OneHandedFighting,
        TwoHandedFighting,
        TwoHandedSwing,
    }
    
    public class StateManager : MonoBehaviour
    {
        public MovementState CurrentMovementState { get; private set; }
        public FightingState CurrentFightingState { get; private set; }
        
        private Animator _animator;
        private Input.Actions _input;
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _input = Input.Actions.Instance;
            if (_input == null) _input = gameObject.GetComponent<Input.Actions>();
            if (_input == null) _input = gameObject.AddComponent<Input.Actions>();
        }
        
        private bool CheckIfDead(MovementManager movement)
        {
            if (movement.isDead)
            {
                EnterDeadState(movement);
                return true;
            }
            return false;
        }
        
        public void EnterMovementState(MovementState state, MovementManager movement)
        {
            if (CheckIfDead(movement)) return;
            if (CurrentMovementState == state) return;
            
            CurrentMovementState = state;
            _animator.SetFloat("AnimType", Random.Range(0f, 1f));
            
            switch (state)
            {
                case MovementState.Crouch:
                    EnterCrouchState(movement);
                    break;
                case MovementState.Walk:
                    EnterWalkState(movement);
                    break;
                case MovementState.Run:
                    EnterRunState(movement);
                    break;
                case MovementState.Sprint:
                    EnterSprintState(movement);
                    break;
                case MovementState.Dead:
                    EnterDeadState(movement);
                    break;
            }
        }
    
        public void UpdateMovementState(MovementManager movement)
        {
            if (CheckIfDead(movement)) return;
            
            switch (CurrentMovementState)
            {
                case MovementState.Crouch:
                    UpdateCrouchState(movement);
                    break;
                case MovementState.Walk:
                    UpdateWalkState(movement);
                    break;
                case MovementState.Run:
                    UpdateRunState(movement);
                    break;
                case MovementState.Sprint:
                    UpdateSprintState(movement);
                    break;
            }
        }
        
        private void EnterCrouchState(MovementManager movement)
        {
            _animator.SetBool("Crouching", true);
        }
        
        private void UpdateCrouchState(MovementManager movement)
        {
            if (_input.RightBumper) ExitMovementState(movement, MovementState.Run);
            //if (_input.RightBumper) ExitMovementState(movement, MovementState.Walk);
        }

        private void EnterWalkState(MovementManager movement)
        {
            _animator.SetBool("Walking", true);
        }

        private void UpdateWalkState(MovementManager movement)
        {
            if ((_input.LeftBumper) && (movement.horizontalInput != 0 || movement.verticalInput != 0))
                ExitMovementState(movement, MovementState.Run);
            if (_input.RightBumper) ExitMovementState(movement, MovementState.Run);
        }

        private void EnterRunState(MovementManager movement)
        {
            _animator.SetBool("Running", true);
        }

        private void UpdateRunState(MovementManager movement)
        {
            if (movement.dir.magnitude < 0.1f) ExitMovementState(movement, MovementState.Crouch);
            if (!_input.RightBumper) ExitMovementState(movement, MovementState.Walk);
            //if (_input.RightBumper) ExitMovementState(movement, MovementState.Crouch);
        }
        
        private void EnterSprintState(MovementManager movement)
        {
            _animator.SetBool("Sprinting", true);
            ExitFightingState(movement, FightingState.NonCombat);
        }

        private void UpdateSprintState(MovementManager movement)
        {
            //if (movement.dir.magnitude < 0.1f) ExitMovementState(movement, MovementState.Crouch);
            if (_input.LeftBumper) ExitMovementState(movement, MovementState.Walk);
            //if (_input.RightBumper) ExitMovementState(movement, MovementState.Crouch);
        }

        private void EnterDeadState(MovementManager movement)
        {
            _animator.SetBool("Dead", true);
            _animator.SetBool("Crouching", false);
            _animator.SetBool("Walking", false);
            _animator.SetBool("Running", false);
            _animator.SetBool("Sprinting", false);
            _animator.SetBool("UnarmedCombat", false);
            _animator.SetBool("SwordAndShield", false);
        }
        
        public void ExitDeadState(MovementManager movement)
        {
            if (!movement.isDead) return;
            movement.isDead = false;
            _animator.SetBool("Dead", false);
            EnterMovementState(MovementState.Walk, movement);
        }
        
        private void ExitMovementState(MovementManager movement, MovementState newState)
        {
            switch (CurrentMovementState)
            {
                case MovementState.Crouch:
                    _animator.SetBool("Crouching", false);
                    break;
                case MovementState.Walk:
                    _animator.SetBool("Walking", false);
                    break;
                case MovementState.Run:
                    _animator.SetBool("Running", false);
                    break;
                case MovementState.Sprint:
                    _animator.SetBool("Sprinting", false);
                    break;
            }
            EnterMovementState(newState, movement);
        }
        
        public void EnterFightingState(FightingState state, MovementManager movement)
        {
            if (movement.isDead) return;
            if (CurrentFightingState == state) return;

            CurrentFightingState = state;
            switch (state)
            {
                case FightingState.NonCombat:
                    _animator.SetBool("UnarmedCombat", false);
                    _animator.SetBool("SwordAndShield", false);
                    _animator.SetBool("1HSwordMov", false);
                    _animator.SetBool("2HSwordMov", false);
                    break;
                case FightingState.UnarmedFighting:
                    _animator.SetBool("UnarmedCombat", true);
                    _animator.SetBool("SwordAndShield", false);
                    _animator.SetBool("1HSwordMov", false);
                    _animator.SetBool("2HSwordMov", false);
                    break;
                case FightingState.SwordAndShield:
                    _animator.SetBool("UnarmedCombat", false);
                    _animator.SetBool("SwordAndShield", true);
                    break;
                case FightingState.OneHandedFighting:
                    _animator.SetBool("UnarmedCombat", false);
                    _animator.SetBool("1HSwordMov", true);
                    break;
                case FightingState.TwoHandedFighting:
                    _animator.SetBool("2HSwordMov", true);
                    _animator.SetBool("UnarmedCombat", false);
                    _animator.SetBool("SwordAndShield", false);
                    _animator.SetBool("1HSwordMov", false);
                    break;
                case FightingState.TwoHandedSwing:
                    AnimationController.Instance.TwoHandAttackSwing(_animator);
                    break;
            }
        }

        private void ExitFightingState(MovementManager movement, FightingState newState)
        {
            switch (CurrentFightingState)
            {
                case FightingState.UnarmedFighting:
                    _animator.SetBool("UnarmedCombat", false);
                    break;
                case FightingState.SwordAndShield:
                    _animator.SetBool("SwordAndShield", false);
                    break;
                case FightingState.OneHandedFighting:
                    _animator.SetBool("1HSwordMov", false);
                    break;
                case FightingState.TwoHandedFighting:
                    _animator.SetBool("2HSwordMov", false);
                    break;
            }
            EnterFightingState(newState, movement);
        }
        
        
    }
}
