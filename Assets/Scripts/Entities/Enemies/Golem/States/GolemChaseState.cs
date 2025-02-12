using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GolemChaseState : EnemyChaseState
{
    [field: Header("Config")]
    [field: SerializeField] public float AttackRange { get; private set; } = 5f;
    
    private Golem golem;

    private protected override void Init(Entity entity)
    {
        base.Init(entity);

        golem = entity as Golem;
    }

    public override void OnEnter()
    {
        golem.SetSpeedModifier(.75f);
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
            golem.ChangeState(golem.GolemReadyAttackState);
            return;
        }
        
     
    }
    
}
