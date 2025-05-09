using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxCallbacks : MonoBehaviour
{
    public static VfxCallbacks Instance { get; private set; }
    [SerializeField] private GameObject _ImpactHitvfx = default;
    [SerializeField] private Transform _impactHitvfxtRTransform = default;
    [SerializeField] private GameObject _bloodVFX = default;
    [SerializeField] private Transform _bloodVfxTransform = default;
    [SerializeField] private GameObject _groundImpactVFX = default;
    [SerializeField] private Transform _groundImpactVfxTransform = default;

    public void ImpactHitVFX()
    {
        Instantiate(_ImpactHitvfx, _impactHitvfxtRTransform.position, _impactHitvfxtRTransform.rotation);
    }

    public void bloodVFX()
    {
        Instantiate(_ImpactHitvfx, _bloodVfxTransform.position, _bloodVfxTransform.rotation);
    }
    
    public void GroundImpactVFX()
    {
        Instantiate(_groundImpactVFX, _groundImpactVfxTransform.position, _groundImpactVfxTransform.rotation);
    }
}
