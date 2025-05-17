using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class CustomCondition : MonoBehaviour
{
    private TriggerOnConditions _condMan;
    public bool ConditionMet { get; private set; }
    
    [Header("Conditions for LivingEntities")]
    [SerializeField] private LivingEntity entity;
    [SerializeField] private bool triggersIfEntityIsHit;
    [SerializeField] private bool triggersIfEntityIsHealed;
    [SerializeField] private bool triggersIfEntityIsKilled;


    private void OnEnable()
    {
        if (entity != null)
        {
            entity.OnHit += EntityHit;
            entity.OnKilled += EntityKilled;
            entity.OnHeal += EntityHealed;
        }
    }

    public void SetManager(TriggerOnConditions condMan)
    {
        _condMan = condMan;
    }
    
    private void OnConditionMet()
    {
        ConditionMet = true;
        _condMan.OnConditionMet(this);
    }
    
    private void EntityHit() { if (triggersIfEntityIsHit) OnConditionMet(); }
    private void EntityKilled() { if (triggersIfEntityIsKilled) OnConditionMet(); }
    private void EntityHealed() { if (triggersIfEntityIsHealed) OnConditionMet(); }
  
}
