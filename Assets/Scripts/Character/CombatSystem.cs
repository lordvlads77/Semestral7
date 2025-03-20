using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Input;
using Character;
using Scriptables;
using UnityEngine.Serialization;

public class CombatSystem : Singleton<CombatSystem>
{
    [FormerlySerializedAs("_EnemyObject")]
    [Header("Weapon Object Ref")]
    [SerializeField] private GameObject _WeaponObject = default;
    [FormerlySerializedAs("_hitdir")]
    [Header("Hit Point Var")]
    [SerializeField] private Vector3 _hitPoint = default;
    [SerializeField] private int _dmgDealt = default;
    [SerializeField] private LivingEntity _player = default;
    [SerializeField] private WeaponStats _weaponStats;

    protected override void OnAwake()
    {
        if (_player == null)
        {
            _player = GameObject.FindWithTag("Player").GetComponent<LivingEntity>();
        }
    }

    private void Start()
    {
        _hitPoint = Vector3.forward;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (_WeaponObject.CompareTag("Enemy"))
        {
            LivingEntity _enemy = other.GetComponent<LivingEntity>();
            if (_enemy == null)
            {
                EDebug.LogError("Errooor");
                return;
            }
            CombatUtils.Attack(_player, other.GetComponent<LivingEntity>(), _weaponStats  );
        }
    }
    
    //public override  TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
}
