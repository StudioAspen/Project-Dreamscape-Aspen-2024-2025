using UnityEngine;

public class GolemReadyAttackState : GolemBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public float AttackReadyDuration { get; private set; } = 0.5f;

    private float readyTimer;
    private float readyDuration;
    private Vector3 attackDirection;

    public override void OnEnter()
    {
        golem.TransitionToAnimation("Attack", AttackReadyDuration);

        golem.SetSpeedModifier(0f);

        readyDuration = Random.Range(0.5f * AttackReadyDuration, 1.25f * AttackReadyDuration);
        readyTimer = 0f;
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate() // Adjust the logic later here to either stomp or ground smash depending on range
    {
        golem.ApplyGravity();

        golem.LookAt(golem.transform.position + attackDirection);
        
        golem.TransitionToAnimation("GolemGroundSmash");

        readyTimer += golem.LocalDeltaTime;

        if (readyTimer > readyDuration)
        {
            golem.ChangeState(golem.GolemGroundSmashState); // Set to ground smash state for now for testing
            return;
        }
    }
    
    public void SetAttackDirection(Vector3 direction)
    {
        attackDirection = direction;
    }
    
}