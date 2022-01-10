using Foxlair.Enemies;
using Foxlair.Interfaces;
using Foxlair.Tools.Events;
using SWS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foxlair.StageStructure
{

    public class BossWave : Wave
    {

        private List<BossSpaceship> enemies = new List<BossSpaceship>();
        public List<BossSpaceship> enemiesThatDied;
        public List<PathManager> pathContainers;
        bool isCleared = false;
        public GameObject enemyPrefab;
        public Transform shootingPosition;
        void Start()
        {
            FoxlairEventManager.Instance.EnemyHealthSystem_OnDeath_Event += OnEnemyDeath;
            StartCoroutine(SpawnEnemy(pathContainers[0]));
        }

        void OnDestroy()
        {
            FoxlairEventManager.Instance.EnemyHealthSystem_OnDeath_Event -= OnEnemyDeath;
        }

        private void OnEnemyDeath(IEnemy enemySpaceship, Wave enemyWave)
        {
            if (enemyWave != this)
                return;

            OnWaveCleared();
        }

        private void OnWaveCleared()
        {
            isCleared = true;
            FoxlairEventManager.Instance.EnemyWave_OnWaveCleared_Event?.Invoke(this);
            Destroy(gameObject, 3f);
        }

        IEnumerator SpawnEnemy(PathManager pathManager)
        {
            
                GameObject enemy = Instantiate(enemyPrefab, new Vector3(100, 100, 0), transform.rotation);
                BossSpaceship enemyComponent = enemy.GetComponent<BossSpaceship>();
                SplineMove splineFollower = enemy.GetComponent<SplineMove>();
                splineFollower.pathContainer = pathManager;
                enemyComponent.shootingPosition = shootingPosition;
            enemyComponent.splineMove = splineFollower;
                enemies.Add(enemyComponent);
                enemyComponent.enemyWave = this;
                //FoxlairEventManager.Instance.Enemy_OnBirth_Event(enemyComponent);
                yield return null;
        }

    }
}