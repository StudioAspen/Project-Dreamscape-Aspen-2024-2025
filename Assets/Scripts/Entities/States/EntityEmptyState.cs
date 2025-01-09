public class EntityEmptyState : EntityBaseState
{
    public override void OnEnter()
    {
        entity.TransitionToAnimation("FlatMovement");
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        entity.ApplyGravity();
    }
}
