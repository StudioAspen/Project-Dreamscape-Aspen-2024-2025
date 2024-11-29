using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class DefendObject : Entity
{

    [SerializeField, Scene] public WorldManager worldManager;
    [SerializeField, Scene] private EventManager eventManager;

    private Vector2Int currentGridPosition;

    private float transitionTimer;
    private float transitionTimerLength = 5f;

    private List<Vector2Int> adjecentLandsList = new List<Vector2Int>();

    void Start()
    {
        currentGridPosition = worldManager.GetLandByWorldPosition(this.transform.position).GridPosition;
        transitionTimer = transitionTimerLength;
        GrabAdjacentLands();

        
    }



    private void Update()
    {
        transitionTimer -= Time.deltaTime;
        if(transitionTimer <= 0)
        {
            Debug.Log("SDLFKJSDF");
            transitionTimer = transitionTimerLength;
            MoveDefendObject();
            GrabAdjacentLands();
        }


        if (this.CurrentHealth <= 0) //if object dies, end of wave
        {
            eventManager.FinishDefendWave();
        }

    }




    public void Destroy()
    {
        Debug.Log("DESTROY");
        Destroy(this.GameObject());
    }

    private void GrabAdjacentLands()
    {
        adjecentLandsList.Clear();

        foreach (Vector2Int existentGridPosition in worldManager.SpawnedLandsDictionary.Keys)
        {
            if (existentGridPosition == currentGridPosition + Vector2.up)
            {
                adjecentLandsList.Add(existentGridPosition);
            }
            else if (existentGridPosition == currentGridPosition + Vector2.down)
            {
                adjecentLandsList.Add(existentGridPosition);
            }
            else if (existentGridPosition == currentGridPosition + Vector2.left)
            {
                adjecentLandsList.Add(existentGridPosition);
            }
            else if (existentGridPosition == currentGridPosition + Vector2.right)
            {
                adjecentLandsList.Add(existentGridPosition);
            }
        }


        for(int i = 0; i < adjecentLandsList.Count; i++)
        {
            Debug.Log(adjecentLandsList[i]);
        }

    }

    private void MoveDefendObject()
    {
        int randomAdjacentListIndex;
        randomAdjacentListIndex = UnityEngine.Random.Range(0, adjecentLandsList.Count);

        currentGridPosition = adjecentLandsList[randomAdjacentListIndex];
        this.transform.position = worldManager.GetLandPositionByGridPosition(currentGridPosition, 1f);


    }



}
