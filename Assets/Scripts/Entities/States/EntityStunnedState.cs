using UnityEngine;

[System.Serializable]
public class EntityStunnedState : EntityBaseState
{
    [field: SerializeField] public AnimationClip AnimationClip { get; protected set; }

    private protected float stunDuration;
    private protected float timer = 0f;

    public override void OnEnter()
    {
        entity.PlayOneShotAnimation(AnimationClip);

        timer = 0f;

        entity.SetSpeedModifier(0);
    }

    public override void OnExit()
    {
        entity.PlayDefaultAnimation();
    }

    public override void OnUpdate()
    {
        entity.ApplyGravity();

        timer += entity.LocalDeltaTime;

        if (timer > stunDuration)
        {
            entity.ChangeState(entity.DefaultState);
            return;
        }
    }

    public void StunEntity(Entity stunner, float duration)
    {
        stunDuration = duration;
        entity.ChangeState(entity.EntityStunnedState, true);

        entity.OnEntityStunned.Invoke(stunner, entity, duration);
        if(stunner != null) stunner.OnStunEntity.Invoke(stunner, entity, duration);
    }
}
