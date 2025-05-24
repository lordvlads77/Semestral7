using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    using SaveSystem;
    [RequireComponent(typeof(BoxCollider))]
    public sealed class SavePoint : MonoBehaviour
    {
        BoxCollider selfCoollider;
        private void Awake()
        {
            selfCoollider = GetComponent<BoxCollider>();
            if (!selfCoollider.isTrigger)
            {
                selfCoollider.isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                int index = SaveSystem.CurrentSaveFileIndex;
                SaveSystem.SaveLevelData(index);

                EDebug.Log(Utils.StringUtils.AddColorToString("Se Guardo la partida", Color.cyan), this);
            }
        }


    }
}

