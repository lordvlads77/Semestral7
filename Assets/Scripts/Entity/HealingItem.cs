using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Character;

public class HealingItem : MonoBehaviour
{
    public float healingAmount = 20f; // Cantidad de vida que restaura
    [SerializeField] GameObject onHitPar;
    GameObject onHitP;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que entra en el trigger es el jugador
        MovementManager player = other.GetComponent<MovementManager>();

        if (other.CompareTag("Player"))
        {
            DamageSys damageSys = player.GetComponent<DamageSys>();
            damageSys._life += (int)healingAmount;
        }

        if (player != null)
        {
            player.Heal(healingAmount); // Llama al método de curación
            Destroy(gameObject); // Destruye el objeto después de usarlo
        }
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
    }
}
