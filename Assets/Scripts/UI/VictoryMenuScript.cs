using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{

    public class VictoryMenuScript : MonoBehaviour
    {
        public void Cargar()
        {
            EDebug.Log("<color=orange> CARGAR </color>");
        }

        public void Opciones()
        {
            EDebug.Log("<color=orange> Opciones </color>");
        }

        public void Menu()
        {
            SceneManager.LoadScene("Scenes/Nuevo Menu");
        }

    }

}

