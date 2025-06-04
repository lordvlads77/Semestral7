using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public class HealingItemRespawner : MonoBehaviour
    {
        [SerializeField] private GameObject healingItem;
        [SerializeField] private float respawnTime = 25f;

        public void DeactivateAndRespawn()
        {
            StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            healingItem.SetActive(false);
            yield return new WaitForSeconds(respawnTime);
            healingItem.SetActive(true);
        }
    }
}