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
            node.name = nodes[i].name;
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

        graph.name = name;

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

    // gets the list of nodes that are at the specified level, empty if there are no nodes at that level
    public List<AspectNodeNode> GetNodesAtLevel(int level)
    {
        if (level == 0) return new List<AspectNodeNode>() { GetRootNode() };

        AspectNodeNode currentNode = GetRootNode();
        for(int i = 0; i < level-1; i++)
        {
            if (currentNode.Children.Count == 0)
            {
                Debug.LogError($"No nodes at level {level+1}");
                return currentNode.Children;
            }

            currentNode = currentNode.Children[0];
        }

        if (currentNode.Children.Count == 0) Debug.LogError($"No nodes at level {level + 1}");

        return currentNode.Children;
    }

    // gets total number of levels in the tree
    public int GetTotalLevels()
    {
        int totalLevels = 1;
        AspectNodeNode currentNode = GetRootNode();

        while (currentNode.Children.Count > 0)
        {
            totalLevels++;
            currentNode = currentNode.Children[0];
        }

        return totalLevels;
    }

    // gets the level of the specified node as a Vector2Int, x is the level, y is the index of the node at that level, returns (-1, -1) if the node is not in the tree
    public Vector2Int GetNodeLevel(AspectNodeNode node)
    {
        for(int i = 0; i < GetTotalLevels(); i++)
        {
            List<AspectNodeNode> nodes = GetNodesAtLevel(i);
            if (nodes.Contains(node))
            {
                return new Vector2Int(i, nodes.IndexOf(node));
            }
        }

        return new Vector2Int(-1, -1);
    }
}