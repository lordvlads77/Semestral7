using UnityEngine;
using UnityEngine.SceneManagement;

namespace Objects
{
    public class SceneChangerOnTouch : MonoBehaviour
    {
        [SerializeField] private string sceneToLoad;

        private void OnTriggerEnter(Collider other)
        {
            // Cambia "Player" por la tag del objeto que debe activar el cambio de escena
            if (other.CompareTag("Player"))
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}
