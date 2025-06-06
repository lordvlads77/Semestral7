using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ResetProgressTrigger : MonoBehaviour
{
   // [SerializeField] private bool reloadScene = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Borrar todos los PlayerPrefs
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("Se borraron todos los PlayerPrefs.");
            
            
            // Recargar escena si está activado
          /*  if (reloadScene)
            {
                Scene currentScene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }*/
        }
    }
}
