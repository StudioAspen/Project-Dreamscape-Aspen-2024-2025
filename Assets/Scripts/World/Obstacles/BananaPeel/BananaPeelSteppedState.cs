using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaPeelSteppedState : BananaPeelBaseState
{
    [field: SerializeField] public float LaunchForce { get; private set; } = 5f;
    [field: SerializeField] public float StunDuration { get; private set; } = 2f;

    public override void OnEnter()
    {

    }

    public override void OnExit()
    {
        bananapeel.gameObject.SetActive(false);
    }

    public override void OnUpdate()
    {

    }

    public override void OnOnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            Entity entity = other.gameObject.GetComponent<Entity>();
            Vector3 luanchDirection = entity.GetColliderCenterPosition() - bananapeel.transform.position;
            entity.TryChangeToLaunchState(luanchDirection, LaunchForce, StunDuration);

            bananapeel.ChangeState(bananapeel.BananaPeelIdleState);
        }
    }
}
