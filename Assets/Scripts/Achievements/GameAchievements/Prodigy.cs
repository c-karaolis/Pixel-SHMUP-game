namespace Foxlair.Achievements
{
    /// <summary>
    /// Achievement for finishing the tutorial.
    /// </summary>
    public class Prodigy : Achievement
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
