using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class LanguageManager : Utils.Singleton<LanguageManager>
{
    public Utils.Language currentLanguage { get; private set; } = Utils.Language.En;

    Action<Utils.Language> ChangeLanguage = null;

    /// <summary>
    /// This call a function that is meant to force a language change
    /// </summary>
    Action forcedChange = null;


    public void Subscribe(Action<Utils.Language> method)
    {
        ChangeLanguage += method;
    }

    public void UnSubscribe(Action<Utils.Language> method)
    {
        ChangeLanguage -= method;
    }

    public void setLanguage(Utils.Language newLanguage)
    {
        currentLanguage = newLanguage;

        if (ChangeLanguage != null)
            ChangeLanguage.Invoke(currentLanguage);
    }

    public void ForceCallSubscribe(Action _forcedChange)
    {
        forcedChange += _forcedChange;
    }

    public void ForceCallUnSubscribe(Action _forcedChange)
    {
        forcedChange -= _forcedChange;
    }

}
