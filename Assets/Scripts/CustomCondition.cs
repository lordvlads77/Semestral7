using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class CustomCondition : MonoBehaviour
{
    private TriggerOnConditions _condMan;
    [SerializeField] private string conditionID;
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
            
            if (triggersIfEntityIsKilled && entity.isDead && !ConditionMet)
            {
                OnConditionMet();
            }
        }
        LoadConditionState();
    }

    public void SetManager(TriggerOnConditions condMan)
    {
        _condMan = condMan;
    }
    
    private void OnConditionMet()
    {
        ConditionMet = true;
        _condMan.OnConditionMet(this);
        
        PlayerPrefs.SetInt($"Condition_{conditionID}_Met", 1);
        PlayerPrefs.Save();
    }
    private void LoadConditionState()
    {
        if (PlayerPrefs.GetInt($"Condition_{conditionID}_Met", 0) == 1)
        {
            ConditionMet = true;
            // Notificamos al manager que esta condición ya estaba cumplida para que se valide el trigger
            if (_condMan != null)
                _condMan.OnConditionMet(this);
        }
    }
    private void EntityHit() { if (triggersIfEntityIsHit) OnConditionMet(); }
    private void EntityKilled() { if (triggersIfEntityIsKilled) OnConditionMet(); }
    private void EntityHealed() { if (triggersIfEntityIsHealed) OnConditionMet(); }
  
}
