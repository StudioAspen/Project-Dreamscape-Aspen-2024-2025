using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MemorySystem : MonoBehaviour
{
    [SerializeField, Self] private Player player;
    [SerializeField]private int barMaximum; //amount of memory points to fill the memory bar
    Dictionary<string, int> memories = new Dictionary<string, int>(); //dictionary so we can add new memory types whenever


    private void OnEnable()
    {
        //enemies need a memory type and amount property before we can link these together easily..
        //player.OnKillEntity.AddListener(GainMemory())
    }

    private void OnDisable()
    {
        memories.Clear();
    }

    private void GainMemory(string type, int memoryGained) //add memory and check if meter is full 
    {
        if (memories.ContainsKey(type))
        {
            memories[type] += memoryGained;
        }
        else
        {
            memories[type] = memoryGained;
        }
        

        if (MemoryIsFull())
        {
            Debug.Log(MemoryEarned());
        }
    }

    private bool MemoryIsFull() //true if full false if not
    {
        int totalMemory = 0;
        foreach (string memory in memories.Keys)
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

    private string MemoryEarned()
    {
        //not sure how best to order memories in terms of priority in case two have the same value when meter is full
        //OH DESIGNERRRSSS (rings bell)
        //also probably the function we'd bestow the buff/item/whatever it was on
        string memoryType = memories.Max(x => x.Key); //idk how this line works :p
        memories.Clear();
        return memoryType;
    }
}
