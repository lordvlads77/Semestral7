using System.Collections;
using UnityEngine;
using Utils;

namespace HUD
{
    public class CamShaker : MonoBehaviour
    {
        public static CamShaker Instance;
        private float _shakeStr;
        private int _shakeFrames = -1;
        private Vector3 _initPos;
        private Coroutine _shakeCoroutine;
        
        private void Awake()
        {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            _shakeCoroutine = null;
        }

        public void ShakeIt(float str, int timeInFrames)
        {
            EDebug.Log(StringUtils.AddColorToString("We shakin' boys!", Color.blue));
            _initPos = transform.localPosition;
            _shakeStr = str;
            _shakeFrames = timeInFrames;
            if (_shakeCoroutine != null) StopCoroutine(_shakeCoroutine);
            _shakeCoroutine = StartCoroutine(ShakeCoroutine());
        }

        private IEnumerator ShakeCoroutine()
        {
            int framesRemaining = _shakeFrames;
            while (framesRemaining > 0) {
                transform.localPosition = _initPos + Random.insideUnitSphere * _shakeStr;
                framesRemaining--;
                yield return null;
            }
            transform.localPosition = _initPos;
            EDebug.Log(StringUtils.AddColorToString("We ain't shaking... boys.... :(", Color.blue));
        }
    }
}
