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
        
        [Header("Animator Reference")]
        private Animator animator = default;
    
        public void Unarmed()
        {
            oneHandedWeapon.SetActive(false);
            twoHandedWeapon.SetActive(true);
        }

        public void WithdrawOneHandedWeapon()
        {
            //Add Animation Calling Here
            //Add VFX Calling Here if applicable
            //Add SFX Calling Here
            oneHandedWeapon.SetActive(true);
            if (twoHandedHand.activeInHierarchy)
            {
                twoHandedHand.SetActive(false);
                EDebug.Log("You already have a weapon equipped");
            }
        }
        
        public void SheathOneHandedWeapon()
        {
            //Add Animation Calling Here
            //Add VFX Calling Here if applicable
            //Add SFX Calling Here
            oneHandedWeapon.SetActive(false);
        }

        public void WithdrawTwoHandedWeapon()
        {
            AnimationController.Instance.TwoHandsWeaponWithdraw(animator);
            //Add VFX Calling Here if applicable
            //Add SFX Calling Here
            twoHandedWeapon.SetActive(false);
            twoHandedHand.SetActive(true);
            if (oneHandedWeapon.activeInHierarchy)
            {
                oneHandedWeapon.SetActive(false);
                EDebug.Log("You already have a weapon equipped");
            }
        }
        
        public void SheathTwoHandedWeapon()
        {
            //Add Animation Calling Here
            //Add VFX Calling Here if applicable
            //Add SFX Calling Here
            twoHandedHand.SetActive(false);
            twoHandedWeapon.SetActive(true);
        }

    }
}
