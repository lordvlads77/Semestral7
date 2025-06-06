using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Entity;
using Utils;

public class EnemyCounter : MonoBehaviour
{
    [Header("Lista manual de enemigos")] public LivingEntity[] enemies;

    [Header("Referencia al texto UI")] public Text enemyCounterText;

    private void Start()
    {
        LoadTriggersState();
        LoadEnemiesState();
        UpdateUI();
    }

    private void Update()
    {
        // Filtra los enemigos vivos
        int aliveCount = enemies.Count(e => e != null && !e.isDead);
        UpdateUI(aliveCount);
    }

    private void UpdateUI(int count = -1)
    {
        if (count < 0)
            count = enemies.Count(e => e != null && !e.isDead);

        enemyCounterText.text = $"{count}";
    }

    private void LoadEnemiesState()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy is Enemy enemyComponent)
            {
                string key = $"Enemy_{enemyComponent.enemyID}_IsDead";
                if (PlayerPrefs.HasKey(key) && PlayerPrefs.GetInt(key) == 1)
                {
                    Destroy(enemyComponent.gameObject); // Evita que reviva
                }
            }
        }
    }
    private void LoadTriggersState()
    {
        TriggerOnConditions[] triggers = FindObjectsOfType<TriggerOnConditions>();

        foreach (var trigger in triggers)
        {
            if (!string.IsNullOrEmpty(trigger.triggerID))
            {
                int triggeredValue = PlayerPrefs.GetInt($"Trigger_{trigger.triggerID}_IsTriggered", 0);
                bool isTriggered = triggeredValue == 1;
                trigger.SetTriggered(isTriggered);
            }
        }
    }
}
