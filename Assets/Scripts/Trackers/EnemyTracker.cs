using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trackers
{
    using Entity;
    using Utils;
    public sealed class EnemyTracker : MonoBehaviour
    {
        [SerializeField] Enemy[] enemies;
        [SerializeField] float updateTime = 1.0f;
        public bool areEnemiesDestroyed { get; private set; } = false;


        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            enemies = GameObject.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            for (int i = 0; i < enemies.Length; i++)
            {
                EDebug.Log(StringUtils.AddColorToString("Getting all enemies in the scene", Color.cyan), this);
                enemies[i].DestroyAction += OnEnemyDestroy;
            }
            StartCoroutine(InfrequentUpdate());
        }


        private IEnumerator InfrequentUpdate()
        {
            while (enemies.Length > 0)
            {
                yield return new WaitForSeconds(updateTime);
            }

            areEnemiesDestroyed = true;

            if (areEnemiesDestroyed)
            {
                GameManager.Instance.SetGameState(GameStates.Won);
            }

            EDebug.Log(StringUtils.AddColorToString($"are all enemies destroyed = {areEnemiesDestroyed}", Color.cyan), this);
        }


        private void OnEnemyDestroy(Enemy _enemy)
        {
            EDebug.Log(StringUtils.AddColorToString($"{nameof(OnEnemyDestroy)} is called", Color.cyan), this);
            for (int i = enemies.Length - 1; i > -1; i--)
            {
                if (_enemy == enemies[i])
                {
                    EDebug.Log(StringUtils.AddColorToString($"Found the duplicate", Color.cyan), this);
                    _enemy.DestroyAction -= OnEnemyDestroy;
                    Array<Enemy>.RemoveAt(ref enemies, i);
                    break;
                }
            }
        }


    }

}
