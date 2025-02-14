using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectsManager : MonoBehaviour
{
    private LevelSystem levelSystem;

    [SerializeField] private List<AspectTree> aspectTrees = new List<AspectTree>();

    public AspectTree CurrentAspectTree { get; private set; }
    public int AspectTokens { get; private set; } = 0;

    private void Awake()
    {
        levelSystem = GetComponent<LevelSystem>();

        levelSystem.OnLevelUp += LevelSystem_OnLevelUp;
    }

    private void OnDestroy()
    {
        levelSystem.OnLevelUp -= LevelSystem_OnLevelUp;
    }

    private void LevelSystem_OnLevelUp(int newLevel)
    {
        AspectTokens++;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && CurrentAspectTree == null)
        {
            SetCurrentAspectTree(aspectTrees[0]);
        }
    }

    /// <summary>
    /// Sets the current aspect tree to the specified runtime instance of the aspect tree.
    /// </summary>
    /// <param name="aspectTree">The aspect tree to set as the current aspect tree.</param>
    public void SetCurrentAspectTree(AspectTree aspectTree)
    {
        CurrentAspectTree = aspectTree.CreateRuntimeInstance();

        Debug.Log($"Set current aspect tree to {CurrentAspectTree.name}");
    }

    public void ConsumeAspectToken()
    {
        AspectTokens--;
    }
}
