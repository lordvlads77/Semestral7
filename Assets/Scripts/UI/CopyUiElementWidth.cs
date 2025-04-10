using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyUiElementWidth : MonoBehaviour
{
    [SerializeField] private RectTransform selfUiRectTransform;

    [SerializeField] private RectTransform otherUiRectTransform;


    private void Awake()
    {
        if (selfUiRectTransform == null)
        {
            selfUiRectTransform = GetComponent<RectTransform>();
        }
        Debug.Assert(selfUiRectTransform != null, $"The Script Need a {nameof(RectTransform)} to work ", this);
        Debug.Assert(otherUiRectTransform != null, "<color=orange>The variable otherUiRectTransform is null fix that</color>", this);
    }

    private void FixedUpdate()
    {
        float selfWidth = selfUiRectTransform.rect.width;
        float otherWidth = otherUiRectTransform.rect.width;

        if (selfWidth < otherWidth || selfWidth > otherWidth)
        {
            selfUiRectTransform = otherUiRectTransform;
        }

    }
}
