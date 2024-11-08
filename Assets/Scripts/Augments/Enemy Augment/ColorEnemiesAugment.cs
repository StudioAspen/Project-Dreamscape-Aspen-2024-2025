using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorEnemiesAugment : Augment
{


    public void Start()
    {
        base.Start();

    }

    public void Awake()
    {
        Level = 1;
    }

    /* 
     * using EnemyAugment script that is obsolete due to Entity script
    protected override void OnEnemiesChanged(List<Enemy> enemies)
    {
        ColorEnemies(enemies);
    }
    */

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
