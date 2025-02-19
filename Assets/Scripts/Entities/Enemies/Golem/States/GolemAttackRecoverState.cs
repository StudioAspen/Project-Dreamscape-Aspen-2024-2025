using UnityEngine;

[System.Serializable]
public class GolemAttackRecoverState : GolemBaseState
{
    [field: SerializeField] public float AttackRecoverDuration { get; private set; } = 1f;

    private float recoverTimer;

    public override void OnEnter()
    {
        golem.PlayDefaultAnimation();
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
            golem.ChangeState(golem.GolemWanderState);
            return;
        }
    }
}
