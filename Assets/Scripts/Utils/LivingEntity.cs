using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utils
{
    public class LivingEntity : MonoBehaviour
    {
        public delegate void GameStateChange(GameStates state);
        public event GameStateChange OnGameStateChange;

        public void ChangeGameState(GameStates state)
        {
            gameState = state;
            OnGameStateChange?.Invoke(state);
        }   
        
        [Header("Living Entity Variables")]
        public bool isPlayer;
        [Tooltip("The particle systems that will play when the entity takes damage (Will be skipped if none are present)")]
        public ParticleSystem[] dmgParticles;
        [Tooltip("The particle systems that will play when the entity takes LETHAL damage (Will be skipped if none are present)")]
        public ParticleSystem[] criticalDmgParticles;
        [Tooltip("The entity's maximum health")]
        [SerializeField] private float maxHealth = 100f;
        [Tooltip("For how long is the entity immune to damage after taking some?")]
        [SerializeField] private float dmgImmunityTime = 0.05f;
        [SerializeField] private int armorClass = 1;
        [SerializeField] private int armorDurability = 3;
        
        private float _health;
        [HideInInspector] public bool isDead;
        [HideInInspector] public bool canTakeDamage = true;
        [HideInInspector] public GameStates gameState;

        protected Coroutine DamageImmunityCoroutine;
        private readonly List<ParticleSystem> _particleSystems = new List<ParticleSystem>();
        protected Input.Actions IInput = default;

        private Action _wLTHandler;
        private Action _wUTHandler;
        private Action _wRTHandler;
        private Action _wDTHandler;

        public float GetHealth() { return _health; }
        public float GetMaxHealth() { return maxHealth; }
        public int GetArmorClass() { return armorClass; }
        public int GetArmorDurability() { return armorDurability; }
        public WeaponType Weapon { get; private set; }
        
        private void Awake()
        {
            _health = maxHealth;
            Weapon = WeaponType.Unarmed;
            if (!isPlayer) return;
            IInput = Input.Actions.Instance;
            if (IInput == null) IInput = gameObject.GetComponent<Input.Actions>();
            if (IInput == null) IInput = gameObject.AddComponent<Input.Actions>();
            if (IInput != null)
            {
                _wLTHandler = () => ChangeWeapon(WeaponType.LightSword);
                _wUTHandler = () => ChangeWeapon(WeaponType.GreatSword);
                _wRTHandler = () => ChangeWeapon(WeaponType.NamePending3);
                _wDTHandler = () => ChangeWeapon(WeaponType.NamePending4);
                IInput.OnWeaponLeftToggledEvent += _wLTHandler;
                IInput.OnWeaponUpToggledEvent += _wUTHandler;
                IInput.OnWeaponRightToggledEvent += _wRTHandler;
                IInput.OnWeaponDownToggledEvent += _wDTHandler;
            }
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

        private void Unsubscribe()
        {
            if (!isPlayer) return;
            if(IInput == null) return;
            IInput.OnWeaponLeftToggledEvent -= _wLTHandler;
            IInput.OnWeaponUpToggledEvent -= _wUTHandler;
            IInput.OnWeaponRightToggledEvent -= _wRTHandler;
            IInput.OnWeaponDownToggledEvent -= _wDTHandler;
        }

        public virtual void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection, float armorPiercing = 0f)
        {
            float damageReduction = GetDamageReduction(armorPiercing);
            float finalDamage = damage * (1 - damageReduction);
            _health -= finalDamage;
            if (_health <= 0 && !isDead)
            {
                isDead = true;
                Die();
                if (criticalDmgParticles.Length > 0) 
                    PlayParticleEffect(criticalDmgParticles[Random.Range(0, criticalDmgParticles.Length)], hitPoint, hitDirection);
                else
                    Debug.LogWarning("criticalDmgParticles array is empty.");
            }
            else
            {
                if (DamageImmunityCoroutine != null)
                    StopCoroutine(DamageImmunityCoroutine);
                DamageImmunityCoroutine = StartCoroutine(DamageImmunity());
                if (dmgParticles.Length > 0)
                    PlayParticleEffect(dmgParticles[Random.Range(0, dmgParticles.Length)], hitPoint, hitDirection);
                else
                    Debug.LogWarning("dmgParticles array is empty.");
            }
            if (armorClass > 1) ReduceArmorDurability();
        }
        
        public virtual void Heal(float amount)
        {
            _health = Mathf.Min(maxHealth, _health + amount);
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
            if(health > maxHealth)
            {
                _health = maxHealth;
                return;
            }
            _health = health;
        }
        
    }
}
