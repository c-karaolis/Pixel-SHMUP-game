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
        int NumberOfWaves { get { return enemyWaves.Count; } }

        int currentWave = -1;
        public float waveDelay = 5f;

        void Start()
        {
            StartCoroutine(SpawnWave(true,0f));
        }

        void Update()
        {

        }

        IEnumerator SpawnWave(bool useCustomDelay = false,float delay = 0f)
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
    }
}