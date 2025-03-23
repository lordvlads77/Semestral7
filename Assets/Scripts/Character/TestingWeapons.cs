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
            if (_input == null) _input = gameObject.GetComponent<Input.Actions>();
            if (_input == null) _input = gameObject.AddComponent<Input.Actions>();
        }

        private void Cosa(LivingEntity entity)
        {
            if (entity.Weapon == WeaponType.Unarmed) return;
        }

        private void OnEnable()
        {
            _input.OnAttackTriggeredEvent += WeaponHeavy;
        }

        private void WeaponHeavy()
        {
            WeaponSystem.Instance.WithdrawTwoHandedWeapon();
        }

        private void OnDisable()
        {
            _input.OnAttackTriggeredEvent -= WeaponHeavy;
        }

        private void OnDestroy()
        {
            _input.OnAttackTriggeredEvent -= WeaponHeavy;
        }
        
    }
}
