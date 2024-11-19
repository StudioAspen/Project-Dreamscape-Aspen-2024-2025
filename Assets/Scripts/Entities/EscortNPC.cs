using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EscortNpc : Entity
{

    [SerializeField, Scene] public WorldManager worldManager;
    [SerializeField, Scene] private EventManager eventManager;

    private Vector2Int currentGridPosition;

    void Start()
    {
        currentGridPosition = worldManager.GetLandByWorldPosition(this.transform.position).GridPosition;
        worldManager.SpawnedLandsDictionary[currentGridPosition].EnemySpawner.NpcPresent = true;
    }


    void Update()
    {

        if (currentGridPosition != worldManager.GetLandByWorldPosition(this.transform.position).GridPosition)
        {
            worldManager.SpawnedLandsDictionary[currentGridPosition].EnemySpawner.NpcPresent = false;
            currentGridPosition = worldManager.GetLandByWorldPosition(this.transform.position).GridPosition;
            worldManager.SpawnedLandsDictionary[currentGridPosition].EnemySpawner.NpcPresent = true;
        }



        if(this.CurrentHealth <= 0) //if npc dies, end of wave
        {
            eventManager.FinishEscortWave();
        }

    }

    



    public void Destroy()
    {
        Destroy(this.GameObject());
    }


}
