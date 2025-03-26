using System;
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
        }

        private void GreatSwordUse(LivingEntity entity)
        {
            if (entity.Weapon == WeaponType.GreatSword) return;
        }

        private void LightSwordUse(LivingEntity entity)
        {
            if (entity.Weapon == WeaponType.LightSword)
            {
                return;
            }
        }

        private void OnEnable()
        {
            _input.OnAttackTriggeredEvent += WithdrawWeaponHeavy;
            _input.OnWeaponLeftToggledEvent += WithdrawWeaponLight;
            _input.OnWeaponRightToggledEvent += SheathWeaponLight;
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

        private void OnDisable()
        {
            _input.OnAttackTriggeredEvent -= WithdrawWeaponHeavy;
            _input.OnWeaponLeftToggledEvent -= WithdrawWeaponLight;
            _input.OnWeaponRightToggledEvent -= SheathWeaponLight;
        }

        private void OnDestroy()
        {
            _input.OnAttackTriggeredEvent -= WithdrawWeaponHeavy;
            _input.OnWeaponLeftToggledEvent -= WithdrawWeaponLight;
            _input.OnWeaponRightToggledEvent -= SheathWeaponLight;
        }
        
    }
}
