using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    using TMPro;
    using UnityEngine.UI;

    public class SaveFileSelectable : MonoBehaviour
    {
        [SerializeField] Text selfText;
        [SerializeField] TextMeshProUGUI indicatorText;
        [SerializeField] public int saveFileIndex;
        [SerializeField] public bool isBeingUsed = false;

        private void Awake()
        {
            if (selfText == null)
            {
                selfText.GetComponent<Text>();
                EDebug.Assert(selfText != null, $"El script necesita un instancia de |{typeof(UnityEngine.UI.Text)}| en la variable {nameof(selfText)} para funcionar", this);
            }

            EDebug.Assert(indicatorText != null, $"El script necesita un instancia de |{typeof(TextMeshProUGUI)}| en la variable {nameof(indicatorText)} para funcionar", this);
        }

        public void selectSaveFile()
        {
            SaveSystem.SaveSystem.setSaveFileIndex(saveFileIndex);
        }

        public void setTextIndicator()
        {
            TextSwitcherMultiLanguage theIndicatorTextSwitcher = indicatorText.GetComponent<TextSwitcherMultiLanguage>();

            if (isBeingUsed)
            {
                theIndicatorTextSwitcher.setIndex(1);
            }
            else
            {
                theIndicatorTextSwitcher.setIndex(0);
            }

        }



    }

}

