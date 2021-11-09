using SWS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foxlair.Enemies;
using Foxlair.Tools.Events;

namespace Foxlair.StageStructure
{
    public class EnemySpawner : MonoBehaviour
    {

        public EnemyWave enemyWave;
        public Vector2 spawnPosition;
        public GameObject enemyPrefab;
        public float spawnInterval;
        public int numberOfSpawns;
        public PathManager pathContainer;

        void Awake()
        {
            enemyWave = GetComponentInParent<EnemyWave>();
        }
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(SpawnEnemy());
        }

        // Update is called once per frame
        void Update()
        {

        }

        IEnumerator SpawnEnemy()
        {
            for (int i = 0; i < numberOfSpawns; i++)
            {
                GameObject enemy = Instantiate(enemyPrefab, spawnPosition, transform.rotation);
                EnemySpaceship enemyComponent = enemy.GetComponent<EnemySpaceship>();
                SplineMove splineFollower = enemy.GetComponent<SplineMove>();
                splineFollower.pathContainer = pathContainer;
                enemyWave.enemies.Add(enemyComponent);
                enemyComponent.enemyWave = enemyWave;
                //FoxlairEventManager.Instance.Enemy_OnBirth_Event(enemyComponent);
                yield return new WaitForSeconds(spawnInterval);
            }
            //yield return null;
        }
    }
}