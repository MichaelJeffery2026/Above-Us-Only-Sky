using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour
{
    private Tilemap walkableTilemap; // Assign the tilemap that marks walkable areas
    public string tagName; // Assign the target GameObject dynamically
    public float moveSpeed = 5f; // Movement speed

    private List<Vector3> path = new List<Vector3>(); // Stores the computed path
    private int pathIndex = 0; // Tracks the current waypoint

    private Vector3 lastTargetPosition = new Vector3 (0.0f, 0.0f, 0.0f);
    private GameObject target;

    private SpriteRenderer myRenderer;

    private void Start()
    {
        walkableTilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
        myRenderer = this.GetComponent<SpriteRenderer>();
    }


    void Update()
    {
        if (tagName == null) return;
        if (target != null && (target.transform.position.x < transform.position.x))
        {
            myRenderer.flipX = true;
        }
        else
        {
            myRenderer.flipX = false;
        }

        if (pathIndex == 2 || target == null || path.Count == 0 || pathIndex >= path.Count || lastTargetPosition != target.transform.position) //Path not reevaluted until destination is reached (may change)
        {
            GameObject[] towers = GameObject.FindGameObjectsWithTag(tagName);
            int shortestPathLength = 255;
            int shortestPathIndex = 0;

            for (int i = 0; i < towers.Length; i++)
            {
                int currPathLength = PathLength(transform.position, towers[i].transform.position);
                if (currPathLength < shortestPathLength)
                {
                    shortestPathLength = currPathLength;
                    shortestPathIndex = i;
                }
            }
            if (towers.Length != 0)
            {
                target = towers[shortestPathIndex];
                lastTargetPosition = target.transform.position;
                FindPath(transform.position, towers[shortestPathIndex].transform.position);
            }
        }

        MoveAlongPath();
    }

    private int PathLength(Vector3 startPos, Vector3 targetPos)
    {
        Vector3Int start = walkableTilemap.WorldToCell(startPos);
        Vector3Int goal = walkableTilemap.WorldToCell(targetPos);

        if (!IsWalkable(goal))
        {
            Debug.Log("Target position is not walkable!");
            return 255;
        }

        List<Vector3Int> gridPath = AStarAlgorithm(start, goal);
        if (gridPath == null)
        {
            return 255;
        }
        return gridPath.Count;
    }

    private void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Vector3Int start = walkableTilemap.WorldToCell(startPos);
        Vector3Int goal = walkableTilemap.WorldToCell(targetPos);

        if (!IsWalkable(goal))
        {
            Debug.Log("Target position is not walkable!");
            return;
        }

        List<Vector3Int> gridPath = AStarAlgorithm(start, goal);

        if (gridPath != null && gridPath.Count > 0)
        {
            path.Clear();
            pathIndex = 0;
            foreach (Vector3Int tilePos in gridPath) 
            {
                path.Add(walkableTilemap.GetCellCenterWorld(tilePos)); // Convert to world position
            }
        }
    }

    private List<Vector3Int> AStarAlgorithm(Vector3Int start, Vector3Int goal)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();
        PriorityQueue<Node> openSet = new PriorityQueue<Node>();

        Dictionary<Vector3Int, Node> allNodes = new Dictionary<Vector3Int, Node>();

        Node startNode = new Node(start, 0, GetHeuristic(start, goal), null);
        openSet.Enqueue(startNode);
        allNodes[start] = startNode;

        while (openSet.Count > 0)
        {
            Node current = openSet.Dequeue();

            if (current.Position == goal)
            {
                return RetracePath(current);
            }

            closedSet.Add(current.Position);

            foreach (Vector3Int neighbor in GetNeighbors(current.Position))
            {
                if (closedSet.Contains(neighbor) || !IsWalkable(neighbor))
                    continue;

                float newGCost = current.GCost + Vector3Int.Distance(current.Position, neighbor);
                if (!allNodes.ContainsKey(neighbor) || newGCost < allNodes[neighbor].GCost)
                {
                    Node neighborNode = new Node(neighbor, newGCost, GetHeuristic(neighbor, goal), current);
                    allNodes[neighbor] = neighborNode;
                    openSet.Enqueue(neighborNode);
                }
            }
        }

        return null; // No path found
    }

    private List<Vector3Int> RetracePath(Node node)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        while (node != null)
        {
            path.Add(node.Position);
            node = node.Parent;
        }
        path.Reverse();
        return path;
    }

    private List<Vector3Int> GetNeighbors(Vector3Int position)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>
        {
            /*
            new Vector3Int(position.x + 1, position.y, 0),
            new Vector3Int(position.x - 1, position.y, 0),
            new Vector3Int(position.x, position.y + 1, 0),
            */
            new Vector3Int(position.x, position.y + 1, 0),
            // new Vector3Int(position.x + 1, position.y + 1, 0),
            new Vector3Int(position.x + 1, position.y, 0),
            // new Vector3Int(position.x + 1, position.y - 1, 0),
            new Vector3Int(position.x, position.y - 1, 0),
            // new Vector3Int(position.x - 1, position.y - 1, 0),
            new Vector3Int(position.x - 1, position.y, 0),
            // new Vector3Int(position.x - 1, position.y + 1, 0),
        };
        return neighbors;
    }

    private bool IsWalkable(Vector3Int position)
    {
        return walkableTilemap.HasTile(position); // Only considers tiles in the walkable tilemap
    }

    private float GetHeuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // Manhattan distance heuristic
    }

    private void MoveAlongPath()
    {
        if (path.Count == 0 || pathIndex >= path.Count) return;

        Vector3 targetPos = path[pathIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            pathIndex++;
        }
    }

    public GameObject getTarget()
    {
        return target;
    }

    public bool hasTarget()
    {
        return (target != null);
    }
}

// Priority Queue for managing nodes
public class PriorityQueue<T> where T : Node
{
    private List<T> items = new List<T>();

    public int Count => items.Count;

    public void Enqueue(T item)
    {
        items.Add(item);
        items.Sort((a, b) => (a.FCost).CompareTo(b.FCost));
    }

    public T Dequeue()
    {
        T item = items[0];
        items.RemoveAt(0);
        return item;
    }
}

// Node class for pathfinding
public class Node
{
    public Vector3Int Position { get; }
    public float GCost { get; }
    public float HCost { get; }
    public float FCost => GCost + HCost;
    public Node Parent { get; }

    public Node(Vector3Int position, float gCost, float hCost, Node parent)
    {
        Position = position;
        GCost = gCost;
        HCost = hCost;
        Parent = parent;
    }
}