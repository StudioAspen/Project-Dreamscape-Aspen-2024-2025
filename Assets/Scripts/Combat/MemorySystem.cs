using KBCore.Refs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MemorySystem : MonoBehaviour
{
    [SerializeField, Self] private Player player;
    [SerializeField]private int barMaximum; //amount of memory points to fill the memory bar
    Dictionary<Type, int> memories = new Dictionary<Type, int>(); //dictionary so we can add new memory types whenever, tied to enemy class ooooo
    [SerializeField]private RectTransform memoryBarTransform;
    private List<Image> memoryTransforms = new List<Image>();
    private Color followerColor = Color.red;
    private Color chargerColor = Color.blue;
    private Color defaultColor = Color.white;

    private void OnValidate()
    {
        this.ValidateRefs();
    }


    private void OnEnable()
    {
        //enemies need a memory type and amount property before we can link these together easily..
        player.OnKillEntity.AddListener(Player_OnKillEntity);
    }

    private void OnDisable()
    {
        player.OnKillEntity.RemoveListener(Player_OnKillEntity);
        memories.Clear();
    }

    private void Player_OnKillEntity(Entity victim) //add memory and check if meter is full 
    {
        Type type = victim.GetType();


        if (memories.ContainsKey(type))
        {
            memories[type]++;
        }
        else
        {
            memories[type] = 1;
        }
        UpdateDebugMeter();

        if (MemoryIsFull())
        {
            Debug.Log(memories.Values.Max());
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
        Type memoryType = memories.Max(x => x.Key.GetType()); //idk how this line works :p
        memories.Clear();
        return memoryType;
    }

    private void UpdateDebugMeter()
    {

        int i = 0;
        float meterOffset = 0f;
        foreach(Type memory in memories.Keys)
        {
            //if theres more unique memory types than there are bars, add a new one
            if( i+1 > memoryTransforms.Count)
            {
                memoryTransforms.Add(Instantiate(memoryBarTransform, memoryBarTransform,true).GetComponent<Image>());
                //match color to type of enemy
                if(memory == typeof(Follower))
                {
                    memoryTransforms[i].color = followerColor;
                }
                else if(memory == typeof(Charger))
                {
                    memoryTransforms[i].color = chargerColor;
                }
                else
                {
                    memoryTransforms[i].color = defaultColor;
                }
                
            }

            float percentOfMeter = (float)memories[memory] / barMaximum;
            memoryTransforms[i].fillAmount = percentOfMeter + meterOffset;
            meterOffset += memoryTransforms[i].fillAmount;
            i++;
        }
    }
}
