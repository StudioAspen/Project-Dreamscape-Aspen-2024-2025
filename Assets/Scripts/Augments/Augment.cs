using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Augment : MonoBehaviour
{
    [field: Header("Augment Attributes")]
    // [field: SerializeField] public AugmentBranch Branch { get; private set }
    [field: SerializeField] public int Level { get; protected set; }
    [field: SerializeField] public string AugmentName { get; protected set; }
    [field: SerializeField] public Sprite AugmentIcon { get; protected set; }
    
    protected bool isActive;

    protected Player player;


    protected virtual void Awake()
    {

    }


    protected virtual void Start()
    {
        player = GetComponent<AugmentManager>().Player.GetComponent<Player>();
    }


    protected virtual void OnEnable()
    {

    }
}
