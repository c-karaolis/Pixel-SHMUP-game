using Foxlair.Enemies;
using Foxlair.Tools.Events;
using SWS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foxlair.StageStructure
{

    public class BossWave : Wave
    {

        public List<BossSpaceship> enemiesThatDied;
        public List<PathManager> pathContainers;
        bool isCleared = false;


        void Start()
        {
            FoxlairEventManager.Instance.EnemyHealthSystem_OnDeath_Event += OnEnemyDeath;
        }

        void OnDestroy()
        {
            FoxlairEventManager.Instance.EnemyHealthSystem_OnDeath_Event -= OnEnemyDeath;
        }

        private void OnEnemyDeath(EnemySpaceship enemySpaceship, EnemyFormationWave enemyWave)
        {
            OnWaveCleared();
        }

        private void OnWaveCleared()
        {
            isCleared = true;
            FoxlairEventManager.Instance.EnemyWave_OnWaveCleared_Event?.Invoke(this);
        }
    }
}