using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// A 3x3 of lands are highlighted on the map. Enemies will only spawn from those lands, once they are all defeated trigger EOW
[CreateAssetMenu(fileName = "Zones World Event", menuName = "World Event/Zones")]
public class ZonesWorldEventSO : WorldEventSO
{
    [field: Header("Config")]
    [field: SerializeField] public int ZonesEventDummyVariable { get; private set; }

    private List<LandManager> affectedLands = new List<LandManager>();
    private int activeLands;

    private List<GameObject> debugSpheres = new List<GameObject>();

    public override void OnStarted()
    {
        activeLands = 0;

        // Get a random 3x3 of lands and start the enemy spawners on them if they have positive levels
        affectedLands = GetRandom3x3Land();
        foreach(LandManager land in affectedLands)
        {
            if (land.Level <= 0) continue;

            StartEnemySpawnerWithCurrency(land);

            // Track when the enemy spawner is depleted to decrement the activeLands counter
            land.EnemySpawner.OnSpawnerDepleted += EnemySpawner_OnSpawnerDepleted;

            activeLands++;
        }

        if(activeLands <= 0)
        {
            eventManager.ClearEvent();
        }
    }

    public override void OnCleared()
    {
        StopEnemySpawners();

        foreach (LandManager land in affectedLands)
        {
            land.EnemySpawner.KillAll();

            // Unsubscribe from the OnSpawnerDepleted event for each of the affected lands
            land.EnemySpawner.OnSpawnerDepleted -= EnemySpawner_OnSpawnerDepleted;
        }
        affectedLands.Clear();

        foreach(GameObject sphere in debugSpheres)
        {
            GameObject.Destroy(sphere);
        }
        debugSpheres.Clear();
    }

    /// <summary>
    /// Generates a list of 3x3 lands centered around a random land.
    /// </summary>
    /// <returns>The list of 3x3 lands.</returns>
    private List<LandManager> GetRandom3x3Land()
    {
        List<LandManager> resultingLands = new List<LandManager>();

        LandManager centerLand = worldManager.GetRandomLand();
        resultingLands.Add(centerLand);

        debugSpheres.Add(CustomDebug.InstantiateTemporarySphere(centerLand.transform.position + 10f * Vector3.up, 3f, Mathf.Infinity, Color.red));

        List<Vector2Int> offsets = new List<Vector2Int>() {
            new Vector2Int(1, 0),  // Right
            new Vector2Int(-1, 0), // Left
            new Vector2Int(0, 1),  // Top
            new Vector2Int(0, -1), // Bottom
            new Vector2Int(1, 1),  // Top-right
            new Vector2Int(1, -1), // Bottom-right
            new Vector2Int(-1, 1), // Top-left
            new Vector2Int(-1, -1) // Bottom-left
        };

        // Add neighboring lands using the offsets
        foreach (Vector2Int offset in offsets)
        {
            if (worldManager.TryGetLandByGridPosition(centerLand.GridPosition + offset, out LandManager neighborLand))
            {
                resultingLands.Add(neighborLand);

                debugSpheres.Add(CustomDebug.InstantiateTemporarySphere(neighborLand.transform.position + 10f * Vector3.up, 3f, Mathf.Infinity, new Color(1, 0, 0, 0.25f)));
            }
        }

        return resultingLands;
    }

    private void EnemySpawner_OnSpawnerDepleted()
    {
        activeLands--;

        if(activeLands <= 0)
        {
            eventManager.ClearEvent();
        }
    }
}
