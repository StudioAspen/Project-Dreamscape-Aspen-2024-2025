using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    [SerializeField] Transform TilePosition;
    public int MaxFloors = 3;
    [SerializeField] private GameObject FloorModel;

    private void Start()
    {
         BuildLava();  
    }

    void Update()
    {

    }

    private GameObject GetStarterLava()
    {
        return GameObject.FindGameObjectWithTag("Lava");
    }

    private GameObject GetStarterFloor()
    {
        return GameObject.FindGameObjectWithTag("Floor");
    }

    private IEnumerator TestBuildFloor()
    {
        yield return new WaitForSeconds(2.0f);
        
        BuildFloor(0);
        BuildFloor(1);
        BuildFloor(2);
        BuildFloor(3);
    }

    public GameObject BuildFloor(int floor)
    {
        GameObject newFloorModel = GameObject.Instantiate(FloorModel);
        newFloorModel.SetActive (true);
        newFloorModel.transform.position = new Vector3 (0.0f, 0.0f, 0.0f);

        //I don't got a really good looking floor models for this part

        GameObject newFloor = GameObject.Instantiate(GetStarterFloor());
        newFloor.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        newFloor.name = "Floor" + floor;

        newFloorModel.transform.parent = newFloor.transform;
        newFloorModel.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

        newFloor.transform.position = new Vector3(0.0f, floor + 0.7f, 0.0f);

        return newFloor;
    }
     
    public GameObject BuildLava()
    {
        GameObject lava = GameObject.Instantiate(GetStarterLava());
        lava.transform.localPosition = new Vector3();


        for (int floor = 0; floor < MaxFloors; floor++){

            GameObject newFloor = BuildFloor(floor);
            newFloor.transform.parent = lava.transform;
            newFloor.tag = "CanBeRigidbody";
        }
        return lava;

       
            
    }

}
