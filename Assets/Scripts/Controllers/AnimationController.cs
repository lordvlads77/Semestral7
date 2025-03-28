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
        private readonly int _CombatWalk1H = Animator.StringToHash("CombatWalk1H");
        private readonly int _speedCombatWalk1H  = Animator.StringToHash("speedCombatWalk");

        public void TwoHandsWeaponWithdraw(Animator animator)
        {
            animator.SetTrigger(_2HWeaponWithdraw);
        }

        public void OneHandWeaponWithdraw(Animator animator)
        {
            animator.SetTrigger(_1HWeaponWithdraw);
        }
        
        public void OneHandWeaponSheath(Animator animator)
        {
            animator.SetTrigger(_1HWeaponSheath);
        }
        
        public void TwoHandsWeaponSheath(Animator animator)
        {
            animator.SetTrigger(_2HWeaponSheath);
        }
        
        public void CombatWalk1H(Animator animator)
        {
            animator.SetBool(_CombatWalk1H, true);
        }
        
        public void NotCombatWalk1H(Animator animator)
        {
            animator.SetBool(_CombatWalk1H, false);
        }
    }
}
