using BulletPro;
using UnityEngine;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Foxlair.Tools.BulletProBridges
{
    [RequireComponent(typeof(BulletReceiver),typeof(HealthSystem))]
    public class BulletProToFoxlairHealthSystemBridge : MonoBehaviour
    {
        [Required][SerializeField] HealthSystem healthSystem;
        private void OnValidate()
        {
            healthSystem = GetComponent<HealthSystem>();
        }

        public void BulletCollisionToHealthDamage(Bullet bullet, Vector3 vector3)
        {
            healthSystem.TakeDamage(bullet.moduleParameters.GetFloat("_PowerLevel"));
        }
    }
}