using UnityEngine;
using System.Collections.Generic;

class Node
{
    /* Variables */

    public Node parentNode { get; set; }
    public List<Node> neighbours { get; set; }

    public int x { get; set; }
    public int z { get; set; }

    public float gScore { get; set; }
    public float fScore { get; set; }
    public int hScore { get; set; }

    public bool isBlocked { get; set; }
    public bool isStart { get; set; }
    public bool isFinish { get; set; }

    /* Contructors */

    public Node(int x, int z)
    {
        this.gScore = 1;
        this.x = x;
        this.z = z;
    }

    /* Methods */

    public Vector3 currentPosition()
    {
        return new Vector3(this.x, 0, this.z);
    }

    public float getDistance(Node otherNode)
    {
        return (Vector3.Distance(otherNode.currentPosition(), this.currentPosition()));
    }

    public void findNeighbours(Node[,] grid)
    {
        this.neighbours = new List<Node>();

        // Top Middle

        try
        {
            this.neighbours.Add(grid[x - 1, z]);
        }
        catch { }

        // Bottom Middle

        try
        {
            this.neighbours.Add(grid[x + 1, z]);
        }
        catch { }

        // Middle Left

        try
        {
            this.neighbours.Add(grid[x, z + 1]);
        }
        catch { }

        // Middle Right

        try
        {
            this.neighbours.Add(grid[x, z - 1]);
        }
        catch { }
    }
}
