using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu(menuName = "Aspect Tree")]
public class AspectTree : NodeGraph
{
    public AspectNodeNode GetRootNode()
    {
        foreach (Node node in nodes)
        {
            AspectNodeNode aspectNode = node as AspectNodeNode;
            if (aspectNode != null && aspectNode.Parent == null)
            {
                return aspectNode;
            }
        }

        return null;
    }
}