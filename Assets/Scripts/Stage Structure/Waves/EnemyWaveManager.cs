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
        Stage stage;
       
        private void Awake()
        {
            stage = GetComponentInParent<Stage>();
        }
        EnemyWave EnemyWave
        {
            get
            {
                return currentEnemyWave.GetComponent<EnemyWave>();
            }
        }

        void OnEnemyWaveCleared(EnemyWave enemyWave)
        {
            Debug.Log("CLEARED");
            if(enemyWave == currentEnemyWave.GetComponent<EnemyWave>())
            {
                StartCoroutine(CleanupWave(enemyWave));
            }
            StartCoroutine(SpawnWave());
        }

        IEnumerator CleanupWave(EnemyWave enemyWave)
        {
            yield return new WaitForSeconds(.5f);

            Destroy(enemyWave.gameObject);
        }
        IEnumerator SpawnWave(bool useCustomDelay = false, float delay = 0f)
        {
            currentWave++;
            if (currentWave == numberOfWaves)
            {
                Debug.Log("stage cleared");
                FoxlairEventManager.Instance.Stage_OnStageCleared_Event?.Invoke(stage, this);
                yield break;
            }
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
            if (currentWave != numberOfWaves && currentWave != 0)
                yield return new WaitForSeconds(_delay);

            if (currentWave < numberOfWaves)
                currentEnemyWave = Instantiate(enemyWaves[currentWave]);
        }

        public void ResetStage()
        {
            FoxlairEventManager.Instance.Stage_OnStageRestart_Event?.Invoke(stage, this);
            currentWave = -1;
            StartCoroutine(SpawnWave());
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