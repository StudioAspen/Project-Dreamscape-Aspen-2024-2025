using UnityEngine;

public class ChargerStaggeredState : EnemyBaseState
{
    private Charger charger;

    private float timer;

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

        if (timer > charger.StaggeredStateDuration)
        {
            charger.ChangeState(charger.ChargerWanderState);
            return;
        }
    }

    public override void FixedUpdate() { }
}