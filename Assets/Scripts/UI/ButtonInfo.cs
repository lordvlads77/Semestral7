using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    /// <summary>
    /// Esta clase solo es para extarer informacion de el 
    /// Boton de unity (no el de textMeshPro)
    /// </summary>
    public sealed class ButtonInfo : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {

        [SerializeField] public UnityEngine.UI.Button button;

        public bool isPointerOverButton { get; private set; } = false;

        public bool hasBeenClickedEver { get; private set; } = false;

        private const float fiveSeconds = 5.0f;

        private const float oneSecond = 1.0f;

        public float currentTimeSinceLastClick { get; private set; } = 0.0f;

        private void Awake()
        {
            button = GetComponent<UnityEngine.UI.Button>();
            EDebug.Assert(button != null, "Attach this script to a unity button");
        }

        private void Update()
        {
            if (!hasBeenClickedEver) { return; }

            currentTimeSinceLastClick += Time.deltaTime;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            hasBeenClickedEver = true;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {

            isPointerOverButton = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {

            isPointerOverButton = false;
        }

        public bool hasButtonBeenClickFiveSecondsAgo()
        {
            return currentTimeSinceLastClick >= fiveSeconds;
        }

        public bool hasButtonBeenClickOneSecondsAgo()
        {
            return currentTimeSinceLastClick >= oneSecond;
        }

    }

}
