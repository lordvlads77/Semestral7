using System;
using System.Collections.Generic;
using Scriptables;
using UnityEngine;
using UnityEngine.Events;

namespace Utils
{
    public enum GameStates : byte
    {
        Joining,
        Playing,
        Paused,
        GameOver,
    }

    public enum CameraTypes
    {
        FreeLook,
        Locked 
    }
    
    public enum WeaponType
    {
        LightSword,
        GreatSword,
        NamePending3,
        NamePending4,
        Unarmed
    }
    
    public enum DamageType
    {
        Physical,
        Fire,
        Ice,
        Poison,
        Electric,
        Dark,
        Light
    }
    
    public static class MathUtils
    {
        public static Vector3[] CanonBasis(Transform trans)
        {
            Vector3 camForward = trans.forward;
            Vector3 camRight = trans.right;
            camForward.y = 0;
            camRight.y = 0;
            return new[] { camForward.normalized, camRight.normalized };
        }
    }
    
    public static class CombatUtils
    {
        public static void Attack(LivingEntity attacker, LivingEntity target, WeaponStats weaponStats)
        {
            if (attacker == null || target == null)
            {
                EDebug.LogError("Attacker or target is null! D: ");
                return;
            }
            WeaponType weapon = attacker.Weapon;
            WeaponStatistics stats = weaponStats.GetWeaponStats(weapon);
            if (stats == null)
            {
                EDebug.LogError($"WeaponStats not found for WeaponType: {weapon}");
                return;
            }
            target.TakeDamage(
                attacker.transform.position, //Change this later to the actual point of impact
                attacker.transform.forward,
                stats.damageType,
                target.GetDmgTypeResistance(),
                stats.damage,
                stats.knockBack,
                stats.staggerBuildUp,
                stats.armorPenetration,
                stats.critRate,
                stats.critDamage
            );
        }
    }
    
    [Serializable] public class DialogOption
    {
        public string npcDialog;
        public List<ResponseOption> userResponses;
    }
    
    [Serializable] public class ResponseOption
    {
        public string response;
        public float moodChange;
        public DialogOption nextDialog;
        public UnityEvent onResponse;
        
        public virtual void OnResponse()
        {
            EDebug.Log($"Response: {response}");
            onResponse?.Invoke();
        }
    }
    
    [Serializable] public class WeaponStatistics
    {
        public DamageType damageType;                //Type of damage
        public float damage;                         //Flat damage number
        public float attackSpeed;                    //Cooldown between attacks
        public float knockBack;                      //Force applied to the target
        public float staggerBuildUp;                 //Amount of stagger applied to the target (flat number)
        [Range(0,1)] public float armorPenetration;  //Percentage of armor ignored
        [Range(0,1)] public float critRate;          //Chance of landing a critical hit
        [Range(1,5)] public float critDamage;        //Multiplier for critical hits
    }
    
}
