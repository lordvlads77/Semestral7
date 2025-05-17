using System;
using Controllers;
using UnityEngine;
using Utils;

namespace Character
{
    public class TestingWeapons : MonoBehaviour
    {
        private Input.Actions _input;
        
        private void Awake()
        {
            _input = Input.Actions.Instance;
        }

        private void OnEnable()
        {
            _input.OnWeaponUpToggledEvent += WithdrawWeaponHeavy;
            _input.OnWeaponLeftToggledEvent += WithdrawWeaponLight;
            _input.OnWeaponRightToggledEvent += SheathWeaponLight;
            _input.OnWeaponDownToggledEvent += SheathWeaponHeavy;
            _input.ShareButton += LowerHands;
        }

        private void WithdrawWeaponHeavy()
        {
            WeaponSystem.Instance.WithdrawTwoHandedWeapon();
        }
        
        private void WithdrawWeaponLight()
        {
            WeaponSystem.Instance.WithdrawOneHandedWeapon();
        }

        private void SheathWeaponLight()
        {
            WeaponSystem.Instance.SheathOneHandedWeapon();
        }

        private void SheathWeaponHeavy()
        {
            WeaponSystem.Instance.SheathTwoHandedWeapon();
        }

        private void LowerHands()
        {
            WeaponSystem.Instance.LoweringHands();
        }

        private void OnDisable()
        {
            _input.OnWeaponUpToggledEvent -= WithdrawWeaponHeavy;
            _input.OnWeaponLeftToggledEvent -= WithdrawWeaponLight;
            _input.OnWeaponRightToggledEvent -= SheathWeaponLight;
            _input.OnWeaponDownToggledEvent -= SheathWeaponHeavy;
            _input.ShareButton -= LowerHands;
        }

        private void OnDestroy()
        {
            _input.OnWeaponUpToggledEvent -= WithdrawWeaponHeavy;
            _input.OnWeaponLeftToggledEvent -= WithdrawWeaponLight;
            _input.OnWeaponRightToggledEvent -= SheathWeaponLight;
            _input.OnWeaponDownToggledEvent -= SheathWeaponHeavy;
            _input.ShareButton -= LowerHands;
        }
        
    }
}
