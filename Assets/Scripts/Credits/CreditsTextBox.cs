using System.Collections;
using System.Collections.Generic;

namespace Credits
{
    using UnityEngine;
    using TMPro;

    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class CreditsTextBox : MonoBehaviour
    {
        public TextMeshProUGUI textMeshPro;
        public RectTransform rectTransform;
        public float speed = 1.0f;
        public Vector3 direction = Vector3.up;
        public bool move = false;
        public float max_distance = 0.01f;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            textMeshPro = GetComponent<TextMeshProUGUI>();
            EDebug.Assert(textMeshPro != null, "", this);
            EDebug.Assert(rectTransform != null, "", this);
        }

        private void FixedUpdate()
        {
            if(!move) { return; }
            rectTransform.position += (direction * speed);
        }


        public void setDirection(Vector3 dir) { direction = dir; }

    }


}
