using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyTracker
{
    public static HashSet<string> aliveEnemies = new HashSet<string>();

    public static void Register(string id)
    {
        if (!aliveEnemies.Contains(id))
            aliveEnemies.Add(id);
    }

    public static void Unregister(string id)
    {
        if (aliveEnemies.Contains(id))
            aliveEnemies.Remove(id);
    }

    public static void Clear()
    {
        aliveEnemies.Clear();
    }
}
