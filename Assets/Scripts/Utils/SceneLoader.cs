using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public sealed class SceneLoader : MonoBehaviour
    {
        public float fadeOutTime { get; set; } = 1.0f;

        public void LoadScene(string scene_name)
        {
            LoadingManager.Instance.LoadSceneByName(scene_name, fadeOutTime);
        }

        public void LoadScene(int scene_index)
        {
            LoadingManager.Instance.LoadSceneByIndex(scene_index, fadeOutTime);
        }
    }

}
