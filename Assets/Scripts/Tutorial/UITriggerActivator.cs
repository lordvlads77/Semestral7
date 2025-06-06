using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITriggerActivator : MonoBehaviour
{
    public CanvasGroup uiElemento; // El elemento de UI a mostrar
    public string tagActivador = "Player"; // Tag del objeto que activa la UI

    void Start()
    {
        if (uiElemento != null)
        {
            uiElemento.alpha = 0f; // Ocultar al inicio
            uiElemento.interactable = false;
            uiElemento.blocksRaycasts = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagActivador) && uiElemento != null)
        {
            uiElemento.alpha = 1f; // Mostrar
            uiElemento.interactable = true;
            uiElemento.blocksRaycasts = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagActivador) && uiElemento != null)
        {
            uiElemento.alpha = 0f; // Ocultar
            uiElemento.interactable = false;
            uiElemento.blocksRaycasts = false;
        }
    }
}
