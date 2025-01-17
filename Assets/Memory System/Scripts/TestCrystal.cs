using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCrystal : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Replace with your desired key
        {
            MemorySystemInterface.Instance.ActivateAbility();
        }
    }
}
