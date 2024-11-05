using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSoundAugment : Augment
{
    private AudioSource audioSource;

    private bool soundTriggered = false;

    [Header("Augment Parameters")]
    [SerializeField] private AudioClip jumpSound;

    protected void Awake()
    {
        Branch = AugmentBranch.MARIO_BRANCH;
        Level = 1;
    }


    protected override void Start()
    {
        base.Start();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = jumpSound;
    }

    // Update is called once per frame
    private void Update()
    {
        if (player.IsJumping && !soundTriggered)
        {
            audioSource.Play();
            soundTriggered = true;
            Debug.Log("JUMP PLAYED");
        }
        else if (!player.IsJumping && soundTriggered)
        {
            soundTriggered = false;
        }
    }
}
