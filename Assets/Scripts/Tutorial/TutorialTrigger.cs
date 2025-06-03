using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private string textKey;
    [SerializeField] private float duration = 3f;

    private bool alreadyTriggered = false;

    private bool ready = false;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f); // espera a que todo cargue
        ready = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!ready || alreadyTriggered || !other.CompareTag("Player")) return;

        var messageSystem = FindObjectOfType<TutorialMessage>();
        if (messageSystem != null)
        {
            messageSystem.EnqueueMessage(textKey, duration);
            alreadyTriggered = true;
        }
    }
}
