using UnityEngine;
using UnityEngine.UI;
using Foxlair.Tools.Events;
using System;

namespace Foxlair.Achievements
{
    public class Achievement : MonoBehaviour
    {
        public AchievementType type;
        public Image image;
        public bool achieved;

        protected bool isSubscribedToEvents = false;
        public void Achieve()
        {
            if (achieved) { return; }
            achieved = true;
            FoxlairEventManager.Instance.Achievements_Achieved_Event?.Invoke(this);
            UnsubscribeInternal();
            Debug.Log($"Achievement Unlocked: {name}");
        }
        void Start()
        {
            if (!achieved)
            {
                SubscribeInternal();
               // FoxlairEventManager.Instance.EnemyHealthSystem_OnDeath_Event += CheckIfKilledZombie;
            }
        }
        void OnDestroy()
        {
            UnsubscribeInternal();
        }
        private void SubscribeInternal()
        {
            SubscribeToEvents();
            isSubscribedToEvents = true;
        }
        virtual protected void SubscribeToEvents()
        {
            Debug.LogWarning($"Achievement {this.name} is not overriding SubscribeToEvents");
        }
        private void UnsubscribeInternal()
        {
            if (isSubscribedToEvents)
            {
                UnsubscribeFromEvents();
                isSubscribedToEvents = false;
            }
        }
        virtual protected void UnsubscribeFromEvents()
        {
            Debug.LogWarning($"Achievement {this.name} is not overriding UnsubscribeFromEvents");
        }
    }

    public enum AchievementType
    {
        Progress,
        Goal
    }
}