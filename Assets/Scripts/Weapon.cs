using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Weapon : MonoBehaviour
{
    private LivingEntity _lEntity;
    private string _owner;
    public bool inUse = false;

    private void Awake()
    {
        _lEntity = GetComponentInParent<LivingEntity>();
        if (_lEntity == null)
        {
            Debug.LogError("LivingEntity component not found in parent. Self destruction initialized.");
            Destroy(this);
            return;
        }
        _owner = _lEntity.entityName;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!inUse) return;
        LivingEntity livingEntity = other.GetComponent<LivingEntity>();
        if (livingEntity == null) return;
        if (livingEntity.entityName != _owner)
            CombatUtils.Attack(_lEntity, livingEntity);
    }
    
}
