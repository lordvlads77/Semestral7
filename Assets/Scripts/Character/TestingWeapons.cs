using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingWeapons : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            WeaponSystem.Instance.UsingSword();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            WeaponSystem.Instance.UsingHalberd();
        }
    }
}
