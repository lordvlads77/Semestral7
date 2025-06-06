using System.Collections;
using UnityEngine;
using Utils;

namespace Objects
{
    public class HealingItem : MonoBehaviour
    {
        [SerializeField] private float healAmount = 20f;
        [SerializeField] private GameObject onHitPar;

        private GameObject onHitP;
        private HealingItemRespawner respawner;

        private void Start()
        {
            // Busca al respawner en el padre
            respawner = GetComponentInParent<HealingItemRespawner>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            LivingEntity player = other.GetComponent<LivingEntity>();
            if (player == null) return;

            player.Heal(healAmount);

            if (onHitP == null)
                onHitP = Instantiate(onHitPar, transform.position, transform.rotation);
            else
            {
                onHitP.transform.position = transform.position;
                onHitP.transform.rotation = transform.rotation;
                onHitP.SetActive(true);
            }

            // Pide al respawner que se encargue
            respawner.DeactivateAndRespawn();
        }
    }
}
