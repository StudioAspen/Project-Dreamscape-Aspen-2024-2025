using UnityEngine;

[System.Serializable]
public class GolemStaggeredState : EntityStaggeredState
{
    private Golem golem;

    public override void Init(Entity entity)
    {
        base.Init(entity);
        golem = entity as Golem;
    }

    public override void OnEnter()
    {
        golem.PlayOneShotAnimation(AnimationClip, StaggerDuration);
        golem.SetSpeedModifier(0f);
        timer = 0f;
        golem.UseRootMotion = true;
    }

    public override void OnExit()
    {
        golem.UseRootMotion = false;
    }

    public override void OnUpdate()
    {
        golem.ApplyGravity();
        timer += golem.LocalDeltaTime;

        if (timer > StaggerDuration)
        {
            golem.ResetDamageTakenSinceLastStagger();
            golem.ResetDamageTakenWhileStaggered();
            golem.ChangeState(golem.GolemWanderState);
            return;
        }
    }
}