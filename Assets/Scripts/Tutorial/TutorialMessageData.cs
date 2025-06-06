using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMessageData : MonoBehaviour
{
    public string textKey;
    public float duration;

    public TutorialMessageData(string key, float time)
    {
        textKey = key;
        duration = time;
    }
}
