using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOnConditions : MonoBehaviour
{
    [SerializeField] private CustomCondition[] conditions;

    private void Awake()
    {
        foreach (var cond in conditions) {
            cond.SetManager(this);
        }
    }
    
    public void OnConditionMet(CustomCondition condition)
    {
        Debug.Log($"Condition met for: {condition.name}");
        ValidateConditions();
    }
    
    private void ValidateConditions()
    {
        int metCount = 0;
        foreach (var cond in conditions) {
            if (cond.ConditionMet)
                metCount++;
        }
        if (metCount == conditions.Length) {
            Debug.Log("All conditions met!! :O");
            // You can do stuff here
        }
    }
}
