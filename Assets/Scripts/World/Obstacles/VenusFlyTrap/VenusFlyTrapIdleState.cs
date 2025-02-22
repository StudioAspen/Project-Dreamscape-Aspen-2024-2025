using UnityEngine;
using System.Collections.Generic;

public class VenusFlyTrapIdleState : VenusFlyTrapBaseState
{
    [field: SerializeField] public float DetectionRadius { get; private set; } = 5f;

    public override void OnEnter()
    {
        // Do nothing initially, just wait for a player to enter range
    }

    public override void OnUpdate()
    {
        // Try to find a target (player) within range
        List<Entity> nearbyEntities = GetNearbyTargets();
        if (nearbyEntities.Count > 0)
        {
            Debug.Log("Venus Fly Trap found a target!");
            venusFlyTrap.ChangeState(venusFlyTrap.VenusFlyTrapWindupState);
        }
    }

    public override void OnExit()
    {

    }

    private List<Entity> GetNearbyTargets()
    {
        Collider[] hits = Physics.OverlapSphere(venusFlyTrap.transform.position, DetectionRadius, LayerMask.GetMask("Entity"));

        List<Entity> targets = new List<Entity>();
        foreach (Collider hit in hits)
        {
            Entity entity = hit.GetComponent<Player>();
            if (entity != null)
            {
                targets.Add(entity);
            }
        }
        return targets;
    }
}
