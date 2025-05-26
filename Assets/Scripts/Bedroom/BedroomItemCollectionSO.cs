using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BedroomItemCollection", menuName = "Bedroom/Item Collection")]
public class BedroomItemCollectionSO : ScriptableObject
{
    [field:SerializeField] public List<BedroomItemConfigSO> Items { get; private set; } = new List<BedroomItemConfigSO>();
}