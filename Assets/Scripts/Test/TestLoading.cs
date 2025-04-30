using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public sealed class TestLoading : MonoBehaviour
{
    [Tooltip("☑ = load the level selected\n☐ = DON'T load the level selected")]
    public bool shouldLoad = false;


    [SerializeField] private string sceneNameToLoad = "";
    [SerializeField] private int sceneIndexToLoad = -1337;

    void Update()
    {

        if (shouldLoad)
        {
            shouldLoad = false;
            LoadingManager.Instance.LoadSceneByName(sceneNameToLoad);
        }
        
    }
}
