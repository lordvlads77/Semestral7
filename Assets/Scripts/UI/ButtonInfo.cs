using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class ButtonInfo : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {

        [SerializeField] public UnityEngine.UI.Button button;

        public bool isPointerOverButton { get; private set; } = false;

        private void Awake()
        {
            button = GetComponent<UnityEngine.UI.Button>();
            EDebug.Assert(button != null, "Attach this script to a unity button");
        }

        public void OnPointerClick(PointerEventData eventData)
        {

        }

        public void OnPointerEnter(PointerEventData eventData)
        {

            isPointerOverButton = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {

            isPointerOverButton = false;
        }

    }

}
