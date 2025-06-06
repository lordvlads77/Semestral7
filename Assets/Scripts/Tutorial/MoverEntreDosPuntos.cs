using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverEntreDosPuntos : MonoBehaviour
{
    public Transform puntoA;
    public Transform puntoB;
    public float velocidad = 2f;

    private Vector3 destinoActual;

    void Start()
    {
        if (puntoA != null)
            destinoActual = puntoA.position;
    }

    void Update()
    {
        if (puntoA == null || puntoB == null)
            return;

        transform.position = Vector3.MoveTowards(transform.position, destinoActual, velocidad * Time.deltaTime);

        if (Vector3.Distance(transform.position, destinoActual) < 0.01f)
        {
            destinoActual = destinoActual == puntoA.position ? puntoB.position : puntoA.position;
        }
    }
}
