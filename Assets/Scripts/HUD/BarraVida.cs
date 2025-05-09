using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace HUD
{
    public class BarraVida : MonoBehaviour
    {
        public Image healthImage;
        public Image healthBackgroundImg;
        [SerializeField] private LivingEntity player;
    
        private float _initHealth;
        private Vector2 _healthImgOgSize;
        private Vector2 _healthBgImgOgSize;

        private void Start()
        {
            if (player == null) return;
            _initHealth = player.GetHealth();
            _healthImgOgSize = healthImage.rectTransform.sizeDelta;
            _healthBgImgOgSize = healthBackgroundImg.rectTransform.sizeDelta;
        }

        private void LateUpdate()
        {
            if (player == null) return;
            float maxHealth = player.GetMaxHealth();
            float scale = maxHealth / _initHealth;
            healthBackgroundImg.rectTransform.sizeDelta = new Vector2(_healthBgImgOgSize.x * scale, _healthBgImgOgSize.y);
            healthImage.rectTransform.sizeDelta = new Vector2(_healthImgOgSize.x * scale, _healthImgOgSize.y);
            healthImage.rectTransform.anchoredPosition = new Vector2(0, healthImage.rectTransform.anchoredPosition.y);
        }
    }
}
