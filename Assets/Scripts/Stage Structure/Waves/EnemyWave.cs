using Foxlair.Enemies;
using Foxlair.Tools.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWave : MonoBehaviour
{
    public Formation formation;

    public List<EnemySpaceship> enemies;
    public int NumberOfEnemies { get { return enemies.Count; } }


    void Start()
    {
        FoxlairEventManager.Instance.EnemyHealthSystem_OnDeath_Event += OnEnemyDeath;
    }

    void OnDestroy()
    {
        FoxlairEventManager.Instance.EnemyHealthSystem_OnDeath_Event -= OnEnemyDeath;
    }

    private void OnEnemyDeath(EnemySpaceship enemySpaceship)
    {
        enemies.Remove(enemySpaceship);
        if (enemies.Count == 0)
            OnWaveCleared();
    }

    private void OnWaveCleared()
    {
        FoxlairEventManager.Instance.EnemyWave_OnWaveCleared_Event?.Invoke(this);
    }
}
