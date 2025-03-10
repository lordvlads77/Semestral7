using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class CombatSystem : Singleton<CombatSystem>
{
    [SerializeField] private GameObject _EnemyObject = default;

    private void OnTriggerEnter(Collider other)
    {
        if (_EnemyObject.CompareTag("Enemy"))
        {
            //LivingEntity entity = other.GetComponent<LivingEntity>();
        }
    }
}
