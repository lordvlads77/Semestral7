using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using JetBrains.Annotations;
using Scriptables;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;
using Random = UnityEngine.Random;

using SCG = System.Collections.Generic;

namespace Utils
{
    public enum Language // NOTE: This enum is used for localization...
    {                   // JSON files are named after the enum values and MUST be lowercase
        En,
        Es
    }

    [System.Flags]
    public enum EventConditions
    {
        None = 0,
        OnEnable = 1 << 0,
        OnAwake = 1 << 1,
        OnStart = 1 << 2,
        OnBoolFalse = 1 << 3,
        OnBoolTrue = 1 << 4,
        OnDestroy = 1 << 5,
        OnDisable = 1 << 6,
        OnGamePaused = 1 << 7,
        OnGameUnpaused = 1 << 8,
        OnTriggerEnter = 1 << 9,
        OnTriggerExit = 1 << 10,
    }

    [System.Flags]
    public enum TriggerConditions
    {
        None = 0,
        ByLayers = 1 << 0,
        ByTags = 1 << 1,
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

    public enum WindowMode
    {
        Fullscreen = 0,
        Windowed = 1,
        Borderless = 2,
        Maximized = 3,
    }

    public enum WindowResolution
    {
        R7680X4320 = 0,
        R5120X2880 = 1,
        R3840X2160 = 2,
        R3440X1440 = 3,
        R2560X1600 = 4,
        R2560X1440 = 5,
        R1920X1200 = 6,
        R1920X1080 = 7,
        R1680X1050 = 8,
        R1600X900 = 9,
        R1440X900 = 10,
        R1366X768 = 11,
        R1280X800 = 12,
        R1280X720 = 13,
        R1024X768 = 14,
        R800X600 = 15,
        R640X480 = 16,
    }

    public static class FmodUtils
    {
        private static readonly GameManager Gm = MiscUtils.GetOrCreateGameManager();
        public static float GetSingleVolume(SoundType soundType)
        {
            if (!Gm.LoadedData) SaveSystem.SaveSystem.LoadVolumePrefs();
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
            float frac = (soundType == SoundType.Master) ? 1.0f : GetSingleVolume(SoundType.Master);
            return GetSingleVolume(soundType) * frac;
        }
    }

    public static class StringUtils
    {
        public static string AddSizeTagToString(string input, int size)
        {
            string strSize = size.ToString();
            return $"<size={strSize}> {input} </size>";
        }

        public static string AddColorToString(string input, Color color)
        {
            string colorStr = ColorUtility.ToHtmlStringRGBA(color);
            return $"<color=#{colorStr}> {input} </color>";
        }
    }

    public static class Localization
    {
        private static readonly Dictionary<string, string> Translations = new Dictionary<string, string>();
        private static Language _lang = Language.En;
        private static GameManager _gm = MiscUtils.GetOrCreateGameManager();

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
            if (!Application.isPlaying || Singleton<GameManager>.applicationIsQuitting)
            {
                EDebug.LogError($"Translation attempted while GameManager is unavailable. Key: {key}");
                return key; // Fallback
            }
            if (_gm == null)
            {
                _gm = Singleton<GameManager>.TryGetInstance();
                if (_gm == null)
                {
                    EDebug.LogError("GameManager is null! Cannot translate.");
                    return key; // Fallback
                }
            }
            if (Translations.Count == 0 || _lang != _gm.CurrentLanguage)
                LoadLanguage(_gm.CurrentLanguage);
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
            if (target.isDead) return;
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
            if (target.isDead) return;
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
            if (!Application.isPlaying || Singleton<GameManager>.applicationIsQuitting)
            {
                EDebug.LogError("Attempted to create GameManager while the application is not playing or is quitting.");
                return null;
            }
            GameManager gm = GameManager.Instance;
            if (gm != null) return gm;
            GameObject newGm = new GameObject("GameManager")
            { transform = { position = new Vector3(0, 10, 0) } };
            gm = newGm.AddComponent<GameManager>();
            newGm.AddComponent<Input.Actions>();
            UnityEngine.Object.DontDestroyOnLoad(newGm);
            EDebug.Log("GameManager was not found, a new one was created.");
            return gm;
        }
        
