# pathfinding
Re-visited pathfinding. Implemented the manhattan heuristic in Unity3D.

## How does it work?

This project is entirely based off the following

[Wikipedia A *](https://en.wikipedia.org/wiki/A*_search_algorithm)

[Manhattan Heuristic](https://heuristicswiki.wikispaces.com/Manhattan+Distance)

Taking the Wikipedia psuedo code the project is a C# Unity3D implementation of it.

## Where is the code?

Assets/Scripts/Main.cs

and

Assets/Scripts/Node.cs

***But here is the code regardless***

### Main.cs

```
using System;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class Main : MonoBehaviour {

    public GameObject tileBlock;
    public GameObject wallBlock;
    public GameObject startBlock;
    public GameObject finishBlock;
    public GameObject gridObject;

    const int GRIDSIZE = 15;
    const int MAXBLOCKED = 25;
    const float BLOCK_HEIGHT = 1.2f;
    const float START_FINISH_HEIGHT = 0.8f;
    const float OFFSET = -0.5f;

    Node[,] gridNodes = new Node[15,15];
    List<Node> blockedNodeList = new List<Node>();
    bool notPlaced = true;
    int blockedCount = 0;

    void Start()
    {
        for (int i = 0; i < GRIDSIZE; ++i)
        {
            for (int j = 0; j < GRIDSIZE; ++j)
            {
                bool spaceBlocked = false;

                gridNodes[i, j] = new Node(i, j);

                if (UnityEngine.Random.Range(0, 30) < 2 && blockedCount < MAXBLOCKED)
                {
                    if (i != 0 && j != GRIDSIZE && i != GRIDSIZE && j != 0)
                    {
                        GameObject temp = Instantiate(wallBlock, new Vector3(i + OFFSET, BLOCK_HEIGHT, j + OFFSET), Quaternion.identity);
                        temp.name = "Obstacle: " + i + " : " + j;
                        temp.gameObject.transform.SetParent(gridObject.transform);

                        gridNodes[i, j].isBlocked = true;
                        blockedNodeList.Add(gridNodes[i,j]);
                        ++blockedCount;

                        spaceBlocked = true;
                    }
                }
                
                if(!spaceBlocked)
                {
                    GameObject temp = Instantiate(tileBlock, new Vector3(i + OFFSET, 0.2f, j + OFFSET), Quaternion.identity);
                    temp.name = "Tile: " + i + " : " + j;
                    temp.gameObject.transform.SetParent(gridObject.transform);
                }
            }
        }

        for (int i = 0; i < GRIDSIZE; ++i)
        {
            for (int j = 0; j < GRIDSIZE; ++j)
            {
                gridNodes[i, j].findNeighbours(gridNodes);
            }
        }

        int x = UnityEngine.Random.Range(0, GRIDSIZE);
        int z = UnityEngine.Random.Range(0, GRIDSIZE);

        Node start = null;
        Node finish = null;

        while (notPlaced)
        {
            if (gridNodes[x, z].isBlocked == false)
            {
                GameObject temp = Instantiate(startBlock, new Vector3(x + OFFSET, START_FINISH_HEIGHT, z + OFFSET), Quaternion.identity);
                temp.name = "Start: " + x + " : " + z;
                temp.gameObject.transform.SetParent(gridObject.transform);

                gridNodes[x, z].isStart = true;
                start = gridNodes[x, z];

                notPlaced = false;
            }

            x = UnityEngine.Random.Range(0, GRIDSIZE);
            z = UnityEngine.Random.Range(0, GRIDSIZE);
        }

        notPlaced = true;

        x = UnityEngine.Random.Range(0, GRIDSIZE);
        z = UnityEngine.Random.Range(0, GRIDSIZE);

        while (notPlaced)
        {
            if (gridNodes[x, z].isBlocked == false)
            {
                GameObject temp = Instantiate(finishBlock, new Vector3(x + OFFSET, START_FINISH_HEIGHT, z + OFFSET), Quaternion.identity);
                temp.name = "Finish: " + x + " : " + z;
                temp.gameObject.transform.SetParent(gridObject.transform);
                
                gridNodes[x, z].isFinish = true;
                finish = gridNodes[x, z];

                notPlaced = false;
            }

            x = UnityEngine.Random.Range(0, GRIDSIZE);
            z = UnityEngine.Random.Range(0, GRIDSIZE);
        }

        List<Node> path = PathFinding(start,finish, blockedNodeList);        
        DrawPath(path);
    }

    void DrawPath(List<Node> path)
    {
        List<Vector3> pathpoints = new List<Vector3>();

        foreach (Node n in path)
            pathpoints.Add(new Vector3(n.x + OFFSET, 0.5f, n.z + OFFSET));

        VectorLine pathLine = new VectorLine("PathLine", pathpoints, 5.0f);
        pathLine.lineType = Vectrosity.LineType.Continuous;
        pathLine.color = Color.blue;
        pathLine.Draw3D();
    }
    
    Node OpenLowestFCost(List<Node> open)
    {
        Node lowest = null;

        foreach (Node n in open)
        {
            if (lowest == null)
                lowest = n;

            if (n.fScore < lowest.fScore)
                lowest = n;
        }

        return lowest;
    }

    float heuristicManhattan(Node current, Node finish)
    {
        int val = current.x - finish.x;
        int otherVal = current.z - finish.z;

        return (val + otherVal);
    }

    List<Node> PathFinding(Node startNode, Node finishNode, List<Node> blockedNodeList)
    {
        List<Node> pathPoints = new List<Node>();

        List<Node> closedNodeList = new List<Node>();
        List<Node> openNodeList = new List<Node>();
        Node currentNode = null;

        startNode.gScore = 0;
        startNode.fScore = startNode.gScore + heuristicManhattan(startNode, finishNode);

        openNodeList.Add(startNode);

        foreach(Node n in blockedNodeList)
        {
            closedNodeList.Add(n);
        }

        while (openNodeList.Count != 0)
        {
            currentNode = OpenLowestFCost(openNodeList);
            
            if (currentNode.isFinish)
                return buildPath(finishNode);

            openNodeList.Remove(currentNode);
            closedNodeList.Add(currentNode);

            foreach (Node neighbourNode in currentNode.neighbours)
            {
                if (closedNodeList.Contains(neighbourNode) == false){

                    float currentGScore = currentNode.gScore + currentNode.getDistance(neighbourNode);

                    if (openNodeList.Contains(neighbourNode) == false)
                        openNodeList.Add(neighbourNode);
                    else if (currentGScore >= neighbourNode.gScore)
                        continue;

                    neighbourNode.parentNode = currentNode;
                    neighbourNode.gScore = currentGScore;
                    neighbourNode.fScore = neighbourNode.gScore + heuristicManhattan(neighbourNode,finishNode);
                }
            }
        }

        return pathPoints;
    }
    
    List<Node> buildPath(Node node)
    {
        List<Node> path = new List<Node>();

        while (node.parentNode != null)
        {
            path.Add(node);
            node = node.parentNode;
        }

        path.Add(node);

        return path;
    }
}

```

### Node.cs

```
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
```

### Attribution

* Uses [Vectrosity](https://starscenesoftware.com/vectrosity.html) to draw the line
