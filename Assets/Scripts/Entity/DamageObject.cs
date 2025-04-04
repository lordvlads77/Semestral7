using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Character;
using Utils;

public class DamageObject : MonoBehaviour
{
    [SerializeField] private float damageAmount = 20f; // Cantidad de daño
    [SerializeField] private float armorPiercing = 0f; // Penetración de armadura opcional

    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el jugador ha tocado el objeto dañino
        MovementManager player = other.GetComponent<MovementManager>();

        if (player != null)
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position); // Punto de impacto más cercano
            Vector3 hitDirection = (other.transform.position - transform.position).normalized; // Dirección del impacto

            player.TakeDamage(Vector3.up, Vector3.back, DamageType.Dark,DamageType.Electric,damageAmount,15,0,armorPiercing,0,0);
        }
    }
}
