
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StageData", menuName = "Foxlair Data/Stage Data", order = 1)]
public class StageData : ScriptableObject
{
    public int stage;
    public List<GameObject> enemyWavePrefabs;
    public float timeBetweenEnemyWaves;
}