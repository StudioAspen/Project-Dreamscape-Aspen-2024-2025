using UnityEngine;

public class EntitySpawnState : EntityBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public float SpawnDuration { get; protected set; } = 1.5f;

    private protected float timer = 0f;

    public override void OnEnter()
    {
        entity.TransitionToAnimation("Spawn");

        timer = 0f;

        entity.SetSpeedModifier(0);
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        entity.ApplyGravity();

        timer += entity.LocalDeltaTime;

        if (timer > SpawnDuration)
        {
            entity.ChangeState(entity.DefaultState);
            return;
        }
    }
}
