using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Augment : MonoBehaviour
{
    [Header("Augment Attributes")]
    [SerializeField] public AugmentBranch Branch;
    [SerializeField] public int Level;

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
