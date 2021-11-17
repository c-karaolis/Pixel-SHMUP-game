using DG.Tweening;
using Foxlair.Enemies;
using Foxlair.Tools.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FoxlairEventManager.Instance.EnemyHealthSystem_OnDeath_Event += OnEnemyDeath;
    }

    private void OnEnemyDeath(EnemySpaceship enemySpaceship,EnemyWave enemyWave)
    {
        transform.DOShakePosition(0.2f,.4f, 50, 90, false, true);
    }

    private void OnDestroy()
    {
        FoxlairEventManager.Instance.EnemyHealthSystem_OnDeath_Event -= OnEnemyDeath;
    }

}
