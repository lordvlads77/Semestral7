using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this to a prefab 
/// and use it to change the language from the editor
/// </summary>
public sealed class ChangeLanguageInRealTime : MonoBehaviour
{
    [Tooltip("☐ = do NOT change Language,\n☑ = DO change Language ")]
    [SerializeField] private bool shouldChangeLanguage = true;

    private Utils.Languege currentLanguage;

    [field: SerializeField] public Utils.Languege desiredLanguage { get; private set; }


    private void FixedUpdate()
    {
        if (!shouldChangeLanguage) { return; }

        if (currentLanguage != desiredLanguage && desiredLanguage != Utils.Languege.COUNT)
        {
            LanguageManager.Instance.setLanguage(desiredLanguage);
        }

    }


    #region LanguageManagerBoilerPlate

    private void OnEnable()
    {
        LanguageManager.Instance.Subscribe(OnLanguageChange);
        OnLanguageChange(LanguageManager.Instance.currentLanguage);
    }

    private void OnDisable()
    {
        LanguageManager.TryGetInstance()?.UnSubscribe(OnLanguageChange);
    }

    private void OnLanguageChange(Utils.Languege _language)
    {
        currentLanguage = _language;
    }

    #endregion

}
