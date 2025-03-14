using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class BarraVida : MonoBehaviour
{

    public Image viada;

    [SerializeField] LivingEntity jugador;
    


    void Update()
    {
        if (jugador != null)
        {
            float vidaActual = jugador.GetHealth();
            float maxVida = jugador.GetMaxHealth();
            viada.fillAmount = vidaActual / maxVida;
        }
    }
}
