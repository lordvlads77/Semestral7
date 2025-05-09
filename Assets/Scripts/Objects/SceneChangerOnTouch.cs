using UnityEngine;
using UnityEngine.SceneManagement;

namespace Objects
{
    public class SceneChangerOnTouch : MonoBehaviour
    {
        [SerializeField] private string sceneToLoad;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}
