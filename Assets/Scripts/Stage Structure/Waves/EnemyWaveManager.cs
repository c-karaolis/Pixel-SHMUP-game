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
        int currentWave = 0;
        public float waveDelay = 5f;
        Stage stage;
       
        private void Awake()
        {
            stage = GetComponentInParent<Stage>();
        }

        void OnEnemyWaveCleared(Wave enemyWave)
        {
                StartCoroutine(SpawnWave());
        }

        IEnumerator SpawnWave(float delay = 5f)
        {
            if (currentWave == numberOfWaves)
            {
                Debug.Log("stage cleared");
                FoxlairEventManager.Instance.Stage_OnStageCleared_Event?.Invoke(stage, this);
                yield break;
            }
            Debug.Log("Current Wave: " + (currentWave + 1) + "/" + numberOfWaves);

            //FoxlairEventManager.Instance.WaveManager_OnSpawnProcedureStarted_Event?.Invoke(delay);

            //if (currentWave != numberOfWaves)

            if (currentWave < numberOfWaves)
            {
                yield return new WaitForSeconds(delay);

                currentEnemyWave = Instantiate(enemyWaves[currentWave]);
                currentWave++;
                Debug.Log("currentWave incremented to: " + currentWave);
            }

        }

        public void ResetStage()
        {
            FoxlairEventManager.Instance.Stage_OnStageRestart_Event?.Invoke(stage, this);
            currentWave = 0;
            StartCoroutine(SpawnWave());
        }

        void Start()
        {
            FoxlairEventManager.Instance.EnemyWave_OnWaveCleared_Event += OnEnemyWaveCleared;
            numberOfWaves = enemyWaves.Count;
            StartCoroutine(SpawnWave(1f));
        }
        private void OnDestroy()
        {
            FoxlairEventManager.Instance.EnemyWave_OnWaveCleared_Event -= OnEnemyWaveCleared;
        }

    }
}