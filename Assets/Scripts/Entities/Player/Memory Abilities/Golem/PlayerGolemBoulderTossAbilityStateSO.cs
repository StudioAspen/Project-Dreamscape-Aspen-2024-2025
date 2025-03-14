using System.Collections;
using System.Runtime.InteropServices;
using Dreamscape.Abilities;
using UnityEngine;

/*
 * CastedAbility spawnedAbility = ObjectPoolerManager.Instance.SpawnPooledObject<CastedAbility>(abilityComboData.AbilityPrefab.gameObject);
   spawnedAbility.Init(player);
 */


[CreateAssetMenu(fileName = "Golem Memory Boulder Toss Ability", menuName = "Memory Abilities/Golem/BoulderToss")]
public class PlayerGolemBoulderTossAbilityStateSO : PlayerAbilityStateSO
{
    [field: Header("Config")]
    [field: SerializeField] public AnimationClip ChargeAnimationClip { get; private set; }
    [field: SerializeField] public PlayerGolemWindDownAbilityStateSO WindDownState { get; private set; }
    [field: SerializeField] public float ChargeContactDamageMultiplier { get; private set; } = 2f;
    [field: SerializeField] public float SpeedModifier { get; private set; } = 0f;
    [field: SerializeField] public float TossDuration { get; private set; } = 20f;
    [field: SerializeField] public float rotationSpeed { get; private set; } = 5f;
    [field: SerializeField] public float ChargeOnImpactLaunchForce { get; private set; } = 10f;
    [field: SerializeField] public float ChargeStunDuration { get; private set; } = 4f;
    
    [field: SerializeField] public GameObject BoulderPrefab { get; private set; }
    [field: SerializeField] public float BoulderSpawnDelay { get; private set; } = 1 + (45 / 60f); // Boulder spawn delay should be how long after the throw animation under normal time until boulder prefab spawns

    private Coroutine spawnBoulderCoroutine;

    private float timer;

    public override bool CanUseAbility(Player player)
    {
        bool cannotUseAbility =
            !player.IsGrounded ||
            player.CurrentState == player.PlayerAttackState ||
            player.CurrentState == player.PlayerChargeState ||
            player.CurrentState == player.PlayerDashState;

        return !cannotUseAbility;
    }

    public override bool CanCancelAbility(Player player, EntityBaseState desiredState)
    {
        return desiredState == player.PlayerAbilityState || desiredState == player.DefaultState;
    }

    public override void OnEnter()
    {
        player.PlayOneShotAnimation(ChargeAnimationClip);

        spawnBoulderCoroutine = player.StartCoroutine(UnleashBoulder());

        timer = 0f;
    }

    public override void OnExit()
    {
        if (spawnBoulderCoroutine != null) {
            player.StopCoroutine(spawnBoulderCoroutine);
            spawnBoulderCoroutine = null;
        }
    }

    public override void OnUpdate()
    {
        player.ApplyGravity();

        timer += player.LocalDeltaTime;
        if (timer > TossDuration)
        {
            player.PlayerAbilityState.TryChangeAbilityState(WindDownState, true);
            return;
        }

        player.ApplyRotationToNextMovement();
        player.LookAt(player.transform.position + player.TargetForwardDirection);
    }

    private IEnumerator UnleashBoulder() 
    {
        yield return new WaitForSeconds(BoulderSpawnDelay / player.LocalTimeScale.GetFloatValue());
        CastedAbility spawnedAbility = ObjectPoolerManager.Instance.SpawnPooledObject<CastedAbility>(BoulderPrefab);
        spawnedAbility.Init(player);
    }

   
}
