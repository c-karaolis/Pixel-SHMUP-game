using Foxlair.Enemies;
using Foxlair.Tools.Events;
using SWS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWave : MonoBehaviour
{
    public Formation formation;

    public List<EnemySpaceship> enemies;
    public int numberOfEnemies = 0;
    public List<EnemySpaceship> enemiesThatDied;
    public List<PathManager> pathContainers;
    public float spawnInterval;
    public GameObject enemyPrefab;
    public Vector2 spawnPosition;

    public int numberOfSpawns;
    int spawnsPerPath;


    void Start()
    {
        FoxlairEventManager.Instance.EnemyHealthSystem_OnDeath_Event += OnEnemyDeath;
        spawnsPerPath = numberOfSpawns / pathContainers.Count;

        foreach (PathManager pathManager in pathContainers)
        {
            StartCoroutine(SpawnEnemy(pathManager));
        }
    }

    void OnDestroy()
    {
        FoxlairEventManager.Instance.EnemyHealthSystem_OnDeath_Event -= OnEnemyDeath;
    }

    private void OnEnemyDeath(EnemySpaceship enemySpaceship,EnemyWave enemyWave)
    {

        if (enemyWave != this)
        {
            return;
        }

        if(!enemiesThatDied.Contains(enemySpaceship))
        enemiesThatDied.Add(enemySpaceship);


        enemies.Remove(enemySpaceship);
        if (enemiesThatDied.Count == numberOfSpawns)
        {
            OnWaveCleared();
        }
    }
    IEnumerator SpawnEnemy(PathManager pathManager)
    {
        
            for (int i = 0; i < spawnsPerPath; i++)
            {
                GameObject enemy = Instantiate(enemyPrefab, spawnPosition, transform.rotation);
                EnemySpaceship enemyComponent = enemy.GetComponent<EnemySpaceship>();
                SplineMove splineFollower = enemy.GetComponent<SplineMove>();
                splineFollower.pathContainer = pathManager;
                enemies.Add(enemyComponent);
                enemyComponent.enemyWave = this;
                //FoxlairEventManager.Instance.Enemy_OnBirth_Event(enemyComponent);
                yield return new WaitForSeconds(spawnInterval);
            }        
        //yield return null;
    }

    private void OnWaveCleared()
    {
        FoxlairEventManager.Instance.EnemyWave_OnWaveCleared_Event?.Invoke(this);
    }
}
