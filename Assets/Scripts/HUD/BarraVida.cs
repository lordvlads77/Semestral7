using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class BarraVida : MonoBehaviour
{
    public Image viada; // Imagen de la barra de vida
    public Image fondoBarra; // Fondo de la barra de vida
    [SerializeField] private LivingEntity jugador;

    [SerializeField] private DamageSys jugadorDamageSys = default;

    private float vidaInicial;
    private Vector2 barraOriginalSize;
    private Vector2 fondoOriginalSize;

    private void Start()
    {
        Debug.Assert(jugadorDamageSys != default, "Necesitamos el sistema de daño del enemigo", this);
        if (jugador != null)
        {
            vidaInicial = jugadorDamageSys._life; //jugador.GetMaxHealth();
            barraOriginalSize = viada.rectTransform.sizeDelta; // Tamaño original de la barra
            fondoOriginalSize = fondoBarra.rectTransform.sizeDelta; // Tamaño original del fondo
        }
    }

    void Update()
    {
        if (jugador != null)
        {
            float vidaActual = jugadorDamageSys._life;//jugador.GetHealth();
            float maxVida = jugador.GetMaxHealth();

            // Calcular la escala de crecimiento
            float escala = maxVida / vidaInicial;

            // Expandir el fondo hacia la derecha
            fondoBarra.rectTransform.sizeDelta = new Vector2(fondoOriginalSize.x * escala, fondoOriginalSize.y);

            // Mantener la barra dentro del fondo (sin moverla)
            viada.rectTransform.sizeDelta = new Vector2(barraOriginalSize.x * escala, barraOriginalSize.y);
            viada.rectTransform.anchoredPosition = new Vector2(0, viada.rectTransform.anchoredPosition.y);

            // Ajustar el fillAmount para la vida actual
            viada.fillAmount = vidaActual / maxVida;
        }
    }
}
