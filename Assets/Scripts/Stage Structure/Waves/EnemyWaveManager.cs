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
            if (currentWave == numberOfWaves)
            {
                FoxlairEventManager.Instance.Stage_OnStageCleared_Event?.Invoke(stage, this);
                return;
            }
            else if(currentEnemyWave.GetComponent<Wave>() == enemyWave)
            {
                Invoke("SpawnWave", 5f);
            }
        }

        void SpawnWave()
        {

            if (currentWave < numberOfWaves)
            {
                Debug.Log("Current Wave: " + (currentWave + 1) + "/" + numberOfWaves);
                currentEnemyWave = Instantiate(enemyWaves[currentWave]);
                currentWave++;
                Debug.Log("currentWave incremented to: " + currentWave);
            }
        }

        public void ResetStage()
        {
            FoxlairEventManager.Instance.Stage_OnStageRestart_Event?.Invoke(stage, this);
            currentWave = 0;
            Invoke("SpawnWave", 5f);
        }

        void Start()
        {
            FoxlairEventManager.Instance.EnemyWave_OnWaveCleared_Event += OnEnemyWaveCleared;
            numberOfWaves = enemyWaves.Count;
            Invoke("SpawnWave", 2f);
        }
        private void OnDestroy()
        {
            FoxlairEventManager.Instance.EnemyWave_OnWaveCleared_Event -= OnEnemyWaveCleared;
        }

    }
}