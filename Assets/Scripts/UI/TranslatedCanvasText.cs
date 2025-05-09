using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class TranslatedCanvasText : MonoBehaviour
    {
        [SerializeField] private string textKey;
        private Graphic _text;
        
        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            if (_text == null) _text = GetComponent<Text>();
            if (_text == null) {
                EDebug.LogError("TranslatedText: 'TMP_Text' or 'Text' component not found.");
                return;
            }
            if (string.IsNullOrEmpty(textKey)) {
                EDebug.LogError("TranslatedText: 'textKey' is not set.");
                return;
            }
            switch (_text) {
                case TMP_Text tmpText:
                    tmpText.text = Localization.Translate(textKey);
                    break;
                case Text uiText:
                    uiText.text = Localization.Translate(textKey);
                    break;
            }
        }
    }
}
