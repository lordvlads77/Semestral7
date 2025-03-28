using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

[Serializable]
public struct EnemyData
{

    [Range(0f, 20f)]
    [SerializeField] public float damage;

    [Range(0f, 5f)]
    [SerializeField] public float speed;

    [SerializeField] public float attackCoolDown;

    [SerializeField] public float attackRange;

    [SerializeField] public float timeInsideAttackRange;

    [SerializeField] public LivingEntity underliningEntity;

    [SerializeField] public LivingEntity player;

    public EnemyData Default()
    {
        return new EnemyData();
    }

    public EnemyData(float _damage,
        float _speed,
        float _attackCoolDown,
        float _attackRange,
        float _timeInsideAttackRange,
        LivingEntity _underliningEntity = null,
        LivingEntity _player = null)
    {
        damage = _damage;
        speed = _speed;
        attackCoolDown = _attackCoolDown;
        attackRange = _attackRange;
        timeInsideAttackRange = _timeInsideAttackRange;
        underliningEntity = _underliningEntity;
        player = _player;
    }


}
