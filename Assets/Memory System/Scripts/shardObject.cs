using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShard", menuName = "MemorySystem/Shard")]
public class Shard : ScriptableObject

{

    public string shardName; // Unique name for the shard
    public GameObject model; // Model to instantiate when shard drops
    public Color shardColor;
    public Ability[] abilities; // Abilities based on bar level (0, 1, 2)


    [System.Serializable]
    public class Ability
    {
        public string abilityName;
        public string description;
        public int level; // 1, 2, or 3
        public UnityEngine.Events.UnityEvent skillAction; // Add functionality for abilities
    }

    public Ability GetAbility(int level)
    {
        if (level < 1 || level > 3) return null;
        return abilities[level - 1];
    }
}
