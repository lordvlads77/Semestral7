using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utils;

public class SceneTextInt : MonoBehaviour
{
    [SerializeField] int value;
    [SerializeField] Utils.LivingEntity theObject;
    [SerializeField] TextMeshPro text;
    void Start()
    {
        theObject = GetComponentInParent<LivingEntity>();

        Debug.Assert(theObject != null, "Necesito un LivingEntity");

        text = GetComponent<TextMeshPro>();

        Debug.Assert(text != null, "Necesito un TextMeshPro");
    }

    void Update()
    {
        value = (int)theObject.GetHealth();
        text.text = value.ToString() ;
        
    }
}
