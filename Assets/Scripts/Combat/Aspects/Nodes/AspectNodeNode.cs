using System.Collections.Generic;
using UnityEngine;
using XNode;

public class AspectNodeNode : Node
{
    [Input] public AspectNodeNode Parent;
    [Output(connectionType = ConnectionType.Multiple)] public List<AspectNodeNode> Children;

    public bool IsApplied { get; protected set; }

    private void OnValidate()
    {
        GetParent();
        GetChildren();
    }

    protected override void Init()
    {
        GetParent();
        GetChildren();
    }

    private void GetChildren()
    {
        Children = new List<AspectNodeNode>();
        foreach (NodePort port in GetOutputPort("Children").GetConnections())
        {
            AspectNodeNode childNode = port.node as AspectNodeNode;
            if (childNode != null)
            {
                Children.Add(childNode);
            }
        }
    }

    private void GetParent()
    {
        List<NodePort> parentConnections = GetInputPort("Parent").GetConnections();
        if(parentConnections.Count == 0) return;

        Parent = parentConnections[0].node as AspectNodeNode;
    }

    public virtual void ApplyAspect(AspectsManager aspectsManager)
    {
        IsApplied = true;
    }

    public override object GetValue(NodePort port)
    {
        return null;
    }
}
