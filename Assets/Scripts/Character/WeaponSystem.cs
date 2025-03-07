using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponSystem : MonoBehaviour
{
    public static WeaponSystem Instance { get; private set; }
    [Header("Sword GameObject Variable")] 
    [FormerlySerializedAs("_sword")] [SerializeField]
    private GameObject sword = default;
    [Header("Halberd GameObject Variable")]
    [FormerlySerializedAs("polearm")] [FormerlySerializedAs("_polearm")] [SerializeField]
    private GameObject halberd = default;

    private void Awake()
    {
        Instance = this;
        if (Instance!= this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UsingSword()
    {
        //Add Animation Calling Here
        //Add VFX Calling Here if applicable
        //Add SFX Calling Here
        sword.SetActive(true);
    }

    public void UsingHalberd()
    {
        //Add Animation Calling Here
        //Add VFX Calling Here if applicable
        //Add SFX Calling Here
        halberd.SetActive(true);
    }

    #region Keybinds

        void SwordKeybind()
        {
            //TODO: Change this for New Input Sys when it's implemented
            if (Input.GetKeyDown(KeyCode.R))
            {
                UsingSword();
            }
        }

        void HalberdKeybind()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                UsingHalberd();
            }
        }

#endregion
}
