using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    using UnityEngine.UI;
    public class NewLoadingScreen : MonoBehaviour
    {
        [System.Flags]
        enum AnimationType
        {
            PointToPoint = 1,
            Rotation = 1 << 1,
        }

        [SerializeField] Canvas canvas;
        [SerializeField] Image background;
        [SerializeField] TMPro.TextMeshProUGUI textElement;
        [SerializeField] RectTransform animationObject;
        [SerializeField] RectTransform startPosition;
        [SerializeField] RectTransform endPosition;
        public float loadingPercentage = 0.0f;
        public bool isPlayingFadeAnimation = false;
        [SerializeField] AnimationType animationType = AnimationType.PointToPoint;

        public void PlayAnimationByPercentage(float percent)
        {
            Vector3 delta = endPosition.localPosition - startPosition.localPosition;
            animationObject.localPosition = Vector3.Lerp(startPosition.localPosition, endPosition.localPosition, percent);
            loadingPercentage = percent;
            if (textElement != null)
            {
                /// Le agergamos 0.10f dado que unity termina de cargar una ecena cuando llega al 0.90 (90%)
                /// No tengo idea porque hace esto pero bueno
                textElement.text = "Loading:\n" + (loadingPercentage + 0.10f).ToString("0.#%");
            }
        }

        public void PlayFadeOut(float time)
        {
            animationObject.gameObject.SetActive(false);

            float finalTime = Mathf.Clamp(time, 0.1f, 30.0f);

            StartCoroutine(FadeOutAnimation(finalTime));
        }

        [ContextMenu("text Play Animation")]
        private void textPlayAnimation()
        {
            PlayAnimationByPercentage(loadingPercentage);
        }

        private IEnumerator FadeOutAnimation(float time)
        {
            isPlayingFadeAnimation = true;
            bool completedFadeOut = false;
            float currentTime = 0.001f;
            const float hundredPercent = 100.0f;
            const float inverseOneHundred = 1.0f / 100.0f;
            while (!completedFadeOut)
            {
                float tranparecyPercentage = ((currentTime * hundredPercent) / time) * inverseOneHundred;
                Color bgColor = background.color;
                // if you remove the ' - 1.0f ' you would make it would be a FadeIn animation instead
                bgColor.a = Mathf.Abs(tranparecyPercentage - 1.0f);
                background.color = bgColor;
                currentTime += Time.fixedDeltaTime;
                if (currentTime >= time)
                {

                    completedFadeOut = true;
                }

                yield return new WaitForFixedUpdate();
            }
            isPlayingFadeAnimation = false;


        }


    }

}
