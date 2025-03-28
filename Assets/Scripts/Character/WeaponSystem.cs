using Controllers;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Character
{
    public class WeaponSystem : Singleton<WeaponSystem>
    {
        [FormerlySerializedAs("OneHandedWeapon")]
        [FormerlySerializedAs("sword")]
        [Header("Sword GameObject Variable")] 
        [FormerlySerializedAs("_sword")] [SerializeField]
        private GameObject oneHandedWeapon = default;
        [FormerlySerializedAs("TwoHandedWeapon")]
        [FormerlySerializedAs("halberd")]
        [Header("Halberd GameObject Variable")]
        [FormerlySerializedAs("polearm")] [FormerlySerializedAs("_polearm")] [SerializeField]
        private GameObject twoHandedWeapon = default;
        [Header("Two Handed Weapon in Hands")]
        [SerializeField] private GameObject twoHandedHand = default;
        [SerializeField] private bool _isWeaponInUse = false;

        [Header("Animator Reference")]
        [SerializeField] private Animator animator = default;
    
        public void Unarmed()
        {
            oneHandedWeapon.SetActive(false);
            twoHandedWeapon.SetActive(true);
        }

        public void WithdrawOneHandedWeapon()
        {
            if (_isWeaponInUse == false)
            {
                _isWeaponInUse = true;
                AnimationController.Instance.OneHandWeaponWithdraw(animator);
                //Add VFX Calling Here if applicable
                //Add SFX Calling Here
                oneHandedWeapon.SetActive(true);
            }
            else
            {
                EDebug.Log("You already have a weapon equipped");
            }
            if (twoHandedHand.activeInHierarchy)
            {
                SheathTwoHandedWeapon();
                WithdrawOneHandedWeapon();
                EDebug.Log("Switching to one handed weapon");
            }
        }
        
        public void SheathOneHandedWeapon()
        {
            _isWeaponInUse = true;
            if (_isWeaponInUse)
            {
                _isWeaponInUse = false;
                AnimationController.Instance.NotCombatWalk1H(animator);
                //Add VFX Calling Here if applicable
                //Add SFX Calling Here
                if (!oneHandedWeapon.activeInHierarchy)
                {
                    EDebug.Log("You already Sheathed your weapon");
                }
                else
                {
                    AnimationController.Instance.OneHandWeaponSheath(animator);
                    oneHandedWeapon.SetActive(false);
                }
            }
            else
            {
                EDebug.Log("You already sheathed your weapon");
            }
        }

        public void WithdrawTwoHandedWeapon()
        {
            if (_isWeaponInUse == false)
            {
                _isWeaponInUse = true;
                AnimationController.Instance.TwoHandsWeaponWithdraw(animator);
                //Add VFX Calling Here if applicable
                //Add SFX Calling Here
                twoHandedWeapon.SetActive(false);
                twoHandedHand.SetActive(true);
            }
            else
            {
                EDebug.Log("You already have a weapon equipped");
            }
            if (oneHandedWeapon.activeInHierarchy)
            {
                SheathOneHandedWeapon();
                WithdrawTwoHandedWeapon();
                EDebug.Log("Switching to two handed weapon");
            }
        }
        
        public void SheathTwoHandedWeapon()
        {
            if (_isWeaponInUse)
            {
                _isWeaponInUse = false;
                AnimationController.Instance.TwoHandsWeaponSheath(animator);
                //Add VFX Calling Here if applicable
                //Add SFX Calling Here
                twoHandedHand.SetActive(false);
                twoHandedWeapon.SetActive(true);
            }
            else
            {
                EDebug.Log("You already sheathed your weapon");
            }
        }

    }
}
