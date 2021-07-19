using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foxlair.Achievements
{
    public class Achievements : MonoBehaviour
    {
        public Achievement[] availableGameAchievements;

        private void Awake()
        {
            availableGameAchievements = gameObject.GetComponentsInChildren<Achievement>();
        }
        
    }
}