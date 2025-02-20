using UnityEngine;
public class VenusFlyTrapWindupState : VenusFlyTrapBaseState
{
    [field: SerializeField] public float WindupDuration { get; private set; } = 1f;
    private float timer;

    public override void OnEnter()
    {
        timer = 0f;
        // Start windup animation
    }

    public override void OnExit()
    {
        // Cleanup logic
    }

    public override void OnUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= WindupDuration)
        {
            venusFlyTrap.ChangeState(venusFlyTrap.VenusFlyTrapSnapState);
        }
    }
}