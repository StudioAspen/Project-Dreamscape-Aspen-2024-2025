using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggYolkExplosionState : EggYolkBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public float PlayerDetectionRadius { get; private set; } = 5f;
    [field: SerializeField] public float AOEDamageMultiplier { get; private set; } = 1.5f;
    [field: SerializeField] public float AOERadius { get; private set; } = 4f;
    [field: SerializeField] public float AOELaunchForce { get; private set; } = 7.5f;
    [field: SerializeField] public float AOEStunDuration { get; private set; } = 3f;

    public override void OnEnter()
    {

    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        CheckForNearbyPlayers();

        CustomDebug.InstantiateTemporarySphere(transform.position, AOERadius, 0.25f, new Color(1f, 0, 0, 0.2f));

        CameraShakeManager.Instance.ShakeCamera(15f, 1f);
    }

    private void CheckForNearbyPlayers()
    {
        List<Entity> entityList = Entity.GetEntitiesThroughAOE(eggYolk.transform.position, PlayerDetectionRadius, false);
        foreach (Entity entity in entityList)
        {
            Player player = entity as Player;
            if (player = null) continue;
        }
    }
}
