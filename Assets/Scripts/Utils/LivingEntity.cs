using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Entity;
using HUD;
using Scriptables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utils
{
    public class LivingEntity : MonoBehaviour
    {
        public string entityName;
        public DialogOptions dialogOptions;
        public NameCustomization nameCustomization;
        public CustomDialogSprites dialogSprites;
        public HurtFXVars hurtFXVars;
        protected HurtFX HurtFX;
        
        

        [Header("Living Entity Variables")]
        public bool isPlayer;
        [SerializeField] protected LayerMask groundLayers;
        [SerializeField] private WeaponType weaponTypeOverride;
        [Tooltip("The particle systems that will play when the entity takes damage (Will be skipped if none are present)")]
        public ParticleSystem[] dmgParticles;
        [Tooltip("The particle systems that will play when the entity takes LETHAL damage (Will be skipped if none are present)")]
        public ParticleSystem[] criticalDmgParticles;
        [Tooltip("The entity's maximum health")]
        [SerializeField] public float maxHealth = 100f;
        [Tooltip("For how long is the entity immune to damage after taking some?")]
        [SerializeField] private float dmgImmunityTime = 0.05f;
        [SerializeField] public int armorClass = 1;
        [SerializeField] public int armorDurability = 3;
        [SerializeField] private DamageType damageTypeResistance;
        protected Animator Animator;

        private float _health;
        private float _mood;
        public bool isInDialog;
        [HideInInspector] public bool isDead;
        [HideInInspector] public bool canTakeDamage = true;
        [HideInInspector] public GameStates gameState;

        private Coroutine _damageImmunityCoroutine;
        private readonly List<ParticleSystem> _particleSystems = new List<ParticleSystem>();
        protected Input.Actions IInput;

        private Action _wLTHandler;
        private Action _wUTHandler;
        private Action _wRTHandler;
        private Action _wDTHandler;

        public float GetHealth() { return _health; }
        public float GetMaxHealth() { return maxHealth; }
        public int GetArmorClass() { return armorClass; }
        public int GetArmorDurability() { return armorDurability; }
        public float GetMood() { return _mood; }
        public DamageType GetDmgTypeResistance() { return damageTypeResistance; }
        public WeaponType Weapon { get; private set; }
        public Boolean HasCustomName() { return !string.IsNullOrEmpty(entityName); }

        public Boolean HasCustomSprites()
        {
            bool hasSprites = true;
            if (dialogSprites == null) return false;
            if (dialogSprites.dialogBox == null) hasSprites = false;
            if (dialogSprites.dialogOption == null) hasSprites = false;
            if (dialogSprites.nameDivider == null) hasSprites = false;
            return hasSprites;
        }
        
        public event Action OnHit;
        public event Action OnKilled;
        public event Action OnHeal;

        private void Awake()
        {
            HurtFX = GetComponent<HurtFX>();
            if (HurtFX == null) { HurtFX = gameObject.AddComponent<HurtFX>(); }
            groundLayers = (groundLayers == 0)? LayerMask.GetMask("Default", "Ground") : groundLayers;
            
            _health = GetMaxHealth();
            Weapon = weaponTypeOverride;
            entityName = HasCustomName() ? this.entityName : MiscUtils.GetRandomName(
                MiscUtils.GetOrCreateGameManager().randomNames, this.nameCustomization);
            
            EDebug.Log("LivingEntity ► Awake: " + this.entityName.ToString());
            OnAwoken();

            Animator = GetComponent<Animator>();
            
            GameManager.Instance.Subscribe(OnStateChange);
            OnStateChange(GameManager.Instance.GameState);
            
            if (!isPlayer) return;
            IInput = (Input.Actions.Instance != null)? Input.Actions.Instance : MiscUtils.GetOrCreateGameManager().gameObject.GetComponent<Input.Actions>();
            _wLTHandler = () => ChangeWeapon(WeaponType.LightSword);
            _wUTHandler = () => ChangeWeapon(WeaponType.GreatSword);
            _wRTHandler = () => ChangeWeapon(WeaponType.NamePending3);
            _wDTHandler = () => ChangeWeapon(WeaponType.NamePending4);
            IInput.OnWeaponLeftToggledEvent += _wLTHandler;
            IInput.OnWeaponUpToggledEvent += _wUTHandler;
            IInput.OnWeaponRightToggledEvent += _wRTHandler;
            IInput.OnWeaponDownToggledEvent += _wDTHandler;
            if (hurtFXVars != null && hurtFXVars.renderer != null) hurtFXVars.ogMaterials = hurtFXVars.renderer.materials;
            else EDebug.LogError("hurtFXVars or the renderer are not initialized/set", this);
        }

        private void OnEnable()
        {
            GameManager.Instance.RegisterUnsubscribeAction(Unsubscribe);
        }

        private void ChangeWeapon(WeaponType weapon)
        {
            Weapon = (Weapon == weapon) ? WeaponType.Unarmed : weapon;
        }

        public void ChangeMood(float amount)
        {
            _mood = Mathf.Clamp(_mood + amount, -10, 10);
        }

        private void Unsubscribe()
        {
            if (Singleton<GameManager>.applicationIsQuitting) return;
            GameManager.Instance?.Unsubscribe(OnStateChange);
            if (!isPlayer || IInput == null) return;
            IInput.OnWeaponLeftToggledEvent -= _wLTHandler;
            IInput.OnWeaponUpToggledEvent -= _wUTHandler;
            IInput.OnWeaponRightToggledEvent -= _wRTHandler;
            IInput.OnWeaponDownToggledEvent -= _wDTHandler;
        }

        public virtual void TakeDamage(Vector3 hitPoint, Vector3 hitDirection, DamageType dmgType, DamageType resType,
            float damage, float knockBack, float stagger, float armorPenetration, float critRate, float critDmg)
        {
            if (canTakeDamage)
            {
                OnHit?.Invoke();
                float damageReduction = GetDamageReduction(armorPenetration);
                float finalDamage = damage * (1 - damageReduction);
                if (Random.value < critRate)
                    finalDamage *= critDmg; // Rand -> 0 - 1 The higher the critRate, the higher the chance of crit
                _health -= finalDamage;
                EDebug.Log(this.entityName + "Took " + finalDamage + " damage. Health: " + _health);
                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb != null) {
                    Vector3 knockBackForce = hitDirection * knockBack;
                    knockBackForce.y += 0.5f;
                    rb.AddForce(knockBackForce, ForceMode.Impulse);
                }
                else transform.position += hitDirection * knockBack;
                if (_health <= 0 && !isDead) {
                    isDead = true;
                    Die();
                    if (criticalDmgParticles.Length > 0)
                        PlayParticleEffect(criticalDmgParticles[Random.Range(0, criticalDmgParticles.Length)], hitPoint,
                            hitDirection);
                    else
                        Debug.LogWarning("criticalDmgParticles array is empty.");
                }
                else {
                    if (_damageImmunityCoroutine != null)
                        StopCoroutine(_damageImmunityCoroutine);
                    _damageImmunityCoroutine = StartCoroutine(DamageImmunity());
                    if (dmgParticles.Length > 0)
                        PlayParticleEffect(dmgParticles[Random.Range(0, dmgParticles.Length)], hitPoint, hitDirection);
                    else
                        Debug.LogWarning("dmgParticles array is empty.");
                }
                if (stagger > 0) {
                    // Same with the stagger stuff
                }
                if (armorClass > 1) ReduceArmorDurability();
                OnDamageTaken();
            }
            OnHurtButNoDamage();
        }

        protected virtual void OnDamageTaken()
        {
            HurtFX?.Hit(hurtFXVars);
            if (isPlayer) CamShaker.Instance.ShakeIt(0.25f, 10);
        }
        protected virtual void OnHurtButNoDamage(){}
        protected virtual void OnHealed(){}
        protected virtual void OnAwoken(){}

        public virtual void Heal(float amount)
        {
            _health = Mathf.Min(maxHealth, _health + amount);
            OnHeal?.Invoke();
            OnHealed();
        }

        protected virtual float GetDamageReduction(float armorPiercing)
        {
            float reduction = 0f;
            switch (armorClass)
            {
                case 2:
                    reduction = 0.25f;
                    break;
                case 3:
                    reduction = 0.5f;
                    break;
            }
            return Mathf.Max(0, reduction - armorPiercing);
        }

        protected virtual void ReduceArmorDurability()
        {
            armorDurability--;
            if (armorDurability <= 0)
            {
                armorClass = Mathf.Max(1, armorClass - 1);
                armorDurability = 3;
            }
        }

        private IEnumerator DamageImmunity()
        {
            canTakeDamage = false;
            yield return new WaitForSeconds(dmgImmunityTime);
            canTakeDamage = true;
        }

        protected virtual void PlayParticleEffect(ParticleSystem particlePrefab, Vector3 position, Vector3 direction)
        {
            if (particlePrefab == null)
            {
                Debug.LogWarning("No particle system assigned.");
                return;
            }

            ParticleSystem system = GetInactiveParticleSystem(particlePrefab);
            if (system == null)
            {
                system = Instantiate(particlePrefab, position, Quaternion.LookRotation(-direction));
                _particleSystems.Add(system);
                system.gameObject.SetActive(true);
            }
            else
            {
                system.transform.position = position;
                system.transform.rotation = Quaternion.LookRotation(-direction);
                system.gameObject.SetActive(true);
            }
            system.transform.SetParent(transform);
            system.Play();
        }

        private ParticleSystem GetInactiveParticleSystem(ParticleSystem prefab)
        {
            foreach (ParticleSystem ps in _particleSystems)
            {
                if (!ps.gameObject.activeInHierarchy && (ps.name == prefab.name || ps.name == prefab.name + "(Clone)"))
                    return ps;
            }
            return null;
        }

        protected virtual void Die()
        {
            isDead = true;
            OnKilled?.Invoke();
            // So... what happens when the entity dies? 
            // I could try a skinned mesh particle system that could look cool, but I don't know...
            // What are we making? 
        }

        /// <summary>
        /// Sets health but no beyond maxHealth
        /// </summary>
        /// <param name="health"></param>
        public void SetHealth(float health)
        {
            _health = Mathf.Clamp(health, 0, maxHealth);
            // If 0 death
        }
        
        protected virtual void OnStateChange(GameStates state)
        {
            gameState = state;
            Animator animator = GetComponent<Animator>();
            animator.enabled = (state == GameStates.Playing);
        }


        #region SAVING
        public string SaveLivingEntity()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("LE");
            sb.Append("/");
            sb.Append(entityName);
            //sb.Append(SaveDialogOptions());
            sb.Append("/");
            sb.Append(SaveNameCustomization());
            sb.Append('/');
            sb.Append(isPlayer ? 1 : 0);
            sb.Append('/');
            sb.Append(maxHealth);
            sb.Append('/');
            sb.Append(dmgImmunityTime);
            sb.Append('/');
            sb.Append(armorClass);
            sb.Append('/');
            sb.Append(armorDurability);
            sb.Append('/');
            sb.Append(SaveDamegeType());
            sb.Append('/');
            sb.Append(GetHealth());
            sb.Append('/');
            sb.Append(_mood);
            sb.Append('/');
            sb.Append(isDead ? 1 : 0);
            sb.Append("/");
            sb.Append(canTakeDamage ? 1 : 0);
            sb.Append('/');
            sb.Append((int)gameState);
            sb.Append('/');
            sb.Append(transform.position.x);
            sb.Append('/');
            sb.Append(transform.position.y);
            sb.Append('/');
            sb.Append(transform.position.z);

            return sb.ToString();
        }

        // TODO : TERMINA ESTA FUNCION recursividad infinita
        private string SaveDialogOptions()
        {
            StringBuilder sb = new StringBuilder();
            List<DialogOption> options = dialogOptions.dialogOptions;
            //Localization.Translate

            sb.Append("/");
            sb.Append(options.Count);
            for (int i = 0; i < options.Count; ++i)
            {
                sb.Append("/");
                sb.Append(options[i].npcDialog);
                sb.Append("/");
                sb.Append(options[i].userResponses.Count);
                for (int j = 0; j < options[i].userResponses.Count; ++j)
                {
                    sb.Append("/");
                    sb.Append(options[i].userResponses[j].response);
                    sb.Append("/");
                    sb.Append(options[i].userResponses[j].moodChange);
                }
            }
            return sb.ToString();
        }


        private string SaveNameCustomization()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameCustomization.isMale ? 1 : 0);
            sb.Append(nameCustomization.includeName ? 1 : 0);
            sb.Append(nameCustomization.includeLastName ? 1 : 0);
            sb.Append(nameCustomization.includeNickname ? 1 : 0);
            sb.Append(nameCustomization.includeTitle ? 1 : 0);
            sb.Append(nameCustomization.startsWithTitle ? 1 : 0);
            sb.Append(nameCustomization.replaceNameWithNickname ? 1 : 0);
            sb.Append(nameCustomization.lastNameThenName ? 1 : 0);
            sb.Append(nameCustomization.useTitleDividers ? 1 : 0);

            return sb.ToString();
        }

        private string SaveDamegeType()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append((int)damageTypeResistance);

            return sb.ToString();
        }

        #endregion


        #region LOADING

        public void loadData(string _data)
        {
            string[] dataDivided = _data.Split('/');
            int index = 0;
            if (dataDivided[index] != "LE")
            {
                EDebug.LogError("Datos incompatibles", this);
                EDebug.Log(_data, this);
                return;
            }

            index += 1;
            entityName = dataDivided[index];

            index += 1;
            //loadDataSaveDialogOptions(dataDivided, ref index);
            loadDataCustomization(dataDivided, ref index);

            index += 1;
            string isPlayerBool = dataDivided[index];

            isPlayer = (isPlayerBool[0] - '0') >= 1;
            index += 1;

            maxHealth = int.Parse(dataDivided[index]);
            index += 1;

            dmgImmunityTime = float.Parse(dataDivided[index]);
            index += 1;

            armorClass = int.Parse(dataDivided[index]);
            index += 1;

            armorDurability = int.Parse(dataDivided[index]);
            index += 1;

            loadDamageType(dataDivided, ref index);
            index += 1;

            _health = int.Parse(dataDivided[index]);
            index += 1;

            _mood = float.Parse(dataDivided[index]);
            index += 1;

            string isDeadBool = dataDivided[index];
            isDead = (isDeadBool[0] - '0') >= 1;
            index += 1;

            string canTakeDamageBool = dataDivided[index];
            canTakeDamage = (canTakeDamageBool[0] - '0') >= 1;
            index += 1;

            gameState = (GameStates)int.Parse(dataDivided[index]);

            Vector3 entity_pos = Vector3.zero;

            index += 1;
            entity_pos.x = float.Parse(dataDivided[index]);
            index += 1;
            entity_pos.y = float.Parse(dataDivided[index]);
            index += 1;
            entity_pos.z = float.Parse(dataDivided[index]);
            transform.position = entity_pos;

        }

        private void loadDataCustomization(string[] _data, ref int index)
        {
            string boolArray = _data[index];
            nameCustomization.isMale = (boolArray[0] - '0') >= 1;
            nameCustomization.includeName = (boolArray[1] - '0') >= 1;
            nameCustomization.includeLastName = (boolArray[2] - '0') >= 1;
            nameCustomization.includeNickname = (boolArray[3] - '0') >= 1;
            nameCustomization.includeTitle = (boolArray[4] - '0') >= 1;
            nameCustomization.startsWithTitle = (boolArray[5] - '0') >= 1;
            nameCustomization.replaceNameWithNickname = (boolArray[6] - '0') >= 1;
            nameCustomization.lastNameThenName = (boolArray[7] - '0') >= 1;
            nameCustomization.useTitleDividers = (boolArray[8] - '0') >= 1;
        }

        private void loadDamageType(string[] _data, ref int index)
        {
            damageTypeResistance = (DamageType)int.Parse(_data[index]);
        }

        #endregion

    }
}
