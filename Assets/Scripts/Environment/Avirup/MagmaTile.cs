using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MagmaTile", menuName = "Magma Biome/Magma Tile", order = 2)]
public class MagmaTile : ScriptableObject
{
    public enum FeatureType { Geyser, MagmaLake, FloatingPlatform };

    public string tileName;
    public FeatureType type;
    public GameObject prefab;
    public float spawnProbability; // Chance of this feature spawning (0.0 - 1.0)
    // Add other properties as needed (e.g., min/max size for lakes)
}
