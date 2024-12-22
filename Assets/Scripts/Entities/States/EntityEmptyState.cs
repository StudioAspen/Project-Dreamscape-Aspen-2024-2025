public class EntityEmptyState : EntityBaseState
{
    private Entity entity;

    public EntityEmptyState(Entity entity)
    {
        this.entity = entity;
    }

    public override void OnEnter()
    {
        entity.TransitionToAnimation("FlatMovement");
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {

    }

    public override void FixedUpdate()
    {

    }
}
