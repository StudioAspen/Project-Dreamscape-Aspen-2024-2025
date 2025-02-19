public abstract class DreamBushBaseState : ObstacleBaseState
{
    private protected DreamBush dreamBush;

    private protected override void Init()
    {
        dreamBush = obstacle as DreamBush;
    }
}
