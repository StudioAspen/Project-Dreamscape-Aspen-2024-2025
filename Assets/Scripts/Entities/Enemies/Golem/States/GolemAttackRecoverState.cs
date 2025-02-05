using UnityEngine;

public class GolemAttackRecoverState : GolemBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public float AttackRecoverDuration { get; private set; } = 1f;

    private float recoverTimer;

    public override void OnEnter()
    {
        golem.SetSpeedModifier(0f);

        recoverTimer = 0f;
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        golem.ApplyGravity();

        recoverTimer += golem.LocalDeltaTime;

        if (recoverTimer > AttackRecoverDuration)
        {
            // golem.ChangeState(golem.DefaultState);
            golem.ChangeState(golem.GolemGroundSmashState); // For testing purposes only
            return;
        }
    }
}
