using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{

    using SaveSystem;
    [DefaultExecutionOrder(2)]
    public class LoadSettingOnStart : MonoBehaviour
    {
        void Start()
        {
            GameManager gm = GameManager.Instance;
            WindowResolution winRes = SaveSystem.GetWindowResolution();
            Language lang = SaveSystem.GetLanguage();
            LanguageManager.Instance.setLanguage(lang);
            gm.ChangeResolution(winRes);
        }
    }

}
