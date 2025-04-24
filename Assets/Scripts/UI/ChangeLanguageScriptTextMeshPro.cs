using TMPro;
using UnityEngine;

namespace UI
{
    using Scriptables;

    public class ChangeLanguageScriptTextMeshPro : MonoBehaviour
    {
        [SerializeField] Scriptables.MultiLanguageContainer languageContainer;

        [SerializeField] TextMeshProUGUI uiText;


        private void OnEnable()
        {
            if (uiText == null)
            {
                uiText = GetComponent<TextMeshProUGUI>();
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

