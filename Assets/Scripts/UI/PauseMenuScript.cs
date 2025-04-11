using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class PauseMenuScript : MonoBehaviour
{
    public void Restore()
    {
        GameManager.Instance.SetGameState(Utils.GameStates.Playing);
    }



    public void Options()
    {
        EDebug.Log("Options");
    }


    public void Quit()
    {
        EDebug.Log("Quit");
        SceneManager.LoadScene("Scenes/Nuevo Menu");
    }
}
