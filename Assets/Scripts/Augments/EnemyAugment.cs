using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// TODO: Switch the system to using Event Hooks for enemy spawning

// Augments related to Enemy wide actions will inherit the checking logic
public class EnemyAugment : Augment
{
    private List<Enemy> current_enemies; 
    private int prev_count = 0;

    public override void Start()
    {
        base.Start();
        current_enemies = new List<Enemy>(FindObjectsOfType<Enemy>());
        StartCoroutine(CheckforEnemyChanges());
    }
    
    // REMOVE: when shifting to modular
    public void Awake()
    {
        Branch = AugmentBranch.ENEMY_BRANCH;
        Level = 1;
    }

    // Refresh the list of enemies in the scene
    private IEnumerator CheckforEnemyChanges()
    {
        while (true)
        {
            yield return new WaitForSeconds(4.0f);

            List<Enemy> current_enemies = new List<Enemy>(FindObjectsOfType<Enemy>());

            if (current_enemies.Count != prev_count)
            {
                Debug.Log("Number of spawned current_enemies: " + current_enemies.Count);
                prev_count = current_enemies.Count;
                ColorEnemies(current_enemies);
            }
        }
    }

    // Placehold Function: This will be done in a subclass
    private void ColorEnemies(List<Enemy> current_enemies)
    {
        foreach (var enemy in current_enemies)
        {
            if (enemy != null)
            {
                Renderer renderer = enemy.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.blue;
                }
            }
        }
    }


}
