using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShaker : MonoBehaviour
{
    public static CamShaker instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    float shakeStr;
    int shakeTimeFrames = -1;
    Vector3 initialPos;

    public void SetShake(float _str, int _timeInFrames) 
    { 
        initialPos = transform.localPosition;
        shakeStr = _str;
        shakeTimeFrames = _timeInFrames;
    }
    public void CancelShake()
    {
        shakeTimeFrames = -1;
        transform.position = initialPos;
    }

    void FixedUpdate()
    {
        if (shakeTimeFrames > 0)
        {
            shakeTimeFrames--;
            transform.localPosition += Random.insideUnitSphere * shakeStr;
        }
        else if (shakeTimeFrames == 0)
        {
            CancelShake();
        }
    }
}
