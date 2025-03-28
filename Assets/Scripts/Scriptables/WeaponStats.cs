using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "WeaponStats", menuName = "Scriptables/WeaponStats")]
    public class WeaponStats : ScriptableObject
    {
        public WeaponStatistics lightSwordStats;
        public WeaponStatistics greatSwordStats;
        public WeaponStatistics namePending3Stats;
        public WeaponStatistics namePending4Stats;
        public WeaponStatistics unarmedStats;

        private Dictionary<WeaponType, WeaponStatistics> _weaponStatsDict;

        private void Awake()
        {
        }

        private void OnEnable()
        {
            _weaponStatsDict = new Dictionary<WeaponType, WeaponStatistics>
            {
                { WeaponType.LightSword, lightSwordStats },
                { WeaponType.GreatSword, greatSwordStats },
                { WeaponType.NamePending3, namePending3Stats },
                { WeaponType.NamePending4, namePending4Stats },
                { WeaponType.Unarmed, unarmedStats }
            };
        }

        public WeaponStatistics GetWeaponStats(WeaponType weaponType)
        {
            if (_weaponStatsDict.TryGetValue(weaponType, out var stats))
            {
                return stats;
            }
            Debug.LogWarning($"WeaponStats not found for WeaponType: {weaponType}");
            return null;
        }
        
    }
}
