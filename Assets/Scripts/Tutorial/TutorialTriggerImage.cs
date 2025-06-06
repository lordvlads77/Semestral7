using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTriggerImage : MonoBehaviour
{
    [SerializeField] private Sprite tutorialImage;
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

        var messageSystem = FindObjectOfType<TutorialMessageImage>();
        if (messageSystem != null)
        {
            messageSystem.EnqueueMessage(tutorialImage, duration);
            alreadyTriggered = true;
        }
    }
}
