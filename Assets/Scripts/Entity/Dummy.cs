using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public sealed class Dummy : LivingEntity
{
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection, float armorPiercing = 0)
    {
        base.TakeDamage(damage, hitPoint, hitDirection, armorPiercing);
    }

    protected override void Die()
    {
        base.Die();
    }


}
