using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Character;

public class MaxHealthBoost : MonoBehaviour
{
    [SerializeField] private float maxHealthIncrease = 20f; // Cantidad de vida máxima a aumentar

    private void OnTriggerEnter(Collider other)
    {
        MovementManager player = other.GetComponent<MovementManager>();

        if (player != null)
        {
            player.IncreaseMaxHealth(maxHealthIncrease);
            Destroy(gameObject); // Destruye el objeto después de recogerlo
        }
    }
}
