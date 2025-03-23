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
        Chatting,
        GameOver,
        Idle,
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
    
    public enum ENEMY_TYPE
    {
        None = 0,
        CHASER = 1,
        THROWER = 2,
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
        public static void Attack(LivingEntity attacker, LivingEntity target)
        {
            if (attacker == null || target == null)
            {
                EDebug.LogError("Attacker or target is null! D: ");
                return;
            }
            WeaponType weapon = attacker.Weapon;
            WeaponStatistics stats = GameManager.Instance.weaponStats.GetWeaponStats(weapon);
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

    public static class MiscUtils
    {
        public static String GetRandomName(RandomNames randomNames, NameCustomization nameCustomization)
        {
            return randomNames.GetRandomName(
                nameCustomization.isMale,
                nameCustomization.includeName,
                nameCustomization.includeLastName,
                nameCustomization.includeNickname,
                nameCustomization.includeTitle,
                nameCustomization.startsWithTitle,
                nameCustomization.replaceNameWithNickname,
                nameCustomization.lastNameThenName,
                nameCustomization.useTitleDividers
            );
        }
        
        public static GameManager CreateGameManager()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null)
            {
                GameObject newGm = new GameObject("GameManager");
                newGm.transform.position = new Vector3(0, 10 ,0);
                newGm.AddComponent<GameManager>();
                newGm.AddComponent<Input.Actions>();
                UnityEngine.Object.Instantiate(newGm);
                EDebug.Log("GameManager was not found, a new one was created.");
                gm = newGm.GetComponent<GameManager>();
            }
            return gm;
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

    [Serializable] public class NameCustomization
    {
        public bool isMale;
        public bool includeName;
        public bool includeLastName;
        public bool includeNickname;
        public bool includeTitle;
        public bool startsWithTitle;
        public bool replaceNameWithNickname;
        public bool lastNameThenName;
        public bool useTitleDividers;
    }
    
    [Serializable] public class CanvasPrefabs
    {
        [Header("Canvas Sprites")]
        public RandomSprite[] canvasSprites;
        [Header("NPC Stuffs")]
        public Canvas npcCanvas;
        public GameObject npcOption;
        // Add more as needed! 
        // (I'd like it if you added a header for each category)
    }

    [Serializable] public class CustomDialogSprites
    {
        public Sprite dialogBox;
        public Sprite dialogOption;
        public Sprite nameDivider;
    }
    
}
