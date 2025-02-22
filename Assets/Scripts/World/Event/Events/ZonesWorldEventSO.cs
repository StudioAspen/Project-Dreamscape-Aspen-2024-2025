using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// A 3x3 of lands are highlighted on the map. Enemies will only spawn from those lands, once they are all defeated trigger EOW
[CreateAssetMenu(fileName = "Zones World Event", menuName = "World/World Event/Zones")]
public class ZonesWorldEventSO : WorldEventSO
{
    [field: Header("Config")]
    /// <summary>
    /// Controls the max search distance used for finding lands in an outbreak.
    /// </summary>
    [field: Range(1, 29)]
    [field: SerializeField] private int MaxSearchLayer;

    private List<LandManager> affectedLands = new List<LandManager>();
    private int activeLands;

    private List<GameObject> debugSpheres = new List<GameObject>();

    private protected override void OnStarted()
    {
        activeLands = 0;

        // Get a random 3x3 of lands and start the enemy spawners on them if they have positive levels
        affectedLands = AffectLandsFromEpicenter();
        foreach(LandManager land in affectedLands)
        {
            // if (land.Level <= 0) continue;

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

    private protected override void OnCleared()
    {
        StopEnemySpawners();

        foreach (LandManager land in affectedLands)
        {
            land.EnemySpawner.DeactivateAllEnemies();

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
  /// Creates a square-shaped outbreak pattern starting from a randomly selected land,
  /// then spiraling outward layer by layer in a clockwise direction.
  /// </summary>
  /// <returns>List of affected lands in order of infection</returns>
  private List<LandManager> AffectLandsFromEpicenter() {
    List<LandManager> outbreakLands = new List<LandManager>();

    // Get the highest possible floored integer from a square root
    int s = Mathf.FloorToInt(Mathf.Sqrt(worldManager.SpawnedLands.Count));
    // Returns the largest integer that, when squared, will be <= the number of spawned lands
    int outbreakSize = Mathf.FloorToInt(Mathf.Pow(s, 2));
    Debug.Log($"Spawned Lands: {worldManager.SpawnedLands.Count} \nOutbreak Size: {outbreakSize}");

    // Gets a land by its weight, which increases proportionally with its land level.
    LandManager epicenter = worldManager.GetRandomLandByWeight();
    Debug.Log($"Epicenter Level: {epicenter.Level}");
    outbreakLands.Add(epicenter);

    debugSpheres.Add(CustomDebug.InstantiateTemporarySphere(epicenter.transform.position + 10f * Vector3.up, 3f, Mathf.Infinity, Color.red));

    int layer = 1;
    while (outbreakLands.Count < outbreakSize - 1 && layer <= MaxSearchLayer)
    {
      Vector2Int gridPos;

      // Top row 
      for (int x = -layer; x <= layer; x++)
      {
        gridPos = epicenter.GridPosition + new Vector2Int(x, layer);
        if (TryAddLand(gridPos, outbreakLands) && ShouldReturn(outbreakLands, outbreakSize)) 
          return outbreakLands;
      }

      // Right side 
      for (int y = layer - 1; y >= -layer; y--)
      {
        gridPos = epicenter.GridPosition + new Vector2Int(layer, y);
        if (TryAddLand(gridPos, outbreakLands) && ShouldReturn(outbreakLands, outbreakSize)) 
          return outbreakLands;
      }

      // Bottom row 
      for (int x = layer; x >= -layer; x--)
      {
        gridPos = epicenter.GridPosition + new Vector2Int(x, -layer);
        if (TryAddLand(gridPos, outbreakLands) && ShouldReturn(outbreakLands, outbreakSize)) 
          return outbreakLands;
      }

      // Left side
      for (int y = -layer; y < layer; y++)
      {
        gridPos = epicenter.GridPosition + new Vector2Int(-layer, y);
        if (TryAddLand(gridPos, outbreakLands) && ShouldReturn(outbreakLands, outbreakSize)) 
          return outbreakLands;
      }

      layer++;
    }
    
    Debug.Log($"Outbreak Lands: {outbreakLands.Count}\nLast Layer Searched: {layer}");
    return outbreakLands;
  }

  private bool TryAddLand(Vector2Int pos, List<LandManager> outbreakLands)
  {
    if (worldManager.TryGetLandByGridPosition(pos, out LandManager land))
      if (!outbreakLands.Contains(land))
      {
        outbreakLands.Add(land);
        debugSpheres.Add(CustomDebug.InstantiateTemporarySphere(
          land.transform.position + 10f * Vector3.up, 
          3f, 
          Mathf.Infinity, 
          new Color(1, 0, 0, 0.25f)));
        return true;
      }
    return false;
  }

  private bool ShouldReturn(List<LandManager> outbreakLands, int outbreakSize)
  {
    return outbreakLands.Count >= outbreakSize || 
      outbreakLands.Count >= worldManager.SpawnedLands.Count;
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
