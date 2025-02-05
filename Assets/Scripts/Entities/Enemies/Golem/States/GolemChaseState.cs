using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GolemChaseState : EnemyChaseState
{
    [field: Header("Config")]
    [field: SerializeField] public float AttackRange { get; private set; } = 1f;
    
    private Golem golem;

    private protected override void Init(Entity entity)
    {
        base.Init(entity);

        golem = entity as Golem;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if(golem.Target == null)
        {
            golem.ChangeState(golem.GolemWanderState);
            return;
        }

        if(golem.Distance(golem.Target) < AttackRange)
        {
            Vector3 attackDir = golem.Target.transform.position - golem.transform.position;
            golem.GolemReadyAttackState.SetAttackDirection(attackDir);
            golem.ChangeState(golem.GolemReadyAttackState);
            return;
        }
        
     
    }
    
}
