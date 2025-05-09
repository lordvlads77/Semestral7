using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    using System;
    using UnityEngine.UI;
    using Scriptables;
    using TMPro;

    public class TextSwitcherMultiLanguage : MonoBehaviour
    {

        private TextMeshProUGUI selfText;
        [SerializeField] TextSwitcherMultiLanguageContainer textContainer;

        [field: Header("controls for TextSwitcher")]

        [SerializeField] TextSwitcherLanguageContainer textToSwitchInto;
        [SerializeField] public int currentIndex { get; private set; }
        [SerializeField] private int desiredIndex;
        [SerializeField] public float indexChangeDaley = 0.5f;
        [SerializeField] public bool isInputBlocked { get; private set; } = false;

        [SerializeField] public Utils.Language currentLanguage;
        public Utils.Language desiredLanguage;


        [Header("Lister")]
        [field: SerializeField] public Action<string> textChanged;

        void Start()
        {
            selfText = GetComponent<TextMeshProUGUI>();
            EDebug.Assert(selfText != null, $"Poner el script con un Objecto que tiene el tipo {nameof(TextMeshProUGUI)}", this);
            EDebug.Assert(textContainer != null, $"La variable {nameof(textContainer)} necesita un instancia de {typeof(TextSwitcherMultiLanguageContainer)}", this);
            currentIndex = 0;
            desiredIndex = 0;
            textToSwitchInto = textContainer.findLanguageContainer(currentLanguage);
        }

        private void FixedUpdate()
        {
            ManualUpdate();
        }

        /// <summary>
        /// Esta funcion existe por si se necesita actualizar el textSwitcher de forma manual
        /// </summary>
        public void ManualUpdate()
        {
            if (currentLanguage != desiredLanguage)
            {
                UpdateLanguage();
            }


            if (!isInputBlocked && currentIndex != desiredIndex)
            {
                UpdateText();
            }
        }

        private void UpdateText()
        {
            currentIndex = desiredIndex;
            selfText.text = textToSwitchInto.text[currentIndex];
            textChanged?.Invoke(textToSwitchInto.text[currentIndex]);
            StartCoroutine(blockIndexChange());
        }

        private void UpdateLanguage()
        {
            currentLanguage = desiredLanguage;
            textToSwitchInto = textContainer.findLanguageContainer(currentLanguage);
            UpdateText();
        }

        #region LANGUAGE_MANAGER_BOLIER_PLATE

        private void OnEnable()
        {
            LanguageManager.Instance.Subscribe(OnLanguageChange);
            OnLanguageChange(LanguageManager.Instance.currentLanguage);
        }

        private void OnDisable()
        {
            LanguageManager.TryGetInstance()?.UnSubscribe(OnLanguageChange);
        }

        private void OnLanguageChange(Utils.Language newLanguage)
        {
            desiredLanguage = newLanguage;
        }

        #endregion

        #region MODIFIY_INDEX
        public void IncreaseIndex()
        {
            desiredIndex++;
            if (desiredIndex > textToSwitchInto.text.Length - 1)
            {
                desiredIndex = 0;
            }
        }

        public void DecreaseIndex()
        {
            desiredIndex--;
            if (desiredIndex < 0)
            {
                desiredIndex = textToSwitchInto.text.Length - 1;
            }
        }

        public void setIndex(int _new_index)
        {
            desiredIndex = Math.Clamp(_new_index, 0, textToSwitchInto.text.Length);
        }
        #endregion


        #region Coroutines

        IEnumerator blockIndexChange()
        {
            isInputBlocked = true;
            yield return new WaitForSeconds(indexChangeDaley);
            isInputBlocked = false;
        }

        #endregion


        public string getCurrentString => textToSwitchInto.text[currentIndex];

        public int indexCount => textToSwitchInto.text.Length;

    }

}

