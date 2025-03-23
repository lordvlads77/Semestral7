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
        }

        private void WithdrawWeaponHeavy()
        {
            WeaponSystem.Instance.WithdrawTwoHandedWeapon();
        }
        
        private void WithdrawWeaponLight()
        {
            WeaponSystem.Instance.WithdrawOneHandedWeapon();
        }

        private void OnDisable()
        {
            _input.OnAttackTriggeredEvent -= WithdrawWeaponHeavy;
            _input.OnWeaponLeftToggledEvent -= WithdrawWeaponLight;
        }

        private void OnDestroy()
        {
            _input.OnAttackTriggeredEvent -= WithdrawWeaponHeavy;
            _input.OnWeaponLeftToggledEvent -= WithdrawWeaponLight;
        }
        /*private void Update() Got rid of this for now... 
        {                       I'm moving the weapon types so that they're compatible with all Entities
            switch (_input.CurrentWeapon)
            {
                default:
                case Input.WeaponType.Unarmed:
                    WeaponSystem.Instance.Unarmed();
                    break;
                case Input.WeaponType.NamePending1:
                    WeaponSystem.Instance.WithdrawOneHandedWeapon();
                    break;
                case Input.WeaponType.NamePending2:
                    WeaponSystem.Instance.WithdrawTwoHandedWeapon();
                    break;
            }
        }*/
    }
}
