using System.Collections.Generic;
using UnityEngine;
using XNode;

public class AspectNodeNode : Node
{
    [Input] public AspectNodeNode Parent;
    [Output(connectionType = ConnectionType.Multiple)] public List<AspectNodeNode> Children;

    [field: Header("Display")]
    [field: SerializeField] public string DisplayName { get; private set; } = "Aspect Node";
    [field: SerializeField, TextArea(5, 20)] public string Description { get; private set; } = "Aspect Node Description";

    public bool IsApplied { get; protected set; }

    // updates the node connections in the editor
    private void OnValidate()
    {
        ManualInit();
    }

    // xNodes calls this OnEnable()
    protected override void Init()
    {
        ManualInit();
    }

    /// <summary>
    /// Initializes the AspectNodeNode by assigning the parent and children nodes.
    /// Manually called when the graph is cloned in runtime
    /// </summary>
    public void ManualInit()
    {
        AssignParent();
        AssignChildren();
    }

    /// <summary>
    /// Assigns the children nodes by iterating through the connections of the "Children" output port.
    /// </summary>
    private void AssignChildren()
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

    /// <summary>
    /// Assigns the parent node by retrieving the connections of the "Parent" input port.
    /// </summary>
    private void AssignParent()
    {
        List<NodePort> parentConnections = GetInputPort("Parent").GetConnections();
        if(parentConnections.Count == 0) return;

        Parent = parentConnections[0].node as AspectNodeNode;
    }

    /// <summary>
    /// Applies the aspect to the node.
    /// Override this function for custom logic when applying the aspect.
    /// </summary>
    /// <param name="aspectsManager">The aspects manager.</param>
    public virtual void ApplyAspect(AspectsManager aspectsManager)
    {
        IsApplied = true;
    }

    // not being used but all xNodes need this override
    public override object GetValue(NodePort port)
    {
        return null;
    }
}
