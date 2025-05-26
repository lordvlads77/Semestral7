using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

/// <summary>
/// Give IDs to living entities
/// </summary>
public sealed class IDManager : RegulatorSingleton<IDManager>
{

    private long _id = 0;

    protected override void OnAwake()
    {
        base.OnAwake();
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    public long GetNewID()
    {
        _id += 1;
        return _id;
    }



    #region SCENE_CHANGE_CODE
    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode load_mode)
    {
        _id = 0;
    }
    #endregion

    private void OnApplicationQuit()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

}
