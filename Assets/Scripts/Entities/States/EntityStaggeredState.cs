using System.Collections;
using UnityEngine;

[System.Serializable]
public class EntityStaggeredState : EntityBaseState
{
    [field: SerializeField] public AnimationClip AnimationClip { get; protected set; }
    [field: SerializeField] public float StaggerDuration { get; protected set; } = 0.5f;

    private protected float timer = 0f;

    public override void OnEnter()
    {
        entity.PlayOneShotAnimation(AnimationClip, StaggerDuration);

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

        if (timer > StaggerDuration)
        {
            entity.ChangeState(entity.DefaultState);
            return;
        }
    }
}
