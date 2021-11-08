using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foxlair.Enemies
{
    public class EnemyWaveManager : MonoBehaviour
    {
        public List<GameObject> enemyWaves;
        int NumberOfWaves { get { return enemyWaves.Count; } }

        public int currentWave = 0;
        public float waveDelay = 5f;

        void Start()
        {

        }

        void Update()
        {

        }
    }
}