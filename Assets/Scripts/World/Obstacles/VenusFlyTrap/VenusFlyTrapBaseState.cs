public abstract class VenusFlyTrapBaseState : ObstacleBaseState
{
    private protected VenusFlyTrap venusFlyTrap;

    private protected override void Init()
    {
        venusFlyTrap = obstacle as VenusFlyTrap;
    }
}