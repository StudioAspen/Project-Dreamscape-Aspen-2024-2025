public abstract class GeyserBaseState : ObstacleBaseState
{
    private protected Geyser geyser;

    private protected override void Init()
    {
        geyser = obstacle as Geyser;
    }
}
