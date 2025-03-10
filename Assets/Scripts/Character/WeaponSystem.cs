using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

public class WeaponSystem : Singleton<WeaponSystem>
{
    public static WeaponSystem Instance { get; private set; }
    [Header("Sword GameObject Variable")] 
    [FormerlySerializedAs("_sword")] [SerializeField]
    private GameObject sword = default;
    [Header("Halberd GameObject Variable")]
    [FormerlySerializedAs("polearm")] [FormerlySerializedAs("_polearm")] [SerializeField]
    private GameObject halberd = default;

    protected override void OnAwake()
    {
        Instance = this;
        if (Instance!= this)
        {
            Destroy(gameObject);
        }
    }

    public void Unarmed()
    {
        sword.SetActive(false);
        halberd.SetActive(false);
    }

    public void UsingSword()
    {
        //Add Animation Calling Here
        //Add VFX Calling Here if applicable
        //Add SFX Calling Here
        sword.SetActive(true);
        halberd.SetActive(false);
    }

    public void UsingHalberd()
    {
        //Add Animation Calling Here
        //Add VFX Calling Here if applicable
        //Add SFX Calling Here
        halberd.SetActive(true);
        sword.SetActive(false);
    }

}
