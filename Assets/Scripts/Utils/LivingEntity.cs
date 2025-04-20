using System;
using System.Collections;
using System.Collections.Generic;
using Entity;
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
        
        private void Awake()
        {
            HurtFX = GetComponent<HurtFX>();
            if (HurtFX == null) { HurtFX = gameObject.AddComponent<HurtFX>(); }
            
            _health = maxHealth;
            Weapon = weaponTypeOverride;
            entityName = HasCustomName()? this.entityName : MiscUtils.GetRandomName(
                MiscUtils.GetOrCreateGameManager().randomNames, this.nameCustomization);
            
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
            hurtFXVars.ogMaterials = hurtFXVars.renderer.materials;
            OnAwoken();
        }
        
        private void OnDestroy()
        {
            Unsubscribe();
        }
        private void OnDisable()
        {
            Unsubscribe();
        }
        
        private void ChangeWeapon(WeaponType weapon)
        {
            Weapon = (Weapon == weapon)? WeaponType.Unarmed : weapon;
        }
        
        public void ChangeMood(float amount)
        {
            _mood = Mathf.Clamp(_mood + amount, -10, 10);
        }

        private void Unsubscribe()
        {
            if (!isPlayer) return;
            if(IInput == null) return;
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
                float damageReduction = GetDamageReduction(armorPenetration);
                float finalDamage = damage * (1 - damageReduction);
                if (Random.value < critRate)
                    finalDamage *= critDmg; // Rand -> 0 - 1 The higher the critRate, the higher the chance of crit
                _health -= finalDamage;
                if (_health <= 0 && !isDead)
                {
                    isDead = true;
                    Die();
                    if (criticalDmgParticles.Length > 0)
                        PlayParticleEffect(criticalDmgParticles[Random.Range(0, criticalDmgParticles.Length)], hitPoint,
                            hitDirection);
                    else
                        Debug.LogWarning("criticalDmgParticles array is empty.");
                }
                else
                {
                    if (_damageImmunityCoroutine != null)
                        StopCoroutine(_damageImmunityCoroutine);
                    _damageImmunityCoroutine = StartCoroutine(DamageImmunity());
                    if (dmgParticles.Length > 0)
                        PlayParticleEffect(dmgParticles[Random.Range(0, dmgParticles.Length)], hitPoint, hitDirection);
                    else
                        Debug.LogWarning("dmgParticles array is empty.");
                }

                // Create knockBack logic here some time later
                if (stagger > 0)
                {
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
        }
        protected virtual void OnHurtButNoDamage(){}
        protected virtual void OnHealed(){}
        public virtual void OnAwoken(){}

        public virtual void Heal(float amount)
        {
            _health = Mathf.Min(maxHealth, _health + amount);
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
        
        protected void PlayParticleEffect(ParticleSystem particlePrefab, Vector3 position, Vector3 direction)
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
        
    }
}
