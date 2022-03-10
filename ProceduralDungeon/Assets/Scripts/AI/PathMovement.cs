using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathMovement : Movement
{

    private Vector2Int lastPosition;

    private Vector2 nextWaypoint;

    private int waypointChainIndex;
    private List<Vector2Int> waypointChain;

    public float stopDistance;

    private int maxIterations = 20;

    public PathMovement(Transform transform, Rigidbody2D rigidBody, float stopDistance) : base(transform, rigidBody) // poèítání cesty
    {
        this.stopDistance = stopDistance;
        nextWaypoint = new Vector2(transform.position.x, transform.position.y);
        maxIterations = 20;
    }

    public override void Move(Vector2 moveTo, float speed) // pohyb
    {
        Vector2Int find = new Vector2Int(Mathf.FloorToInt(moveTo.x), Mathf.FloorToInt(moveTo.y));

        if (nextWaypoint == null || lastPosition != find)
        {

            lastPosition = find;

            // pathfinding

            Vector2Int startNode = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));

            Dictionary<Vector2Int, Node> nodes = new Dictionary<Vector2Int, Node>();

            nodes.Add(startNode, new Node() { walkDistance = 0, targetDistance = Vector2.Distance(startNode, find), coord = startNode });

            List<Vector2Int> path = new List<Vector2Int>();

            for(int i = 0; i < maxIterations; i++) // poèítání pathfinding
            {
                Vector2Int lowest = LowestCostNode(nodes);
                if(!nodes.ContainsKey(lowest)) // vybírá nejlepší další pozici pro cestu
                {
                    int c = 0;
                    foreach (KeyValuePair<Vector2Int, Node> kv in nodes)
                    {
                        if (!kv.Value.closed) c++;
                    }
                }
                else SearchNode(nodes[lowest], ref nodes);

                if(nodes.ContainsKey(find))
                {
                    maxIterations = 100;
                    WaypointChain(nodes[find], ref path);
                    break;
                }
            }

            if(path.Count > 0)
            {
                path.Reverse();
                waypointChainIndex = 0;
                waypointChain = path;
            }
        }

        // pohyb na bod cesty
        Vector2 position = transform.position;
        bool stop = false;
        if (waypointChain == null) return;
        if (Vector2.Distance(position, nextWaypoint) < 0.08f)
        {
            waypointChainIndex++;
            if(waypointChain.Count <= waypointChainIndex + 1)
            {
                stop = true;
            }
            else
            {
                nextWaypoint = waypointChain[waypointChainIndex] + new Vector2(0.5f, 0.5f);
            }
        }
        if (Vector2.Distance(position, moveTo) >= stopDistance && !stop)
        {
            Vector2 direction = (nextWaypoint - position).normalized;
            rigidBody.velocity = direction * speed;
        }
    }

    void WaypointChain(Node target, ref List<Vector2Int> waypointChain) // øetìzec bodù cesty
    {
        waypointChain.Add(target.coord);
        if (target.parentNode == null) return;
        WaypointChain(target.parentNode, ref waypointChain);
    }

    Vector2Int LowestCostNode(Dictionary<Vector2Int, Node> nodes) // nejlepší další pozice pro pohyb
    {
        float lowest = float.MaxValue;
        Vector2Int best = Vector2Int.zero;
        foreach(KeyValuePair<Vector2Int, Node> kv in nodes)
        {
            if (kv.Value.closed) continue;
            if(kv.Value.walkDistance + kv.Value.targetDistance < lowest)
            {
                lowest = kv.Value.walkDistance + kv.Value.targetDistance;
                best = kv.Key;
            }
        }
        return best;
    }

    void SearchNode(Node node, ref Dictionary<Vector2Int, Node> nodes) // hledání možných pozic pro pohyb
    {
        node.closed = true;

        for (int y = node.coord.y - 1; y <= node.coord.y + 1; y++)
        {
            for (int x = node.coord.x - 1; x <= node.coord.x + 1; x++)
            {
                if (node.coord.x == x && node.coord.y == y) continue;
                Vector2Int coord = new Vector2Int(x, y);

                float wadd = 1;

                if (DungeonGenerator.instance.wallPositions.Contains(coord)) continue;
                if(node.coord.y != y && node.coord.x != x)
                {
                    
                    if (DungeonGenerator.instance.wallPositions.Contains(new Vector2Int(node.coord.x + 1, node.coord.y + 1)) ||
                        DungeonGenerator.instance.wallPositions.Contains(new Vector2Int(node.coord.x - 1, node.coord.y + 1)) ||
                        DungeonGenerator.instance.wallPositions.Contains(new Vector2Int(node.coord.x + 1, node.coord.y - 1)) ||
                        DungeonGenerator.instance.wallPositions.Contains(new Vector2Int(node.coord.x - 1, node.coord.y - 1))) continue;
                    else wadd = 1.4f;
                }
                float walkDistance = wadd + node.walkDistance;
                float targetDistance = Vector2.Distance(coord, lastPosition);

                if (nodes.ContainsKey(coord))
                {
                    if (nodes[coord].walkDistance > walkDistance)
                    {
                        nodes[coord].parentNode = node;
                        nodes[coord].walkDistance = walkDistance;
                    }
                }
                else
                {
                    nodes.Add(coord, new Node() { walkDistance = walkDistance, targetDistance = targetDistance, coord = coord, parentNode = node });
                }
            }
        }

    }

    class Node // tøída pro možné pozice pro cestu
    {

        public Node parentNode;

        public float walkDistance;
        public float targetDistance;
        public Vector2Int coord;

        public bool closed;

    }

}
