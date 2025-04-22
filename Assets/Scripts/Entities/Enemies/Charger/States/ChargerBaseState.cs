public abstract class ChargerBaseState : EnemyBaseState
{
    private protected Charger charger;

    public override void Init(Entity entity)
    {
        base.Init(entity);

        charger = enemy as Charger;
    }
}
