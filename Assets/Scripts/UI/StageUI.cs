using Foxlair.StageStructure;
using Foxlair.Tools.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foxlair.UI
{
    public class StageUI : MonoBehaviour
    {
        public GameObject stageClearedObject;



        public void EnableStageClearedUI(Stage stage,EnemyWaveManager enemyWaveManager)
        {
            stageClearedObject.SetActive(true);
        }

        public void DisableStageClearedUI(Stage stage, EnemyWaveManager enemyWaveManager)
        {
            stageClearedObject.SetActive(false);

        }
        // Start is called before the first frame update
        void Start()
        {
            stageClearedObject.SetActive(false);
            FoxlairEventManager.Instance.Stage_OnStageCleared_Event += EnableStageClearedUI;
            FoxlairEventManager.Instance.Stage_OnStageRestart_Event += DisableStageClearedUI;
        }

        private void OnDestroy()
        {
            FoxlairEventManager.Instance.Stage_OnStageCleared_Event -= EnableStageClearedUI;
            FoxlairEventManager.Instance.Stage_OnStageRestart_Event -= DisableStageClearedUI;

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}