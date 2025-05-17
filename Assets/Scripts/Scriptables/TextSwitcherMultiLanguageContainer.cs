using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptables
{

    /// <summary>
    /// THis Scriptable is the text switcher that also has to change as the language changes
    /// </summary>

    [CreateAssetMenu(fileName = "TextSwitcherMultiLanguageContainer", menuName = "Scriptables/Text Switcher Multi Language Container")]
    public sealed class TextSwitcherMultiLanguageContainer : ScriptableObject
    {

        [SerializeField] TextSwitcherLanguageContainer[] container;
        static readonly string[] defaultText = { "NULL", "TYPE" };

        public TextSwitcherLanguageContainer findLanguageContainer(Utils.Language lang)
        {
            TextSwitcherLanguageContainer result = this.nullType();
            for (int i = 0; i < container.Length; i++)
            {
                if (container[i].language == lang)
                {
                    result = container[i];
                }
            }

            return result;
        }


        private TextSwitcherLanguageContainer nullType()
        {
            TextSwitcherLanguageContainer result;
            result.text = defaultText;
            result.language = Utils.Language.En;
            return result;
        }


    }

    [System.Serializable]
    public struct TextSwitcherLanguageContainer
    {
        [TextArea] public string[] text;
        public Utils.Language language;
    }

}
