public class ChargerBaseState : EnemyBaseState
{
    private protected Charger charger;

    private protected override void Init(Entity entity)
    {
        base.Init(entity);

        charger = enemy as Charger;
    }
}
