using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{

    /// <summary>
    /// This script adds a game Manager if None are in the scene
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class AddGameManagerIfNeeded : MonoBehaviour
    {
        [SerializeField] private GameObject managerPrefab;
        private void Awake()
        {
            GameManager gm = FindAnyObjectByType<GameManager>();
            if (gm == null)
            {
                Instantiate(managerPrefab);
            }

        }
    }
}
