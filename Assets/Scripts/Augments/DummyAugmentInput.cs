using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyAugmentInput : MonoBehaviour
{
    private AugmentManager augmentManager;

    // Start is called before the first frame update
    void Start()
    {
        augmentManager = FindAnyObjectByType<AugmentManager>();
        augmentManager.IncrementLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            Debug.Log(augmentManager.AddAugment<JumpSoundAugment>() ? "[DummyAugmentInput] jump sound add success" : "[DummyAugmentInput] jump sound add failure");
        }
        if (Input.GetKeyDown("o"))
        {
            Debug.Log(augmentManager.AddAugment<SimpleLifestealAugment>() ? "[DummyAugmentInput] lifesteal add success" : "[DummyAugmentInput] lifesteal add failure");
        }
        if (Input.GetKeyDown("i"))
        {
            Debug.Log(augmentManager.AddAugment<AOEAugment>() ? "[DummyAugmentInput] aoe add success" : "[DummyAugmentInput] aoe add failure");
        }
    }
}
