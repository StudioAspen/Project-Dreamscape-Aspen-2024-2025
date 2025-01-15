using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// A stationary object will placed at the center of the land for 1 minute, Every 30 seconds it will go to a neighboring land.
// All lands will spawn enemies, if the object survives, trigger EOW
[CreateAssetMenu(fileName = "Defend World Event", menuName = "World Event/Defend")]
public class DefendWorldEventSO : WorldEventSO
{
    [field: Header("Config")]
    [field: SerializeField] public float DefendEventDuration { get; private set; } = 60f;
    [field: SerializeField] public int DefendEventMaxHealth { get; private set; } = 200;
    [field: SerializeField] public DefendEventEntity DefendEventEntityPrefab { get; private set; }
    [field: SerializeField] public TMP_Text DefendEventUIPrefab { get; private set; }

    private TMP_Text UIText;

    private DefendEventEntity defendEventEntity;

    float remainingTime;

    public override void OnStarted()
    {
        UIText = GameObject.Instantiate(DefendEventUIPrefab, GameObject.FindGameObjectWithTag("Main Canvas").transform);
        
        // All lands will spawn enemies
        StartEnemySpawnersWithCurrency(worldManager.SpawnedLands.Values.ToList());

        // Select a random land and spawn the defend event entity in the center of the land
        LandManager randomLand = worldManager.GetRandomLand();
        defendEventEntity = GameObject.Instantiate(DefendEventEntityPrefab, randomLand.transform.position + 6f * Vector3.up, Quaternion.identity, eventManager.transform);
        defendEventEntity.SetMaxHealth(DefendEventMaxHealth, true);

        // Listen for when the defend event entity dies
        defendEventEntity.OnEntityDeath += DefendEventEntity_OnEntityDeath;

        remainingTime = DefendEventDuration;
    }

    public override void OnCleared()
    {
        StopEnemySpawners();

        foreach (LandManager land in worldManager.SpawnedLands.Values)
        {
            land.EnemySpawner.KillAll();
        }
        
        // Remove the defend event entity and cleanup the listener
        if(defendEventEntity != null)
        {
            defendEventEntity.OnEntityDeath -= DefendEventEntity_OnEntityDeath;
            GameObject.Destroy(defendEventEntity.gameObject);
        }

        GameObject.Destroy(UIText.gameObject);
    }

    public override void OnUpdate()
    {
        if(defendEventEntity == null) return;

        remainingTime -= Time.deltaTime;   

        UIText.text = $"{Mathf.Round(remainingTime)}s";

        if (remainingTime <= 0 && defendEventEntity.CurrentHealth > 0)
        {
            remainingTime = 0f;
            eventManager.ClearEvent();
            return;
        }
    }

    private void DefendEventEntity_OnEntityDeath(GameObject killerObject)
    {
        defendEventEntity.OnEntityDeath -= DefendEventEntity_OnEntityDeath;

        Debug.Log("Defend Event Entity has died. You failed.");
    }
}
