using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLanguageManager : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {

        Vector2 movement = Input.Actions.Instance.Movement;

        if (movement.x > 0)
        {
            EDebug.Log("setting lang to english");
            LanguageManager.Instance.setLanguage(Utils.Languege.English);
        }

        if (movement.x < 0)
        {
            EDebug.Log("setting lang to spanish");
            LanguageManager.Instance.setLanguage(Utils.Languege.Spanish);
        }
    }
}
