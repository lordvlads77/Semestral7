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

    [SerializeField] public Utils.ENEMY_STATE enemy_state;

    public EnemyData Default()
    {
        return new EnemyData();
    }

    public EnemyData(float _damage,
        float _speed,
        float _attackCoolDown,
        float _attackRange,
        float _timeInsideAttackRange,
        Utils.ENEMY_STATE _enemy_state,
        LivingEntity _underliningEntity = null,
        LivingEntity _player = null)
    {
        damage = _damage;
        speed = _speed;
        attackCoolDown = _attackCoolDown;
        attackRange = _attackRange;
        timeInsideAttackRange = _timeInsideAttackRange;
        enemy_state = _enemy_state;
        underliningEntity = _underliningEntity;
        player = _player;
    }


}
