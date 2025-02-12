using UnityEngine;

public class GolemReadyAttackState : GolemBaseState
{
    [field: Header("Config")]
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
        enemy.TransitionToAnimation("AttackWarning");
        golem.SetSpeedModifier(0f);

        if (golem.Target != null) {
            targetDistance = Vector3.Distance(golem.Target.transform.position, golem.transform.position);    
        }
        
        closeRange = (golem.Target != null ? targetDistance <= CloseRangeCutoff : false);
        
        readyDuration = Random.Range(0.5f * (closeRange ? GroundSmashReadyDuration : StompReadyDuration), 1.25f * (closeRange ? GroundSmashReadyDuration : StompReadyDuration));
        readyTimer = 0f;
        
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate() // Adjust the logic later here to either stomp or ground smash depending on range
    {
        if (golem.Target ==null) 
        {
            golem.ChangeState(golem.GolemWanderState);
            return;
        }
        targetDistance = Vector3.Distance(golem.Target.transform.position, golem.transform.position);
        closeRange = targetDistance <= CloseRangeCutoff;
        
        golem.ApplyGravity();
        golem.LookAt(golem.Target.transform.position);
        
        readyTimer += golem.LocalDeltaTime;

        if (readyTimer > readyDuration)
        {
            attackDirection = golem.Target.transform.position - golem.transform.position;
            
            golem.ChangeState(closeRange ? golem.GolemGroundSmashState : golem.GolemStompState); // Set to ground smash state for now for testing
            return;
        }
    }

    public Vector3 GetAttackDirection() {
        return attackDirection;
    }
    
    
    
}