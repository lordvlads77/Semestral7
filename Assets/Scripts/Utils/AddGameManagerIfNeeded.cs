using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{

    /// <summary>
    /// This script adds a game Manager if None are in the scene
    /// </summary>
    [DefaultExecutionOrder(-25)]
    public class AddGameManagerIfNeeded : MonoBehaviour
    {
        private Coroutine _coroutine = null;
        
        [SerializeField] private GameObject managerPrefab;
        private void Awake()
        {
            /*GameManager gm = FindAnyObjectByType<GameManager>();
            if (gm == null)
            {
                Instantiate(managerPrefab);
            }*/
            _coroutine??= StartCoroutine(WaitForGameManager());
        }

        private IEnumerator WaitForGameManager()
        {
            yield return null;
            int i = 0;
            while (!GameManager.Instance && i < 60)
            {
                i++;
                yield return new WaitForSeconds(0.5f);
            }
            Instantiate(managerPrefab);
        }
    }
}
