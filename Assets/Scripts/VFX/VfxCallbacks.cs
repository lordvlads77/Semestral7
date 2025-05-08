using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxCallbacks : MonoBehaviour
{
    public static VfxCallbacks Instance { get; private set; }
    [SerializeField] private GameObject _ImpactHitvfx = default;
    [SerializeField] private Transform _impactHitvfxtRTransform = default;

    public void ImpactHitVFX()
    {
        Instantiate(_ImpactHitvfx, _impactHitvfxtRTransform.position, _impactHitvfxtRTransform.rotation);
    }
}
