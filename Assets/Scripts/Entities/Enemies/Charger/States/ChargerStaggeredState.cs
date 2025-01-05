using UnityEngine;

public class ChargerStaggeredState : EntityStaggeredStateSO
{
    private Charger charger;

    public ChargerStaggeredState(Charger enemy) : base(enemy)
    {
        charger = enemy;
    }

    public override void OnEnter()
    {
        charger.TransitionToAnimation("GetUp");

        charger.SetSpeedModifier(0f);

        timer = 0f;

        charger.UseRootMotion = true;
    }

    public override void OnExit()
    {
        charger.UseRootMotion = false;
    }

    public override void Update()
    {
        charger.ApplyGravity();

        timer += charger.LocalDeltaTime;

        if (timer > charger.StaggerDuration)
        {
            charger.ChangeState(charger.ChargerWanderState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        
    }
}