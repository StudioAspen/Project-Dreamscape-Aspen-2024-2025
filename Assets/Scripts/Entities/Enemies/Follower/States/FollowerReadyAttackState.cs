using UnityEngine;

[System.Serializable]
public class FollowerReadyAttackState : FollowerBaseState
{
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: SerializeField] public float AttackReadyDuration { get; private set; } = 0.5f;

    private float readyTimer;
    private float readyDuration;

    public override void OnEnter()
    {
        follower.PlayOneShotAnimation(AnimationClip, AttackReadyDuration);

        follower.SetSpeedModifier(0f);

        readyDuration = Random.Range(0.5f * AttackReadyDuration, 1.25f * AttackReadyDuration);
        readyTimer = 0f;
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        follower.ApplyGravity();

        readyTimer += follower.LocalDeltaTime;

        if (readyTimer > readyDuration)
        {
            follower.ChangeState(follower.FollowerAttackState);
            return;
        }
    }
}