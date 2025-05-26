using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace HUD
{
    public class BarraVida : MonoBehaviour
    {

        public Image viada;

        [SerializeField] LivingEntity jugador;

        private void barrachange()
        {
            if (jugador != null)
            {
                float vidaActual = jugador.GetHealth();
                float maxVida = jugador.GetMaxHealth();
                viada.fillAmount = vidaActual / maxVida;
            }
        }
        void Update()
        {
            barrachange();
        }


    }
}
