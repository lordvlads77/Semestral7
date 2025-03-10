using UnityEngine;
using Utils;

namespace Entity
{
    public sealed class Dummy : LivingEntity
    {
        
        protected override void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection, float armorPiercing = 0)
        {
            base.TakeDamage(damage, hitPoint, hitDirection, armorPiercing);
        }

        protected override void Die()
        {
            base.Die();
        }

    }
}
