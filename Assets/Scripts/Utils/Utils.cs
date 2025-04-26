using System;
using System.Collections.Generic;
using FMOD.Studio;
using Scriptables;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Utils
{
    public enum Language
    {
        En,
        Es
    }
    
    public enum GameStates : byte
    {
        Joining,
        Playing,
        Paused,
        Chatting,
        GameOver,
        Idle,
        Won,
    }

    public enum CameraTypes
    {
        FreeLook,
        Locked
    }

    public enum WeaponType
    {
        Unarmed,
        LightSword,
        GreatSword,
        NamePending3,
        NamePending4
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
    
    public enum EnemyType 
    {
        Melee = 0,
        Wizard = 1
    }

    public enum EnemyState
    {
        Idle, 
        Chasing,
        Attacking,
        Fleeing,
        Dying,
        Dead
    }
    
    public enum SoundType
    {
        Master,
        Music,
        SFX
    }

    /* "sfx_volume"
    "music_volume"
    "master_volume" */
    
    public static class FmodUtils
    {
        private static readonly GameManager Gm = MiscUtils.GetOrCreateGameManager();
        public static float GetSingleVolume(SoundType soundType)
        {
            if (!Gm.LoadedData) SaveSystem.SaveSystem.LoadLevelData();
            switch (soundType)
            {
                default:
                case SoundType.Master:
                    return PlayerPrefs.GetFloat("master_volume", 1.0f);
                case SoundType.Music:
                    return PlayerPrefs.GetFloat("music_volume", 1.0f);
                case SoundType.SFX:
                    return PlayerPrefs.GetFloat("sfx_volume", 1.0f);
            }
        }

        public static float GetCompositeVolume(SoundType soundType)
        {
            float frac = (soundType == SoundType.Master)? 1.0f : GetSingleVolume(SoundType.Master);
            return GetSingleVolume(soundType) * frac;
        }
    }

    public static class Localization
    {
        private static readonly Dictionary<string, string> Translations = new Dictionary<string, string>();
        private static Language _lang = Language.En;
        private static readonly GameManager Gm = MiscUtils.GetOrCreateGameManager();

        public static void LoadLanguage(Language language)
        { // This method is public, however it's already called by "Translate" so it shouldn't be needed outside... 
            try
            {
                string langFileName = $"Assets/Resources/Lang/{language.ToString().ToLower()}.json";
                TextAsset langFile = Resources.Load<TextAsset>($"Lang/{language.ToString().ToLower()}");
                if (langFile == null)
                {
                    EDebug.LogError($"Couldn't find the language file: {langFileName}");
                    return;
                }
                JSONObject json = JSONObject.Create(langFile.text);
                if (json == null || json.type != JSONObject.Type.Object || json.count == 0)
                {
                    EDebug.LogError($"This lang file is empty or has invalid translations: {langFileName}");
                    return;
                }
                Translations.Clear();
                for (int i = 0; i < json.keys.Count; i++)
                {
                    string key = json.keys[i];
                    string value = json.list[i].stringValue;
                    Translations[key] = value;
                }
                _lang = language;
                EDebug.Log($"Language loaded correctly: {language}");
            }
            catch (Exception ex)
            {
                EDebug.LogError($"Error loading lang file: {language}. Details: {ex.Message}");
            }
        }

        public static string Translate(string key)
        {
            if (Translations.Count == 0 || _lang != Gm.CurrentLanguage)
                LoadLanguage(Gm.CurrentLanguage);
            if (Translations.TryGetValue(key, out string value))
                return value;
            EDebug.LogError($"Translation not found for key: {key}");
            return key; // (Fallback)
        }

        [Serializable] private class SerializableDictionary
        {
            public List<string> keys;
            public List<string> values;

            public Dictionary<string, string> ToDictionary()
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                for (int i = 0; i < keys.Count; i++)
                {
                    dictionary[keys[i]] = values[i];
                }
                return dictionary;
            }
        }
    }

    [Serializable]
    public enum Languege : int
    {
        English = 0,
        Spanish,
        COUNT,/// NOTE : Cuando agregas un lenguage nuevo tienes que ponerlos antes que el COUNT
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

        public static Vector3 RandomPos(float radius, Vector3 origin)
        {
            Vector3 randomPos = Random.insideUnitSphere * radius;
            randomPos += origin;
            return randomPos;
        }
        public static bool RandBool() { return Random.value < 0.5f; }
        public static bool WeightedRandBool(float weight) { return Random.value <= weight; }
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

            EDebug.Log(attacker.entityName + "Attacked ► " + target.entityName);
            target.TakeDamage(
                attacker.transform.position, //Change this later to the actual point of impact for particles
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
        
        public static void Attack(Transform attackFrom, WeaponStatistics stats, LivingEntity target)
        {
            if (stats == null)
            {
                EDebug.LogError($"WeaponStats not found");
                return;
            }
            if (target == null || attackFrom == null)
            {
                EDebug.LogError("Target or attacker transform is null");
                return;
            }
            target.TakeDamage(
                target.transform.position,
                (target.transform.position - attackFrom.position).normalized,
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

        public static void ProjectileDamage(LivingEntity target, Vector3 hitPoint, Vector3 hitDirection, Projectile projectile)
        {
            DamageType resistance = target.GetDmgTypeResistance();
            target.TakeDamage(hitPoint,
                hitDirection,
                projectile.getDamageType,
                resistance,
                projectile.damage,
                0.0f,
                0.0f,
                0.0f,
                0.1f,
                2.0f);

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

        public static GameManager GetOrCreateGameManager()
        {
            GameManager gm = GameManager.Instance;
            if (gm != null) return gm; 
            GameObject newGm = new GameObject("GameManager")
            { transform = { position = new Vector3(0, 10 ,0) } };
            gm = newGm.AddComponent<GameManager>();
            newGm.AddComponent<Input.Actions>();
            UnityEngine.Object.DontDestroyOnLoad(newGm);
            EDebug.Log("GameManager was not found, a new one was created.");
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
        [Range(0, 1)] public float armorPenetration;  //Percentage of armor ignored
        [Range(0, 1)] public float critRate;          //Chance of landing a critical hit
        [Range(1, 5)] public float critDamage;        //Multiplier for critical hits
    }
    
    [Serializable] public class HurtFXVars
    {
        public int blinks = 1;
        public SkinnedMeshRenderer renderer;
        public Material[] ogMaterials;
        public Material[] hitMaterials;
        public float animTime = 0.1f;
        public Coroutine DamageFXCoroutine;
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
        public GameObject dialogPrompt;
        public GameObject promptName;
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
