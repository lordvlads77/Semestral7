using System.Collections;
using System.Collections.Generic;
using Entity;
using UnityEngine;
using SaveSystem;
using Utils;

public class SaveCheckpointTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Asegúrate de que tu jugador tenga el tag "Player"
        {
            Vector3 playerPosition = other.transform.position;
            SavePlayerPosition(playerPosition);
            
            EnemyCounter counter = FindObjectOfType<EnemyCounter>();
            if (counter != null)
            {
                SaveEnemiesState(counter.enemies);
            }
            SaveTriggersState();
        }
    }

    private void SavePlayerPosition(Vector3 position)
    {
        PlayerPrefs.SetFloat("SavedX", position.x);
        PlayerPrefs.SetFloat("SavedY", position.y);
        PlayerPrefs.SetFloat("SavedZ", position.z);
        PlayerPrefs.Save();

        Debug.Log("Posición del jugador guardada: " + position);
    }
    
    private void SaveEnemiesState(LivingEntity[] enemies)
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy is Enemy enemyComponent)
            {
                PlayerPrefs.SetInt($"Enemy_{enemyComponent.enemyID}_IsDead", enemy.isDead ? 1 : 0);
            }
        }

        PlayerPrefs.Save();
    }
    private void SaveTriggersState()
    {
        TriggerOnConditions[] triggers = FindObjectsOfType<TriggerOnConditions>();

        foreach (var trigger in triggers)
        {
            if (!string.IsNullOrEmpty(trigger.triggerID))
            {
                PlayerPrefs.SetInt($"Trigger_{trigger.triggerID}_IsTriggered", trigger.GetTriggeredState() ? 1 : 0);
            }
        }

        PlayerPrefs.Save();
    }
}
