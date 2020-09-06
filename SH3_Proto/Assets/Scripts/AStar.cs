using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public static class AStar
{
    public class Node
    {
        public static Vector2Int target;

        public Node(Vector2Int position, Node fromNode)
        {
            this.position = position;
            from = fromNode;
            if (from != null)
            {
                distanceTraveled = fromNode.distanceTraveled + 1f;
            }
            else distanceTraveled = 0f;

            float xDist = (float)(position.x - target.x);
            float yDist = (float)(position.y - target.y);
            distanceFromTarget = Mathf.Sqrt(
                xDist * xDist + yDist * yDist);
        }

        public Vector2Int position;
        public Node from;
        public float distanceTraveled;
        public float distanceFromTarget;
        public float Score
        {
            get => distanceTraveled + distanceFromTarget;
        }
    }

    public static List<Node> Path(Tilemap walls, int width, int height,
        Vector2Int start, Vector2Int target, Tilemap floor)
    {
        // Automatic failure if target is a wall

        if (walls.GetTile(new Vector3Int(target.x, target.y, 0)) != null)
            return null;

        List<Node> fringe = new List<Node>();
        List<Node> closed = new List<Node>();
        Node.target = target;

        // Populate fringe with startNode

        fringe.Add(new Node(start, null));

        // Main loop

        int safetyCounter = 0;

        while (fringe.Count > 0)
        {
            if (safetyCounter++ > 50000)
            {
                Debug.Log("A* Time Out");
                return null;
            }

            Node currentNode = FindBestNode(fringe);

            if (currentNode.distanceFromTarget == 0f)
            {
                // Path found...

                List<Node> path = new List<Node>();
                Node traceNode = currentNode;
                path.Add(traceNode);

                while(true)
                {
                    if (traceNode.from == null) return path;
                    traceNode = traceNode.from;
                    path.Add(traceNode);
                }
            }

            ExpandFringe(fringe, currentNode, closed,
                walls, width, height);
        }

        return null;
    }

    static Node FindBestNode(List<Node> nodes)
    {
        Node bestNode = nodes[0];
        foreach (var node in nodes)
        {
            if (node.Score < bestNode.Score) bestNode = node;
        }
        return bestNode;
    }

    static void ExpandFringe(List<Node> fringe, Node currentNode,
        List<Node> closed, Tilemap walls, int width, int height)
    {
        // Check four candidate positions...

        //N
        var candidate = currentNode.position;
        candidate.y += 1;
        if (CheckCandidate(candidate, walls, width, height, fringe, closed)) 
            fringe.Add(new Node(candidate, currentNode));

        candidate = currentNode.position;
        candidate.y -= 1;
        if (CheckCandidate(candidate, walls, width, height, fringe, closed))
            fringe.Add(new Node(candidate, currentNode));

        candidate = currentNode.position;
        candidate.x += 1;
        if (CheckCandidate(candidate, walls, width, height, fringe, closed))
            fringe.Add(new Node(candidate, currentNode));

        candidate = currentNode.position;
        candidate.x -= 1;
        if (CheckCandidate(candidate, walls, width, height, fringe, closed))
            fringe.Add(new Node(candidate, currentNode));

        closed.Add(currentNode);
        fringe.Remove(currentNode);
    }

    static bool CheckCandidate(Vector2Int candidate, Tilemap walls, int width,
        int height, List<Node> fringe, List<Node> closed)
    {
        if (candidate.x < 0 || candidate.x >= width ||
            candidate.y < 0 || candidate.y >= height)
            return false;

        if (walls.GetTile(new Vector3Int(candidate.x,
            candidate.y, 0)) != null) return false;

        var result = closed.FindAll(x => x.position == candidate);
        if (result.Count > 0) return false;

        result = fringe.FindAll(x => x.position == candidate);
        if (result.Count > 0) return false;

        return true;
    }
    
}
