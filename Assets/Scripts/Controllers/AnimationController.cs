using UnityEngine;
using Utils;

namespace Controllers
{
    public class AnimationController : Singleton<AnimationController>
    {
        private readonly int _2HWeaponWithdraw = Animator.StringToHash("2HWeaponWithdraw");
        private readonly int _1HWeaponWithdraw = Animator.StringToHash("1HWeaponWithdraw");
        private readonly int _1HWeaponSheath = Animator.StringToHash("1HWeaponSheath");
        //[System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void TwoHandsWeaponWithdraw(Animator animator)
        {
            if (animator == null)
            {
                EDebug.Log("<color=red> animator is not set</color>");
                return;
            }
            animator.SetTrigger(_2HWeaponWithdraw);
        }

        public void OneHandWeaponWithdraw(Animator animator)
        {
            if (animator == null)
            {
                EDebug.Log("<color=red> animator is not set</color>");
                return;
            }
            animator.SetTrigger(_1HWeaponWithdraw);
        }

        public void OneHandWeaponSheath(Animator animator)
        {
            if (animator == null)
            {
                EDebug.Log("<color=red> animator is not set</color>");
                return;
            }
            animator.SetTrigger(_1HWeaponSheath);
        }
    }
}
