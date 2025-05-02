using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class GameOverMenuScript : MonoBehaviour
    {
        public void Retry()
        {
            // recarga la ecena actuial
            // si no funciona ve a File -> Builds Settings
            // y agrega la Ecena actual
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }


        public void Options()
        {
            EDebug.Log($"<color=orange>{nameof(this.Options)} is not implemented yet </color>");
        }

        public void Menu()
        {

            EDebug.Log($"<color=orange>{nameof(this.Menu)} is not implemented yet </color>");
        }

        public void Quit()
        {
            EDebug.Log("Quit");
            SceneManager.LoadScene("Scenes/Nuevo Menu");
        }

    }

}
