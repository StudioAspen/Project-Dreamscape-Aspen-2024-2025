using UnityEngine;

[System.Serializable]
public class FollowerAttackRecoverState : FollowerBaseState
{
    [field: SerializeField] public float AttackRecoverDuration { get; private set; } = 1f;

    private float recoverTimer;

    public override void OnEnter()
    {
        follower.SetSpeedModifier(0f);

        recoverTimer = 0f;
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        follower.ApplyGravity();

        recoverTimer += follower.LocalDeltaTime;

        if (recoverTimer > AttackRecoverDuration)
        {
            follower.ChangeState(follower.DefaultState);
            return;
        }
    }
}
