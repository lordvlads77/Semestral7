using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Input;
using Character;
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
    

    private void Start()
    {
        _hitPoint = Vector3.forward;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (_WeaponObject.CompareTag("Enemy"))
        {
            //LivingEntity.TakeDamage(_dmgDealt, _hitPoint, Vector3.forward);
        }
    }
    
    //public override  TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
}
