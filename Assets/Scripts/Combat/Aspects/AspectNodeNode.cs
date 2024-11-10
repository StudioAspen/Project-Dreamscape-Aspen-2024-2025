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
        AspectNodeNode parent = GetInputValue<AspectNodeNode>("Parent");
        if (parent == null) return;

        Parent = parent;
    }

    public virtual void ApplyAspect()
    {
        IsApplied = true;
    }
}

public class ComboAspectNodeNode : AspectNodeNode
{
    [field: Header("Combo")]
    [field: SerializeField] public ComboDataSO ComboData { get; private set; }

    public override void ApplyAspect()
    {
        base.ApplyAspect();
    }
}

public class AugmentAspectNodeNode : AspectNodeNode
{
    public override void ApplyAspect()
    {
        base.ApplyAspect();
    }
}

public class BuffAspectNodeNode : AspectNodeNode
{
    public override void ApplyAspect()
    {
        base.ApplyAspect();
    }
}
