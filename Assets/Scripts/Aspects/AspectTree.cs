using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

[CreateAssetMenu(menuName = "Aspect Tree")]
public class AspectTree : NodeGraph
{
    [field: Header("Display")]
    [field: SerializeField] public string DisplayName { get; private set; } = "Aspect Tree";
    [field: SerializeField, TextArea(5, 20)] public string Description { get; private set; } = "Description";
    [field: SerializeField] public Sprite AspectSprite { get; private set; }
    [field: SerializeField] public Color AspectTextColor { get; private set; } = Color.white;

    /// <summary>
    /// Makes a runtime copy of this aspect tree so that it can be modified without affecting the original.
    /// </summary>
    /// <returns>The created runtime instance of the aspect tree.</returns>
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

    /// <summary>
    /// Gets the node without a parent.
    /// </summary>
    /// <returns>The root node.</returns>
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

    /// <summary>
    /// Gets the node that was most recently applied.
    /// Returns null if none have been applied.
    /// </summary>
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

    /// <summary>
    /// Gets the list of the nodes that can be applied next, empty if there are no more.
    /// </summary>
    /// <returns>The list of nodes that can be applied next.</returns>
    public List<AspectNodeNode> GetNextUnappliedNodes()
    {
        AspectNodeNode recentlyAppliedNode = GetMostRecentlyAppliedNode();
        if (recentlyAppliedNode == null)
        {
            return new List<AspectNodeNode>() { GetRootNode() };
        }

        return recentlyAppliedNode.Children;
    }

    /// <summary>
    /// Gets the list of nodes that are at the specified level, empty if there are no nodes at that level.
    /// The node that is the highest in the graph is the first index of the list.
    /// </summary>
    /// <param name="level">The level of the nodes to retrieve.</param>
    /// <returns>The list of nodes at the specified level.</returns>
    public List<AspectNodeNode> GetNodesAtLevel(int level)
    {
        if (level == 0) return new List<AspectNodeNode>() { GetRootNode() };

        AspectNodeNode currentNode = GetRootNode();
        for (int i = 0; i < level - 1; i++)
        {
            if (currentNode.Children.Count == 0)
            {
                Debug.LogError($"No nodes leading to level {level + 1}");
                return currentNode.Children;
            }

            currentNode = currentNode.Children[0];
        }

        if (currentNode.Children.Count == 0) Debug.LogError($"No nodes at level {level + 1}");

        return currentNode.Children.OrderByDescending(child => child.position.y).ToList();
    }

    /// <summary>
    /// Gets the total number of levels in the tree.
    /// </summary>
    /// <returns>The total number of levels.</returns>
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

    /// <summary>
    /// Gets the level of the specified node as a Vector2Int, where x is the level and y is the index of the node at that level.
    /// Returns (-1, -1) if the node is not in the tree.
    /// </summary>
    /// <param name="node">The node to get the level for.</param>
    /// <returns>The level of the node as a Vector2Int.</returns>
    public Vector2Int GetNodeLevel(AspectNodeNode node)
    {
        for (int i = 0; i < GetTotalLevels(); i++)
        {
            List<AspectNodeNode> nodes = GetNodesAtLevel(i);
            if (nodes.Contains(node))
            {
                return new Vector2Int(i, nodes.IndexOf(node));
            }
        }

        return new Vector2Int(-1, -1);
    }

    /// <summary>
    /// Gets the levels at which the aspect tree has more than one node.
    /// </summary>
    /// <returns>A list of levels at which the aspect tree branches.</returns>
    public List<int> GetMultiNodeLevels()
    {
        List<int> levels = new List<int>();

        for (int i = 0; i < GetTotalLevels(); i++)
        {
            List<AspectNodeNode> aspectNodes = GetNodesAtLevel(i);

            if (aspectNodes.Count > 1) levels.Add(i);
        }

        return levels;
    }

    /// <summary>
    /// Checks if the specified node is part of a multi-node level in the aspect tree.
    /// </summary>
    /// <param name="node">The node to check.</param>
    /// <returns>True if the node is part of a multi-node level, false otherwise.</returns>
    public bool IsNodePartOfMultiNodeLevel(AspectNodeNode node)
    {
        return GetMultiNodeLevels().Contains(GetNodeLevel(node).x);
    }

    /// <summary>
    /// Checks if a node in a multi-node level can be chosen.
    /// </summary>
    /// <param name="node">The node to check.</param>
    /// <returns>True if the node can be chosen, false otherwise.</returns>
    public bool CanMultiNodeLevelNodeBeChosen(AspectNodeNode node)
    {
        if (!IsNodePartOfMultiNodeLevel(node)) return true;

        Vector2Int currentNodePosition = GetNodeLevel(node);
        List<int> levelsWithMultiNodes = GetMultiNodeLevels();

        // if the node is part of the first instance of multi-node level
        if (currentNodePosition.x == levelsWithMultiNodes[0]) return true;

        // find and get applied node's y at the first level
        int appliedNodeY = -1;
        foreach (AspectNodeNode firstMultiNodeLevelNode in GetNodesAtLevel(levelsWithMultiNodes[0]))
        {
            if (firstMultiNodeLevelNode.IsApplied)
            {
                appliedNodeY = GetNodeLevel(firstMultiNodeLevelNode).y;
                break;
            }
        }

        // if the node is at the same y as the applied node at the first multi-node level, it can be chosen and isnt locked out
        return currentNodePosition.y == appliedNodeY;
    }
}