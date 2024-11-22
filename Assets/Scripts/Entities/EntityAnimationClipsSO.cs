using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Entity Animation Clips", order = 1)]
public class EntityAnimationClipsSO : ScriptableObject
{
    [field: SerializedDictionary("Name", "Animation Clips")]
    [field: SerializeField] public SerializedDictionary<string, AnimationClip> Clips { get; protected set; }

    [SerializeField, Tooltip("Click to reset to default values")] public bool resetToDefault;

    private void OnValidate()
    {
        if (resetToDefault)
        {
            resetToDefault = false;
            GenerateDefault();
        }
    }

    private void GenerateDefault()
    {
        Clips.Clear();
        Clips.Add("Idle", null);
        Clips.Add("Walk", null);
        Clips.Add("Run", null);
        Clips.Add("Fall", null);
        Clips.Add("Jump", null);
        Clips.Add("HurtFall", null);
        Clips.Add("HurtFallImpact", null);
        Clips.Add("Death", null);
        Clips.Add("Stagger", null);
    }
}