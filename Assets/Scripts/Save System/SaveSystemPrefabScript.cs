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
        public void SaveLevel()
        {
            int index = SaveSystem.CurrentSaveFileIndex;
            SaveSystem.SaveLevelData(index);
        }

        public void LoadLevel()
        {
            int index = SaveSystem.CurrentSaveFileIndex;
            SaveSystem.LoadPlayerAndLevel(index);
        }
    }

}

