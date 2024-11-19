using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaBlock : MonoBehaviour
{
    [SerializeField] Transform TilePosition;
    [SerializeField] public int MaxFloors = 10;
    [SerializeField] public int MaxLava = 4;
    [SerializeField] private float startingX = 2.0f;
    [SerializeField] private float startingZ = 2.0f;
    [SerializeField] GameObject floorModel;
    
    private Lava CreateLava;

    //Use this for the initialization
    private void Start()
    {
        TilePosition = GetComponent<Transform> ();
        CreateLava = GetComponentInChildren<Lava>();
    }

    private void BuildBlock()
    {
        for (int address = 0; address < MaxLava; address++) {
            GameObject building = CreateLava.BuildLava();
            building.transform.parent = TilePosition;
            building.transform.position = new Vector3(0.0f, 0.0f, building.transform.position.z + address+3);
            building.name = "Building" + address;


        }
    }
}
