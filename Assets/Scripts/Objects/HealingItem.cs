using UnityEngine;
using Utils;

namespace Objects
{
    public class HealingItem : MonoBehaviour
    {
        [SerializeField] private float healAmount = 20f;
        [SerializeField] GameObject onHitPar;
        GameObject onHitP;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            LivingEntity player = other.GetComponent<LivingEntity>();
            if (player == null) return;
            player.Heal(healAmount);
            if (onHitP == null)
            {
                onHitP = Instantiate(onHitPar, transform.position, transform.rotation);
            }
            else
            {
                onHitP.transform.position = transform.position;
                onHitP.transform.rotation = transform.rotation;
                onHitP.SetActive(true);
            }
            Destroy(gameObject);
        }
    }
}
