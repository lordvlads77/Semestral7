using UnityEngine;
using Utils;

namespace Objects
{
    public class HealingItem : MonoBehaviour
    {
        [SerializeField] private float healAmount = 20f;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            LivingEntity player = other.GetComponent<LivingEntity>();
            if (player == null) return;
            player.Heal(healAmount);
            Destroy(gameObject);
        }
    }
}
