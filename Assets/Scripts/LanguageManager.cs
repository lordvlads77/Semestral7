using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class LanguageManager : Utils.Singleton<LanguageManager>
{
    public Utils.Languege currentLanguage { get; private set; } = Utils.Languege.English;

    Action<Utils.Languege> currentAction = null;


    public void Subscribe(Action<Utils.Languege> method)
    {
        currentAction += method;
    }

    public void UnSubscribe(Action<Utils.Languege> method)
    {
        currentAction -= method;
    }

    public void setLanguage(Utils.Languege newLanguage)
    {
        currentLanguage = newLanguage;
        if (currentAction != null) { currentAction.Invoke(currentLanguage); }
    }
}
