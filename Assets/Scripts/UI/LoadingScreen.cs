using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Loading loading;
    public void LoadingScreenIn()
    {
        loading.OnLoadingScreenSet();
    }

    public void LoadingScreenOut()
    {
        loading.OnLoadingScreenOff();
    }

}
