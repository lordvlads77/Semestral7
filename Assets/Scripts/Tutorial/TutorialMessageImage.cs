using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMessageImage : MonoBehaviour
{
    [SerializeField] private Image tutorialImageUI;

    public void EnqueueMessage(Sprite image, float duration)
    {
        StartCoroutine(ShowImage(image, duration));
    }

    private IEnumerator ShowImage(Sprite image, float duration)
    {
        tutorialImageUI.sprite = image;
        tutorialImageUI.enabled = true;

        yield return new WaitForSeconds(duration);

        tutorialImageUI.enabled = false;
    }
}
