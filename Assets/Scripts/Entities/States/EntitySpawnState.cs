using System;
using UnityEngine;

public class EntitySpawnState : EntityBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public float SpawnDuration { get; protected set; } = 1.5f;

    private protected float timer = 0f;

    /// <summary>
    /// Action that is invoked when the entity finsihes spawning
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>Entity spawnedEntity</c>: The spawned entity.</description></item>
    /// </list>
    /// </remarks>
    public Action<Entity> OnEntityFinishSpawn = delegate { };

    public override void OnEnter()
    {
        entity.TransitionToAnimation("Spawn");

        timer = 0f;

        entity.SetSpeedModifier(0);
    }

    public override void OnExit()
    {
        OnEntityFinishSpawn?.Invoke(entity);
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
