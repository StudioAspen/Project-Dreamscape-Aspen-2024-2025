using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.VFX;

public class MagicMist : Obstacle
{
    [field: Header("MagicMist: States")]

    [field: SerializeField] public float EnemyDamageIncrease { get; private set; } = 1.5f;
    [field: SerializeField] public float PlayerDamageIncrease { get; private set; } = 1.5f;

    private protected override void OnAwake() { }
    private protected override void OnStart() { }
    private protected override void OnUpdate() { }

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            // Increase the enemy's damage
            enemy.DamageModifier.AddMultiplier(EnemyDamageIncrease, this); // Example multiplier
            Debug.Log($"Enemy entered {enemy.DamageModifier.GetFloatValue()} the obstacle");
        }

        if (other.TryGetComponent<Player>(out Player player))
        {
            // Increase the enemy's damage
            player.DamageModifier.AddMultiplier(PlayerDamageIncrease, this); // Example multiplier
            Debug.Log($"Player entered {player.DamageModifier.GetFloatValue()} the obstacle");

            // Reverse Player controls
            PlayerInputReader inputReader = player.GetComponent<PlayerInputReader>();
            if (inputReader != null)
            {
                inputReader.SetMovementReversed(true);
                Debug.Log("Player controls reversed");
            }

        }


    }

    public void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            // Remove the damage increase when the enemy exits the obstacle
            enemy.DamageModifier.RemoveMultiplier(EnemyDamageIncrease, this);
            Debug.Log($"Enemy existed {enemy.DamageModifier.GetFloatValue()} the obstacle");
        }

        if (other.TryGetComponent<Player>(out Player player))
        {
            // Increase the enemy's damage
            player.DamageModifier.RemoveMultiplier(PlayerDamageIncrease, this); // Example multiplier
            Debug.Log($"Player existed {player.DamageModifier.GetFloatValue()} the obstacle");

            // Reverse Player controls
            PlayerInputReader inputReader = player.GetComponent<PlayerInputReader>();
            if (inputReader != null)
            {
                inputReader.SetMovementReversed(false);
                Debug.Log("Player controls normal");
            }
        }
    }
}
