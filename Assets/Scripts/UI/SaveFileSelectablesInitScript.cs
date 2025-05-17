using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// This script need to be put on the parent object off all the SaveFileSelectable types
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class SaveFileSelectablesInitScript : MonoBehaviour
    {
        SaveFileSelectable[] saveFileSelectables;

        void Start()
        {
            saveFileSelectables = gameObject.GetComponentsInChildren<SaveFileSelectable>();
            Debug.Assert(saveFileSelectables != null, $"This script need to be put on the parent object of all the {typeof(SaveFileSelectable)} types", this);
            var curr_lang = LanguageManager.Instance.currentLanguage;
            LanguageManager.Instance.setLanguage(curr_lang);
        }

        private void FixedUpdate()
        {

            for (int i = 0; i < saveFileSelectables.Length; i++)
            {
                bool doesSaveFileExist = SaveSystem.SaveSystem.DoesSaveFileExist(i);

                saveFileSelectables[i].saveFileIndex = i;
                saveFileSelectables[i].isBeingUsed = doesSaveFileExist;
                saveFileSelectables[i].setTextIndicator();
            }
            
        }



    }
}

