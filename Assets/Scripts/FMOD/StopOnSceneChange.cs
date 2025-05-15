using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace FMOD
{
    public class StopOnSceneChange : MonoBehaviour
    {
        [SerializeField] PlayPersistent playPersistent;

        private void Awake()
        {
            if (playPersistent == null)
            {
                playPersistent = gameObject.GetComponent<PlayPersistent>();
            }
            EDebug.Assert(playPersistent != null, $"This script requires a {typeof(PlayPersistent)} to work ", this);

            SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
        }

        private void SceneManager_sceneUnloaded(Scene arg0)
        {
            playPersistent.StopEvent();
            SceneManager.sceneUnloaded -= SceneManager_sceneUnloaded;
            //throw new global::System.NotImplementedException();
        }


    }

}

