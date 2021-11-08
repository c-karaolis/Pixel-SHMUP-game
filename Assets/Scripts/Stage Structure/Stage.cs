using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foxlair.Enemies
{
    public class Stage : MonoBehaviour
    {
        public StageData stageData;
        EnemyWaveManager enemyWaveManager;

        void Awake()
        {
            enemyWaveManager = GetComponentInChildren<EnemyWaveManager>();
            enemyWaveManager.enemyWaves = stageData.enemyWavePrefabs;
        }

        //TODO: create a stageData SO for keeping stage data and waves, also see if possible to create a wave SO for easily creating waves?
        public void StartStage()
        {
            // enemyWaveManager.StartWave();
        }

        public void StopStage()
        {
            // enemyWaveManager.StopWave();
        }

        public void PauseStage()
        {
            // enemyWaveManager.PauseWave();
        }

        public void UnpauseStage()
        {
            //enemyWaveManager.UnpauseWave();
        }

        public void RestartStage()
        {
            // enemyWaveManager.RestartWave();
        }

        public void EndStage()
        {
            // enemyWaveManager.EndWave();
        }

        public void SetStage(StageData stageData)
        {
        }

        void Start()
        {
            StartStage();
        }

    }
}