/*
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

    public void Start()
    {
        base.Start();
        current_enemies = new List<Enemy>(FindObjectsOfType<Enemy>());
        StartCoroutine(CheckforEnemyChanges());
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
                OnEnemiesChanged(current_enemies);
            }
        }
    }



    protected virtual void OnEnemiesChanged(List<Enemy> enemies)
    {

    }
}
*/
