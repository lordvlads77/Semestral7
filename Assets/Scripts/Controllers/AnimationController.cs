using UnityEngine;
using Utils;

namespace Controllers
{
    public class AnimationController : Singleton<AnimationController>
    {
        private readonly int _2HWeaponWithdraw = Animator.StringToHash("2HWeaponWithdraw");
        private readonly int _1HWeaponWithdraw = Animator.StringToHash("1HWeaponWithdraw");
    
        public void TwoHandsWeaponWithdraw(Animator animator)
        {
            animator.SetBool(_2HWeaponWithdraw, true);
        }

        public void OneHandWeaponWithdraw(Animator animator)
        {
            animator.SetBool(_1HWeaponWithdraw, true);
        }
    }
}
