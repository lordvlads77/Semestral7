using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class LanguageManager : Utils.Singleton<LanguageManager>
{
    public Utils.Language currentLanguage { get; private set; } = Utils.Language.En;

    Action<Utils.Language> currentAction = null;


    public void Subscribe(Action<Utils.Language> method)
    {
        currentAction += method;
    }

    public void UnSubscribe(Action<Utils.Language> method)
    {
        currentAction -= method;
    }

    public void setLanguage(Utils.Language newLanguage)
    {
        currentLanguage = newLanguage;
        if (currentAction != null) { currentAction.Invoke(currentLanguage); }
    }
}
