using System.Collections;
using System.Collections.Generic;
using FMOD;
using UnityEngine;

namespace Utils
{
    [DefaultExecutionOrder(1)]
    public class SpawnPlayPersistent : MonoBehaviour
    {
        [SerializeField] PlayPersistent playPersistent;
        // Start is called before the first frame update
        void Start()
        {
            Instantiate(playPersistent);
        }
    }

}