        public static void ActionToDo([CanBeNull] Animator anim, [CanBeNull] string animTriggerName, 
            [CanBeNull] GameObject objToDoStuff, int actionType, [CanBeNull] ParticleSystem particle,
            [CanBeNull] PlayPersistent sound)
        {
            if (anim != null && !string.IsNullOrWhiteSpace(animTriggerName))
                anim.SetTrigger(animTriggerName);
            if (objToDoStuff != null) {
                switch (actionType) {
                    default:
                    case 0: objToDoStuff.SetActive(false);
                        break;
                    case 1: UnityEngine.Object.Destroy(objToDoStuff);
                        break;
                    case 2: objToDoStuff.SetActive(true);
                        break;
                }
            }
            if (particle != null) {
                UnityEngine.Object.Instantiate(particle);
                particle.Play();
            }
            if (sound != null) sound.enabled = true;
        }
    }

    [Serializable]
    public class DialogOption
    {
        public string npcDialog;
        public List<ResponseOption> userResponses;
    }

    [Serializable]
    public class ResponseOption
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

    [Serializable]
    public class WeaponStatistics
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

    [Serializable]
    public class HurtFXVars
    {
        public int blinks = 1;
        public SkinnedMeshRenderer renderer;
        public Material[] ogMaterials;
        public Material[] hitMaterials;
        public float animTime = 0.1f;
        public Coroutine DamageFXCoroutine;
    }

    [Serializable]
    public class NameCustomization
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

    [Serializable]
    public class CanvasPrefabs
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

    [Serializable]
    public class CustomDialogSprites
    {
        public Sprite dialogBox;
        public Sprite dialogOption;
        public Sprite nameDivider;
    }

    [Serializable]
    public class EventSoundType
    {
        public EventInstance EventI;
        public SoundType soundType;
    }

    /// <summary>
    /// Uso esta clase para pasar un referencia a una coroutina
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Ref<T>
    {
        public T backing;
        public T Value { get { return backing; } }
        public Ref(T reference)
        {
            backing = reference;
        }

        public Ref(ref T reference)
        {
            backing = reference;
        }
    }

    public static class WindowEnumUtils
    {

        public static readonly SCG.Dictionary<WindowResolution, string> winToStr = MakeWinToStr();
        public static readonly SCG.Dictionary<string, WindowResolution> strToWin = MakeStrToWin();

        private static SCG.Dictionary<WindowResolution, string> MakeWinToStr()
        {
            SCG.Dictionary<WindowResolution, string> result = new();
            result.Add(WindowResolution.R640X480, "640X480");
            result.Add(WindowResolution.R800X600, "800X600");
            result.Add(WindowResolution.R1024X768, "1024X768");
            result.Add(WindowResolution.R1280X720, "1280X720");
            result.Add(WindowResolution.R1366X768, "1366X768");
            result.Add(WindowResolution.R1440X900, "1440X900");
            result.Add(WindowResolution.R1600X900, "1600X900");
            result.Add(WindowResolution.R1680X1050, "1680X1050");
            result.Add(WindowResolution.R1920X1080, "1920X1080");
            result.Add(WindowResolution.R1920X1200, "1920X1200");
            result.Add(WindowResolution.R2560X1440, "2560X1440");
            result.Add(WindowResolution.R2560X1600, "2560X1600");
            result.Add(WindowResolution.R3440X1440, "3440X1440");
            result.Add(WindowResolution.R3840X2160, "3840X2160");
            result.Add(WindowResolution.R5120X2880, "5120X2880");
            result.Add(WindowResolution.R7680X4320, "7680X4320");


            return result;
        }

        private static Dictionary<string, WindowResolution> MakeStrToWin()
        {
            SCG.Dictionary<string, WindowResolution> result = new();
            result.Add("640X480", WindowResolution.R640X480);
            result.Add("800X600", WindowResolution.R800X600);
            result.Add("1024X768", WindowResolution.R1024X768);
            result.Add("1280X720", WindowResolution.R1280X720);
            result.Add("1366X768", WindowResolution.R1366X768);
            result.Add("1440X900", WindowResolution.R1440X900);
            result.Add("1600X900", WindowResolution.R1600X900);
            result.Add("1680X1050", WindowResolution.R1680X1050);
            result.Add("1920X1080", WindowResolution.R1920X1080);
            result.Add("1920X1200", WindowResolution.R1920X1200);
            result.Add("2560X1440", WindowResolution.R2560X1440);
            result.Add("2560X1600", WindowResolution.R2560X1600);
            result.Add("3440X1440", WindowResolution.R3440X1440);
            result.Add("3840X2160", WindowResolution.R3840X2160);
            result.Add("5120X2880", WindowResolution.R5120X2880);
            result.Add("7680X4320", WindowResolution.R7680X4320);


            return result;
        }

    }

}
