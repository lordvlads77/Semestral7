using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Objects
{
    public class DamageObject : MonoBehaviour
    {
        [SerializeField] private bool onlyHurtsPlayer = false;
        [SerializeField] private float damageInterval = 0.5f;
        [SerializeField] private WeaponStatistics weaponStats;
        
        private Dictionary<LivingEntity, float> _timers = new Dictionary<LivingEntity, float>();
        
        private void OnTriggerEnter(Collider other)
        {
            LivingEntity entity = other.GetComponent<LivingEntity>();
            if (entity != null)
                _timers.TryAdd(entity, 0f);
        }

        private void OnTriggerStay(Collider other)
        {
            LivingEntity entity = other.GetComponent<LivingEntity>();

            if (entity == null) return;
            
            if (onlyHurtsPlayer && !entity.isPlayer) return;
            if (!_timers.TryGetValue(entity, out float timer) || timer > 0f) return;
            CombatUtils.Attack(transform ,weaponStats, entity);
            _timers[entity] = damageInterval;
        }
        
        private void OnTriggerExit(Collider other)
        {
            LivingEntity entity = other.GetComponent<LivingEntity>();
            if (entity != null && _timers.ContainsKey(entity))
            {
                _timers.Remove(entity);
            }
        }
        
        private void FixedUpdate()
        {
            List<LivingEntity> entities = new List<LivingEntity>(_timers.Keys);
            for (int i = 0; i < entities.Count; i++)
            {
                LivingEntity entity = entities[i];
                if (_timers[entity] > 0f)
                    _timers[entity] -= Time.deltaTime;
            }
        }
    }
}
