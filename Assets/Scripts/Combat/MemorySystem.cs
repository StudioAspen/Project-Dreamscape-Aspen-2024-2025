using KBCore.Refs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MemorySystem : MonoBehaviour
{
    [SerializeField, Self] private Player player;
    [SerializeField]private int barMaximum; //amount of memory points to fill the memory bar
    Dictionary<Type, int> memories = new Dictionary<Type, int>(); //dictionary so we can add new memory types whenever, tied to enemy class ooooo
    [SerializeField]private RectTransform memoryBarTransform;
    private List<RectTransform> memoryTransforms = new List<RectTransform>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            
        }
        if (Input.GetKeyDown(KeyCode.U))
        {

        }
        if (Input.GetKeyDown(KeyCode.I))
        {

        }
    }
    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void OnEnable()
    {
        //enemies need a memory type and amount property before we can link these together easily..
        //player.OnKillEntity.AddListener(GainMemory())
    }

    private void OnDisable()
    {
        memories.Clear();
    }

    private void GainMemory(Type type, int memoryGained) //add memory and check if meter is full 
    {
        if (memories.ContainsKey(type))
        {
            memories[type] += memoryGained;
        }
        else
        {
            memories[type] = memoryGained;
        }
        UpdateDebugMeter();

        if (MemoryIsFull())
        {
            Debug.Log(MemoryEarned());
        }
    }

    private bool MemoryIsFull() //true if full false if not
    {
        int totalMemory = 0;
        foreach (Type memory in memories.Keys)
        {
            totalMemory += memories[memory];
            //adds all of them up together for comparison
        }

        if(totalMemory >= barMaximum)
        {
            return true;
        }

        return false;
    }

    private Type MemoryEarned()
    {
        //not sure how best to order memories in terms of priority in case two have the same value when meter is full
        //OH DESIGNERRRSSS (rings bell)
        //also probably the function we'd bestow the buff/item/whatever it was on
        Type memoryType = memories.Max(x => x.Key); //idk how this line works :p
        memories.Clear();
        return memoryType;
    }

    private void UpdateDebugMeter()
    {
        float maxWidth = memoryBarTransform.sizeDelta.x;
        float height = memoryBarTransform.sizeDelta.y;

        int i = 0;
        float meterOffset = 0f;
        foreach(Type memory in memories.Keys)
        {
            if( i+1 > memoryTransforms.Count)
            {
                memoryTransforms.Add(Instantiate(memoryBarTransform.gameObject, transform).GetComponent<RectTransform>());
            }
            float percentOfMeter = (float)memories[memory] / barMaximum;
            Vector2 targetSize = new Vector2(maxWidth * percentOfMeter , height);
            if (i > 0)
            {
                memoryTransforms[i].localPosition = new Vector2(memoryTransforms[i].localPosition.x, meterOffset);
            }
            memoryTransforms[i].sizeDelta = Vector2.Lerp(memoryTransforms[i].sizeDelta, targetSize, 10f * Time.deltaTime);
            meterOffset += memoryTransforms[i].sizeDelta.x + memoryTransforms[i].localPosition.x;
            i++;
            
        }
    }
}
