namespace Foxlair.Achievements
{
    /// <summary>
    /// Achievement for starting the tutorial.
    /// </summary>
    public class EagerLearner : Achievement
    {
        protected override void SubscribeToEvents()
        {
            // FoxlairEventManager.Instance.EnemyHealthSystem_OnDeath_Event += CheckIfKilledZombie;
        }

        protected override void UnsubscribeFromEvents()
        {
            // FoxlairEventManager.Instance.EnemyHealthSystem_OnDeath_Event -= CheckIfKilledZombie;
        }

        //private void CheckIfKilledZombie(EnemyCharacter enemyCharacter)
        //{
        //    if (enemyCharacter is ZombieCharacter)
        //    {
        //        Achieve();
        //    }
        //}
    }
}
