using UnityEngine;

[System.Serializable]
public class GolemChaseState : EnemyChaseState
{
    [field: SerializeField] public float AttackRange { get; private set; } = 5f;
    [field: SerializeField] public float ChaseSpeedModifier { get; private set; } = 1f;
    
    private Golem golem;

    public override void Init(Entity entity)
    {
        base.Init(entity);

        golem = entity as Golem;
    }

    public override void OnEnter()
    {
        golem.SetSpeedModifier(ChaseSpeedModifier);
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (golem.Target == null)
        {
            golem.ChangeState(golem.GolemWanderState);
            return;
        }

        if (golem.Distance(golem.Target) < AttackRange)
        {
            golem.ChangeState(golem.GolemReadyAttackState);
            return;
        }
    }
}
