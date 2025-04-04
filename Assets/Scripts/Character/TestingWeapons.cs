using System;
using Controllers;
using Input;
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
            if (_input == null) _input = gameObject.GetComponent<Input.Actions>();
            if (_input == null) _input = gameObject.AddComponent<Input.Actions>();
        }

        private void Cosa(LivingEntity entity)
        {
            if (entity.Weapon == WeaponType.Unarmed) return;
        }

        private void OnEnable()
        {
            _input.OnWeaponUpToggledEvent += WithdrawWeaponHeavy;
            _input.OnWeaponLeftToggledEvent += WithdrawWeaponLight;
            _input.OnWeaponRightToggledEvent += SheathWeaponLight;
            _input.OnWeaponDownToggledEvent += SheathWeaponHeavy;
            _input.OnAttackHeavySwing += TwoHandsiesWeaponSwing;
        }

        private void WeaponHeavy()
        {
            WeaponSystem.Instance.WithdrawTwoHandedWeapon();
        }

        private void SheathWeaponLight()
        {
            WeaponSystem.Instance.SheathOneHandedWeapon();
        }

        private void SheathWeaponHeavy()
        {
            WeaponSystem.Instance.SheathTwoHandedWeapon();
        }

        public void TwoHandsiesWeaponSwing()
        {
            WeaponSystem.Instance.TwoWeaponSwing();
        }

        private void OnDisable()
        {
            _input.OnWeaponUpToggledEvent -= WithdrawWeaponHeavy;
            _input.OnWeaponLeftToggledEvent -= WithdrawWeaponLight;
            _input.OnWeaponRightToggledEvent -= SheathWeaponLight;
            _input.OnWeaponDownToggledEvent -= SheathWeaponHeavy;
            _input.OnAttackHeavySwing -= TwoHandsiesWeaponSwing;
        }

        private void OnDestroy()
        {
            _input.OnWeaponUpToggledEvent -= WithdrawWeaponHeavy;
            _input.OnWeaponLeftToggledEvent -= WithdrawWeaponLight;
            _input.OnWeaponRightToggledEvent -= SheathWeaponLight;
            _input.OnWeaponDownToggledEvent -= SheathWeaponHeavy;
            _input.OnAttackHeavySwing -= TwoHandsiesWeaponSwing;
        }
        
    }
}
