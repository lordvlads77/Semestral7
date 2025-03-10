using System.Collections;
using System.Collections.Generic;
using Entity;
using UnityEngine;

public sealed class TestDummy : MonoBehaviour
{
    Dummy dummy = null;
    [SerializeField] bool IsDummyBeingTested = false;
    [SerializeField] Vector3 HitDirection = Vector3.forward;
    void Start()
    {
        dummy = GetComponent<Dummy>();
        EDebug.Assert(dummy != null, "Attach me to a object with the Dummy script please");
    }

    /*void Update()
    {
        if(!IsDummyBeingTested) { return; }

        /*if (Input.GetKeyDown(KeyCode.RightShift))
        {
            EDebug.Log("Pressed right shift");
            Transform d_transform = dummy.transform;
            dummy.TakeDamage(10, d_transform.position, HitDirection);
        }
    }*/
}
