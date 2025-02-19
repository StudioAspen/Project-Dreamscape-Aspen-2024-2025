using UnityEngine;

public class ChargerStaggeredState : EntityStaggeredState
{
    private Charger charger;

    private protected override void Init(Entity entity)
    {
        base.Init(entity);

        charger = entity as Charger;
    }

    public override void OnEnter()
    {
        charger.PlayOneShotAnimation(AnimationClip);

        charger.SetSpeedModifier(0f);

        timer = 0f;

        charger.UseRootMotion = true;
    }

    public override void OnExit()
    {
        charger.UseRootMotion = false;
    }

    public override void OnUpdate()
    {
        charger.ApplyGravity();

        timer += charger.LocalDeltaTime;

        if (timer > StaggerDuration)
        {
            charger.ChangeState(charger.ChargerWanderState);
            return;
        }
    }
}