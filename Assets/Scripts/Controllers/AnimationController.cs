using Character;
using UnityEngine;
using Utils;

namespace Controllers
{
    public class AnimationController : Singleton<AnimationController>
    {
        private readonly int _2HWeaponWithdraw = Animator.StringToHash("2HWeaponWithdraw");
        private readonly int _1HWeaponWithdraw = Animator.StringToHash("1HWeaponWithdraw");
        private readonly int _1HWeaponSheath = Animator.StringToHash("1HWeaponSheath");
        private readonly int _2HWeaponSheath = Animator.StringToHash("2HWeaponSheath");
        private readonly int _1HAttackSwing = Animator.StringToHash("1HAttackSwing");
        private readonly int _2HAttackSwing = Animator.StringToHash("2HAttackSwing");
        [SerializeField] private StateManager stateManager = default;
        private readonly int _WeaponType = Animator.StringToHash("WeaponType");
        private readonly int _HandsInUse = Animator.StringToHash("HandsInUse");
        private readonly int _UsingHands = Animator.StringToHash("UsingHands");

        
        public void TwoHandsWeaponWithdraw(Animator animator)
        {
            animator.SetTrigger(_2HWeaponWithdraw);
            stateManager.EnterFightingState(FightingState.TwoHandedFighting, GetComponent<MovementManager>());
        }

        public void OneHandWeaponWithdraw(Animator animator)
        {
            animator.SetTrigger(_1HWeaponWithdraw);
            stateManager.EnterFightingState(FightingState.OneHandedFighting, GetComponent<MovementManager>());
        }
        
        public void OneHandWeaponSheath(Animator animator)
        {
            animator.SetTrigger(_1HWeaponSheath);
            stateManager.EnterFightingState(FightingState.NonCombat, GetComponent<MovementManager>());
        }
        
        public void TwoHandsWeaponSheath(Animator animator)
        {
            animator.SetTrigger(_2HWeaponSheath);
            stateManager.EnterFightingState(FightingState.NonCombat, GetComponent<MovementManager>());
        }

        public void OneHandAttackSwing(Animator animator)
        {
            //animator.SetTrigger(_1HAttackSwing);
            WeaponSystem.Instance.Attack();
        }
        
        public void TwoHandAttackSwing(Animator animator)
        {
            stateManager.EnterFightingState(FightingState.TwoHandedSwing, GetComponent<MovementManager>());
            WeaponSystem.Instance.Attack();
        }

        
        public void WeaponType(Animator animator, int weaponType)
        {
            animator.SetInteger(_WeaponType, weaponType);
        }

        public void LowerHands(Animator animator)
        {
            animator.SetBool(_HandsInUse, false);
        }

        public void UsingHands(Animator animator)
        {
            animator.SetBool(_HandsInUse, true);
        }

        public void HandsUsing(Animator animator)
        {
            animator.SetBool(_HandsInUse, true);
        }
        
        
    }
}
