using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hace que el elemento con el script copie la anchura de otro elemento
/// <note>
/// Si no se asigna el otherUiRectTransform el script en efecto hace nada
/// </note>
/// </summary>
public sealed class CopyUiElementWidth : MonoBehaviour
{
    private RectTransform selfUiRectTransform;

    [Tooltip("Si no se asigna esta variable el script hace nada")]
    [SerializeField] private RectTransform otherUiRectTransform;

    private bool hasOtherUIRectTransformAssigned;

    private void Awake()
    {
        hasOtherUIRectTransformAssigned = false;

        if (selfUiRectTransform == null)
        {
            selfUiRectTransform = GetComponent<RectTransform>();
        }
        Debug.Assert(selfUiRectTransform != null, $"The Script Need a {nameof(RectTransform)} to work ", this);
        hasOtherUIRectTransformAssigned = otherUiRectTransform != null;
        //Debug.Assert(otherUiRectTransform != null, "<color=orange>The variable otherUiRectTransform is null fix that</color>", this);
    }

    private void FixedUpdate()
    {
        if (!hasOtherUIRectTransformAssigned) { return; }

        float selfWidth = selfUiRectTransform.rect.width;
        float otherWidth = otherUiRectTransform.rect.width;

        if (selfWidth < otherWidth || selfWidth > otherWidth)
        {
            Rect selfRect = selfUiRectTransform.rect;
            //selfRect.size = new Vector2(otherUiRectTransform.rect.width, selfRect.height);
            selfUiRectTransform.sizeDelta = new Vector2(otherUiRectTransform.rect.width, selfRect.height);

        }

    }
}
