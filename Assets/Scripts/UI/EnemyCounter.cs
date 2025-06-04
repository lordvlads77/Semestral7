using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Utils;

public class EnemyCounter : MonoBehaviour
{
    [Header("Lista manual de enemigos")]
    public LivingEntity[] enemies;

    [Header("Referencia al texto UI")]
    public Text enemyCounterText;

    private void Start()
    {
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
}
