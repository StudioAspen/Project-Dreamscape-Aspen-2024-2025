using UnityEngine;

public class ChargerStaggeredState : EntityStaggeredState
{
    private Charger charger;

    public ChargerStaggeredState(Charger enemy) : base(enemy)
    {
        charger = enemy;
    }

    public override void OnEnter()
    {
        charger.DefaultTransitionToAnimation("GetUp");

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
        timer += Time.deltaTime;

        if (timer > charger.StaggerDuration)
        {
            charger.ChangeState(charger.ChargerWanderState);
            return;
        }
    }

    public override void FixedUpdate() { }
}