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
        if (ChangeLanguage != null) { ChangeLanguage.Invoke(currentLanguage); }
    }

    public void ForceCallSubscribe(Action _forcedChange)
    {
        forcedChange += _forcedChange;
    }

    public void ForceCallUnSubscribe(Action _forcedChange)
    {
        forcedChange -= _forcedChange;
    }

    public void ForceLanguageChange(Utils.Language newLanguage)
    {
        /// this is a hack to get around code that looks like this
        /// 
        /// if (current_language == new_language) {return ;} // aka do nothing
        ///
        /// assuming you have a better idea please tell me

        Instance.setLanguage(Utils.Language.En);
        Instance.setLanguage(Utils.Language.Es);
        Instance.setLanguage(newLanguage);
    }


}
