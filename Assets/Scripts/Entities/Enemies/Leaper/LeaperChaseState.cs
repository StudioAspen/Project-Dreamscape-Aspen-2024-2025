public class LeaperChaseState : EnemyChaseState
{
    private Leaper leaper; // Reference to the specific Leaper enemy using this state
    private float safeDistanceForLeap = 2f; // Minimum distance from player before preparing to leap or hop back

    public LeaperChaseState(Leaper enemy) : base(enemy)
    {
        leaper = enemy;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        leaper.SetSpeedModifier(0.8f); // Slightly reduce speed to allow for reaction time during hops
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void Update()
    {
        base.Update();

        if (leaper.Target == null)
        {
            leaper.ChangeState(leaper.EnemyIdleState);
            return;
        }

/*        // Move towards the player if the target is beyond the safe distance threshold
        if (leaper.Distance(leaper.Target) > safeDistanceForLeap)
        {
            leaper.SetDestination(leaper.Target.transform.position, true);
        }
        else
        {
            // If within the safe distance, prepare for hop or leap back
            leaper.ChangeState(leaper.LeaperHopState);
        }*/
    }

    public override void FixedUpdate()
    {

    }
}