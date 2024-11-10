using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu(menuName = "Aspect Tree")]
public class AspectTree : NodeGraph
{
    // makes a runtime copy of this aspect tree so that it can be modified without affecting the original
    public AspectTree CreateRuntimeInstance()
    {
        // Instantiate a new nodegraph instance
        AspectTree graph = Instantiate(this);

        // Clear out original node references
        graph.nodes.Clear();

        // Instantiate all nodes inside the graph
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] == null) continue;
            Node.graphHotfix = graph;
            Node node = Instantiate(nodes[i]) as Node;
            node.graph = graph;
            graph.nodes.Add(node);
        }

        // Redirect all connections
        for (int i = 0; i < graph.nodes.Count; i++)
        {
            if (graph.nodes[i] == null) continue;
            foreach (NodePort port in graph.nodes[i].Ports)
            {
                port.Redirect(nodes, graph.nodes);
            }
        }

        // Reinitialize all nodes
        foreach (Node node in graph.nodes)
        {
            AspectNodeNode aspectNode = node as AspectNodeNode;
            aspectNode.ManualInit();
        }

        return graph;
    }

    // gets the node without a parent
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

    // gets the node that was most recently applied, null if none have been applied
    public AspectNodeNode GetMostRecentlyAppliedNode()
    {
        AspectNodeNode currentNode = GetRootNode();

        while (currentNode.Children.Count > 0)
        {
            AspectNodeNode nextNode = null;

            foreach (AspectNodeNode child in currentNode.Children)
            {
                if (child.IsApplied)
                {
                    nextNode = child;
                }
            }

            if (nextNode == null)
            {
                break;
            }

            currentNode = nextNode;
        }

        return currentNode.IsApplied ? currentNode : null;
    }

    // gets the list of the nodes that can be applied next, empty if there are no more
    public List<AspectNodeNode> GetNextUnappliedNodes()
    {
        AspectNodeNode recentlyAppliedNode = GetMostRecentlyAppliedNode();
        if(recentlyAppliedNode == null) 
        {
            return new List<AspectNodeNode>() { GetRootNode() };
        }

        return recentlyAppliedNode.Children;
    }
}