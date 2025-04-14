using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem
{
    /// <summary>
    /// Attach this to an object
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public sealed class SaveSystemPrefabScript : MonoBehaviour
    {
        [TextArea(minLines: 5, maxLines: 15), SerializeField]
        string Level_Data;
        public void SaveLevel()
        {
            SaveSystem.SaveLevelData();
        }

        public void LoadLevel()
        {
            SaveSystem.LoadLevelData();
        }
    }

}

