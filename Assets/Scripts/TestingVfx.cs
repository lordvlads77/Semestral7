using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingVfx : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Untagged"))
        {
            VfxCallbacks.Instance.ImpactHitVFX();
        }
    }
}
