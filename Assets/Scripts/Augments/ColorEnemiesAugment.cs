using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorEnemiesAugment : EnemyAugment
{


    public override void Start()
    {
        base.Start();

    }

    public void Awake()
    {
        Branch = AugmentBranch.COLOR_BRANCH;
        Level = 1;
    }

    protected override void OnEnemiesChanged(List<Enemy> enemies)
    {
        ColorEnemies(enemies);
    }

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
