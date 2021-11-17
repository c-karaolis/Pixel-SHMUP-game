using Foxlair.Tools.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foxlair.StageStructure
{
    public class EnemyWaveManager : MonoBehaviour
    {
        public List<GameObject> enemyWaves;
        GameObject currentEnemyWave;
        int numberOfWaves;
        int currentWave = -1;
        public float waveDelay = 5f;

        EnemyWave EnemyWave
        {
            get
            {
                return currentEnemyWave.GetComponent<EnemyWave>();
            }
        }

        void OnEnemyWaveCleared(EnemyWave enemyWave)
        {
            Destroy(enemyWave.gameObject);
            StartCoroutine(SpawnWave());
        }

        IEnumerator SpawnWave(bool useCustomDelay = false, float delay = 0f)
        {
            float _delay;

            if (useCustomDelay)
            {
                _delay = delay;
            }
            else
            {
                _delay = waveDelay;
            }

            FoxlairEventManager.Instance.WaveManager_OnSpawnProcedureStarted_Event?.Invoke(_delay);
            currentEnemyWave = null;

            yield return new WaitForSeconds(_delay);

            currentEnemyWave = Instantiate(enemyWaves[currentWave + 1]);
        }
        void Start()
        {
            FoxlairEventManager.Instance.EnemyWave_OnWaveCleared_Event += OnEnemyWaveCleared;
            numberOfWaves = enemyWaves.Count;
            StartCoroutine(SpawnWave(true, 0f));
        }
        private void OnDestroy()
        {
            FoxlairEventManager.Instance.EnemyWave_OnWaveCleared_Event -= OnEnemyWaveCleared;
        }

    }
}