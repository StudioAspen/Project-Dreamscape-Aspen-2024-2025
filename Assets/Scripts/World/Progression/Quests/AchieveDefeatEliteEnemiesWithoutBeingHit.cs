using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Defeat X Elite Enemies No Hit Progression Quest", menuName = "World/Progression Quest/Defeat X Elite Enemies No Hit Progression Quest")]
public class AchieveDefeatEliteEnemiesWithoutBeingHit : ProgressionQuestSO
{
    private Player player;

    [field: Header("Config")]
    [field: SerializeField] public int EliteKillGoal { get; private set; } = 2;

    private int killCount;
   
    private protected override void OnActivated()
    {
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            OnCleanUp();
            return;
        }
        killCount = 0;

        player.OnKillEntity += Player_OnKillEntity;
        player.OnEntityTakeDamage += Player_OnTakeDamage;
    }

    private void Player_OnTakeDamage(int damage, Vector3 hitPosition, GameObject source)
    {
            killCount = 0;
    }

    private void Player_OnKillEntity(Entity entity)
    {
        EntityStatusEffector entityStatusEffector = entity.GetComponent<EntityStatusEffector>();
        EliteVariantStatusEffectSO eliteStatus = null;
        if (entityStatusEffector != null)
        {
            foreach (StatusEffectSO statusEffect in entityStatusEffector.CurrentStatusEffects.Values)
            {
                EliteVariantStatusEffectSO eliteVariantStatusEffect = statusEffect as EliteVariantStatusEffectSO;
                if (eliteVariantStatusEffect != null)
                {
                    eliteStatus = eliteVariantStatusEffect;
                    break;
                }
            }
        }

        if (eliteStatus == null) return;

        killCount++;
        if (killCount >= EliteKillGoal)
        {
            Complete();
            return;
        }
    }

    private protected override void OnCleanUp()
    {
        player.OnKillEntity -= Player_OnKillEntity;
        player.OnEntityTakeDamage -= Player_OnTakeDamage;
    }

    private protected override void OnUpdate()
    {

    }
}
