using UnityEngine;

[System.Serializable]
public class GolemReadyAttackState : GolemBaseState
{
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: SerializeField] public float StompReadyDuration { get; private set; } = 2f;
    [field: SerializeField] public float GroundSmashReadyDuration { get; private set; } = 1f;
    [field: SerializeField] public float CloseRangeCutoff { get; private set; } = 2f;

    private float readyTimer;
    private float readyDuration;
    private Vector3 attackDirection;
    private float targetDistance;
    private bool closeRange;

    public override void OnEnter()
    {
        golem.SetSpeedModifier(0f);

        if (golem.Target != null)
        {
            targetDistance = Vector3.Distance(golem.Target.transform.position, golem.transform.position);    
        }
        
        closeRange = (golem.Target != null ? targetDistance <= CloseRangeCutoff : false);
        readyDuration = Random.Range(0.5f * (closeRange ? GroundSmashReadyDuration : StompReadyDuration), 1.25f * (closeRange ? GroundSmashReadyDuration : StompReadyDuration));
        readyTimer = 0f;

        enemy.PlayOneShotAnimation(AnimationClip, readyDuration);
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
        if (golem.Target == null) 
        {
            golem.ChangeState(golem.GolemWanderState);
            return;
        }
        targetDistance = Vector3.Distance(golem.Target.transform.position, golem.transform.position);
        closeRange = targetDistance <= CloseRangeCutoff;
        
        golem.ApplyGravity();
        
        readyTimer += golem.LocalDeltaTime;

        if (readyTimer > readyDuration)
        {
            attackDirection = golem.Target.transform.position - golem.transform.position;
            golem.ChangeState(closeRange ? golem.GolemGroundSmashState : golem.GolemStompState);
            return;
        }
    }

    public Vector3 GetAttackDirection()
    {
        return attackDirection;
    }
}