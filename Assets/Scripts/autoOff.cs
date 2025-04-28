using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class autoOff : MonoBehaviour
{
    public int frameToLive = 60;
    int frameCount = 0;
    private void OnEnable()
    {
        frameCount = frameToLive;
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject,1);
    }
}
