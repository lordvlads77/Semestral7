using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

[CreateAssetMenu(fileName = "Level Data", menuName = "Scriptables/Level Data")]
public class LevelData : ScriptableObject 
{
    [SerializeField] private LivingEntity player;
    [SerializeField] public LivingEntity[] otherLevelEntities;
}
