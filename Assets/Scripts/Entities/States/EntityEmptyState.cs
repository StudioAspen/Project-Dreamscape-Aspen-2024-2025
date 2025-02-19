[System.Serializable]
public class EntityEmptyState : EntityBaseState
{
    public override void OnEnter()
    {
        entity.PlayDefaultAnimation();
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        entity.ApplyGravity();
    }
}
