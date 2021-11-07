using SWS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public Vector2 spawnPosition;
    public GameObject enemyPrefab;
    public float spawnInterval;
    public int numberOfSpawns;

    public PathManager pathContainer;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnEnemy()
    {
        for (int i = 0; i < numberOfSpawns; i++)
        {

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, transform.rotation);
            SplineMove splineFollower = enemy.GetComponent<SplineMove>();
            splineFollower.pathContainer = pathContainer;
            //splineFollower.spline = splineComputer;
            //splineFollower.wrapMode = SplineFollower.Wrap.PingPong;
            //splineFollower.follow = true;
            //splineFollower.followSpeed = 1.5f;
            //newSlot.AddComponent<Slot>();
            yield return new WaitForSeconds(spawnInterval);

        }
        //yield return null;
    }
}
