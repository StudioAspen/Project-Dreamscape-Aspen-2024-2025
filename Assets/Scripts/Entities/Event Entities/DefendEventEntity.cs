using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendEventEntity : Entity
{
    private protected override void OnAwake()
    {
        base.OnAwake();
    }

    private protected override void OnOnEnable()
    {
        base.OnOnEnable();

        SetStartState(EntityEmptyState);
    }

    private protected override void OnOnDisable()
    {
        base.OnOnDisable();
    }

    private protected override void OnStart()
    {
        base.OnStart();

        ChangeTeam(0);

        SetDefaultState(EntityEmptyState);
    }

    private protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    // Override to disable launching
    public override void Launch(Vector3 direction, float force)
    {
        
    }
}
