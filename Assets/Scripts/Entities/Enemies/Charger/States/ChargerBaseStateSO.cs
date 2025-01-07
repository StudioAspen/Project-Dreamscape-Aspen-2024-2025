public class ChargerBaseStateSO : EnemyBaseStateSO
{
    private protected Charger charger;

    private protected override void Init(Entity entity)
    {
        base.Init(entity);
        charger = entity as Charger;
    }
}
