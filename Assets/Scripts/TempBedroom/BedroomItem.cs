using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedroomItem
{
    public string itemName;
    public string itemType;
    public GameObject itemModel;
    public Vector3 itemPosition;
    public string effectDescription;

    public virtual void ApplyEffect()
    {
        Debug.Log(itemName + " effect applied: " + effectDescription);
    }
}
