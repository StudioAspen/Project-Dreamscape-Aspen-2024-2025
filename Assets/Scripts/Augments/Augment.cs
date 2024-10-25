using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Augment : MonoBehaviour // public class Augment : ScriptableObject (Ambrose)
{
    // Ambrose
    [field: Header("[Augment Data]")]
    [field: SerializeField] public string Name { get; private set; } = "";
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public Augments Requirement { get; private set; }
    public bool isActive = false;

    // Evan 
    [Header("Augment Attributes")]
    [SerializeField] public AugmentBranch Branch;
    [SerializeField] public int Level;

    protected Player player;

    public virtual void Start()
    {
        player = GetComponent<AugmentManager>().Player.GetComponent<Player>();
    }

    public virtual void Activate()
    {
        Debug.Log("Augment Activated");
    }
}
