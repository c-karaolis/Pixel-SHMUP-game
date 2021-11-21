using DG.Tweening;
using Foxlair.Enemies;
using Foxlair.Tools.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraShake : MonoBehaviour
{
    CinemachineImpulseSource impulseSource;
    // Start is called before the first frame update
    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }   
    void Start()
    {
        FoxlairEventManager.Instance.EnemyHealthSystem_OnDeath_Event += OnEnemyDeath;
    }

    private void OnEnemyDeath(EnemySpaceship enemySpaceship,EnemyFormationWave enemyWave)
    {
        //transform.DOShakePosition(0.2f,.4f, 50, 90, false, true);
        impulseSource.GenerateImpulse();
    }

    private void OnDestroy()
    {
        FoxlairEventManager.Instance.EnemyHealthSystem_OnDeath_Event -= OnEnemyDeath;
    }

}
