using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Utils;

public class TutorialMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float fadeDuration = 0.5f;

    private Queue<TutorialMessageData> messageQueue = new();
    private HashSet<string> shownKeys = new(); // ← Aquí guardamos los ya mostrados
    private bool isDisplaying = false;

    void Awake()
    {
        messageText.alpha = 0f;
    }

    public void EnqueueMessage(string textKey, float duration)
    {
        if (shownKeys.Contains(textKey)) return; // ← Evita duplicados

        shownKeys.Add(textKey); // ← Lo marcamos como mostrado
        messageQueue.Enqueue(new TutorialMessageData(textKey, duration));

        if (!isDisplaying)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        while (messageQueue.Count > 0)
        {
            isDisplaying = true;

            var data = messageQueue.Dequeue();
            string translated = Localization.Translate(data.textKey);

            yield return StartCoroutine(ShowAndHide(translated, data.duration));
        }

        isDisplaying = false;
    }

    private IEnumerator ShowAndHide(string message, float duration)
    {
        messageText.text = message;
        yield return StartCoroutine(FadeText(0f, 1f, fadeDuration));
        yield return new WaitForSeconds(duration);
        yield return StartCoroutine(FadeText(1f, 0f, fadeDuration));
    }

    private IEnumerator FadeText(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            messageText.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        messageText.alpha = to;
    }
}
