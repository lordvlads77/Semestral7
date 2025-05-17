using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    using UnityEngine.UI;
    public sealed class TextSwitcher : MonoBehaviour
    {
        private Text selfText;
        [SerializeField] private string[] textsToSwitchInto;
        [field: Header("controls for TextSwitcher")]
        [SerializeField] public int currentIndex { get; private set; }
        [SerializeField] private int desiredIndex = 0;
        [SerializeField] public float indexChangeDaley = 0.5f;
        [SerializeField] public bool isInputBlocked { get; private set; } = false;

        [Header("Lister")]
        [field: SerializeField] public Action<string> textChanged;

        void Start()
        {
            selfText = GetComponent<Text>();
            EDebug.Assert(selfText != null, $"Poner el script con un Objecto que tiene el tipo {nameof(UnityEngine.UI.Text)}", this);
            EDebug.Assert(textsToSwitchInto.Length > 0, $"La variable {nameof(textsToSwitchInto)} no tiene textos, dale textos", this);
            currentIndex = 0;
            desiredIndex = 0;
            //InvokeRepeating(nameof(LazyUpdate), 0.2f, 1f);
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
            if (!isInputBlocked && currentIndex != desiredIndex)
            {
                UpdateText();
            }

        }

        #region MODIFIY_INDEX
        public void IncreaseIndex()
        {
            desiredIndex++;
            if (desiredIndex > textsToSwitchInto.Length - 1)
            {
                desiredIndex = 0;
            }
        }

        public void DecreaseIndex()
        {
            desiredIndex--;
            if (desiredIndex < 0)
            {
                desiredIndex = textsToSwitchInto.Length - 1;
            }
        }

        public void setIndex(int _new_index)
        {
            desiredIndex = Math.Clamp(_new_index, 0, textsToSwitchInto.Length);
        }
        #endregion

        private void UpdateText()
        {
            currentIndex = desiredIndex;
            selfText.text = textsToSwitchInto[currentIndex];
            textChanged?.Invoke(textsToSwitchInto[currentIndex]);
            StartCoroutine(blockIndexChange());
        }

        #region Coroutines

        IEnumerator blockIndexChange()
        {
            isInputBlocked = true;
            yield return new WaitForSeconds(indexChangeDaley);
            isInputBlocked = false; 
        }

        #endregion


        public string getCurrentString => textsToSwitchInto[currentIndex];
        public int indexCount => textsToSwitchInto.Length;

        public string getStringAtIndex(uint index)
        {
            string result = "";
            EDebug.Assert(index < textsToSwitchInto.Length,"El indice este fuera de rango", this);
            result = textsToSwitchInto[index];
            return result;
        }
    }

}
