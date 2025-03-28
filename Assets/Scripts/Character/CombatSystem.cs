using Scriptables;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Character
{
    public class CombatSystem : Singleton<CombatSystem>
    {
        [FormerlySerializedAs("_EnemyObject")]
        [Header("Weapon Object Ref")]
        [SerializeField] private GameObject[] weaponObject = default;
        [FormerlySerializedAs("_hitdir")]
        [Header("Hit Point Var")]
        [SerializeField] private LivingEntity player = default;

        protected override void OnAwake()
        {
            if (player == null)
            {
                player = GameObject.FindWithTag("Player").GetComponent<LivingEntity>();
            }
        }
    
        private void OnTriggerEnter(Collider other)
        {
            if (weaponObject[0].CompareTag("Enemy"))
            {
                LivingEntity enemy = other.GetComponent<LivingEntity>();
                if (enemy == null)
                {
                    EDebug.LogError("No LivingEntity component found on " + other.name + " (Enemy)");
                    return;
                }
                CombatUtils.Attack(player, enemy);
            }

            if (weaponObject[1].CompareTag("Enemy"))
            {
                LivingEntity enemy = other.GetComponent<LivingEntity>();
                if (enemy == null)
                {
                    EDebug.LogError("No LivingEntity component found on " + other.name + " (Enemy)");
                    return;
                }
                CombatUtils.Attack(player, enemy);
            }
        }
    
    }
}
