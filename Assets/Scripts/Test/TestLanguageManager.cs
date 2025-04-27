using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLanguageManager : MonoBehaviour
{
    Input.Actions actions;
    void Start()
    {
        actions = Input.Actions.Instance; 
    }

    void Update()
    {

        Vector2 movement = actions.Movement;

        if (movement.x > 0)
        {
            EDebug.Log("setting lang to english");
            LanguageManager.Instance.setLanguage(Utils.Language.En);
        }

        if (movement.x < 0)
        {
            EDebug.Log("setting lang to spanish");
            LanguageManager.Instance.setLanguage(Utils.Language.Es);
        }
    }
}
