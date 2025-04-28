using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO : EL SCRIPT NO SE BORRA CORRECTAMENTE POR EL MOMENTO
/// </summary>
public sealed class LoadingScreen : MonoBehaviour
{
    public Loading loading;


    public void OnEnable()
    {
        //DontDestroyOnLoad(gameObject);
    }

    public void LoadingScreenIn()
    {
        loading.OnEndChangeScene += OnSceneLoadEnd;
        loading.OnLoadingScreenSet();
    }

    public void LoadingScreenOut()
    {
        EDebug.Log($"<color=red> called {nameof(LoadingScreenOut)} </color>", this);
        loading.OnLoadingScreenOff();
        //Destroy(gameObject);
    }

    private void OnSceneLoadEnd()
    {
        loading.OnEndChangeScene -= OnSceneLoadEnd;
        LoadingScreenOut();
    }


}
