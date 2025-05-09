using UnityEngine;
using UnityEngine.SceneManagement;

namespace Objects
{
    public class SceneChangerOnTouch : MonoBehaviour
    {
        
        if (other.CompareTag("Player"))
        {
            // Cambia "Player" por la tag del objeto que debe activar el cambio de escena
            if (other.CompareTag("Player"))
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}
