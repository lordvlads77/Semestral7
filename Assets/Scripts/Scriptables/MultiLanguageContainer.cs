using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Scriptables
{

    [CreateAssetMenu(fileName = "MultiLanguageContainer", menuName = "Scriptables/MultiLanguageContainer")]
    public class MultiLanguageContainer : ScriptableObject
    {
        public LanguageContainer[] languageContainers;

        public LanguageContainer findLanguageContainerOfLanguage(Utils.Languege selected_lang) {
            LanguageContainer result = default;

            for (int i = 0; i < languageContainers.Length; ++i)
            {
                if( languageContainers[i].language == selected_lang)
                {
                    result = languageContainers[i];
                    break;
                }
            }


            return result;
        }

    }

    [System.Serializable]
    public struct LanguageContainer
    {
        [TextArea] public string text;
        public Utils.Languege language;
    }

}

