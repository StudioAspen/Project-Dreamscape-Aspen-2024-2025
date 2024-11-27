public class PlayerHitState : EntityHitState
{
    private Player player;

    public PlayerHitState(Player entity) : base(entity)
    {
        player = entity;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void Update()
    {
        base.Update();

        player.ApplyGravity();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
