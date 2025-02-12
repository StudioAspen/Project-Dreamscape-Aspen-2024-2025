using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSplitState : SlimeBaseState
{

    // [field: Header("Condig")]
    // private Enemy slimeEnemyPrefab;

    // // change location to slime.cs for easy access 
    // [field: SerializeField] public int SplitCount { get; private set;} = 2;

    

    // private protected override void Init(Entity entity)
    // {
    //     base.Init();
    //     slimeEnemyPrefab = GetEnemyPrefabFromCurrentType();

    //     ontransition();
    // }


    // code  bellow this is copied from DuplicatorEliteVariantStatusEffectSO    
    // private Enemy GetEnemyPrefabFromCurrentType()
    // {
    //     if(enemy.Spawner == null)
    //     {
    //         Debug.Log("spawner = null ");
    //     }
        
    //     foreach (Enemy enemyPrefab in enemy.Spawner.NeutralEnemyPrefabs)
    //     {
    //         if (enemyPrefab.GetType() == enemy.GetType())
    //         {
    //             return enemyPrefab;
    //         }
    //     }

    //     Debug.LogWarning("Could not find enemy prefab from current type.");

    //     return null;
    // }

    // private void ontransition()
    // {
    //         // "infinite loop" probably because all slimes start at
    //         // split state so every copy makes another 2 copies
    //         for (int i = 0; i < SplitCount; i++ )
    //         {
    //             Debug.Log("for loop start");
    //             Debug.Break();
    //             float angle = i * (360f / SplitCount);

            
    //             Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad)) * 1f;
            
            
    //             Vector3 spawnPos = enemy.transform.position + offset;
    //             enemy.Spawner.SpawnEnemy(slimeEnemyPrefab, spawnPos);

                
        
    //         }
    // }

}
