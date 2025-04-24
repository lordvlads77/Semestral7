using System.Collections;
using System.Collections.Generic;
using Scriptables;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ChangeLanguageScript : MonoBehaviour
    {
        [SerializeField] Scriptables.MultiLanguageContainer languageContainer;

        [SerializeField] Text uiText;


        private void OnEnable()
        {
            if (uiText == null)
            {
                uiText = GetComponent<Text>();
            }

            LanguageManager.Instance.Subscribe(OnLanguageChange);
            OnLanguageChange(LanguageManager.Instance.currentLanguage);
        }

        private void OnDisable()
        {
            LanguageManager.TryGetInstance()?.UnSubscribe(OnLanguageChange);
        }


        public void OnLanguageChange(Utils.Languege newLanguage)
        {
            LanguageContainer container = languageContainer.findLanguageContainerOfLanguage(newLanguage);
            uiText.text = container.text;
        }
    }

}

