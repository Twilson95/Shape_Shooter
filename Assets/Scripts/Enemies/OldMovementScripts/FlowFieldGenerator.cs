// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class FlowFieldGenerator : MonoBehaviour
// {
//     public float fieldSize = 10f; // The size of the flow field
//     public float gridSize = 1f; // The size of each grid cell in the flow field
//     public LayerMask obstacleLayer; // The layer containing the obstacles
//     public int maxDistance = 10; // The maximum distance to generate the flow field for
//     public Vector2Int playerGridPos; // The player's position in the flow field
//     public Transform player;
//     private float timer;
//     private int gridSizeX;
//     private int gridSizeY;
//     public float interval;
//     public Vector2 startPos;

//     private Vector2[,] flowField; // The flow field data

//     private void Start()
//     {
//         GenerateFlowField();
//     }

//     private void Update() 
//     {
//         //every n seconds regenerate flow field
//         timer += Time.deltaTime;
//         if(timer >= interval)
//         {
//             GenerateFlowField();
//             timer = 0;
//         }
//     }

//     public void GenerateFlowField()
//     {
//         GetPlayerPosition();
//         // Initialize the flow field data
//         gridSizeX = Mathf.FloorToInt(fieldSize / gridSize);
//         gridSizeY = Mathf.FloorToInt(fieldSize / gridSize);
//         flowField = new Vector2[gridSizeX, gridSizeY];

//         startPos = new Vector2(Mathf.Round(player.player.position.x / gridSize) * gridSize - gridSize * (fieldSize / 2 - 0.5f),
//                        Mathf.Round(player.player.position.y / gridSize) * gridSize - gridSize * (fieldSize / 2 - 0.5f));
//         // Generate the flow field for each cell in the grid
//         for (int x = 0; x < gridSizeX; x++)
//         {
//             for (int y = 0; y < gridSizeY; y++)
//             {
//                 // Get the world position of the grid cell
//                 Vector2 worldPos = startPos + new Vector2(x * gridSize, y * gridSize);
            
//                 // Check if the grid cell is obstructed by an obstacle
//                 if (Physics2D.OverlapBox(worldPos, new Vector2(gridSize, gridSize), 0f, obstacleLayer))
//                 {
//                     flowField[x, y] = Vector2.zero; // Obstructed cells have zero flow
//                 }
//                 else
//                 {
//                     // Generate the flow direction for the cell
//                     Vector2 dirToPlayer = playerGridPos - new Vector2Int(x, y);
//                     flowField[x, y] = dirToPlayer.normalized;
//                 }
//             }
//         }
//     }

//     private void GetPlayerPosition(){
//         Vector3 playerPosition = player.player.position;
//         int gridX = Mathf.FloorToInt(playerPosition.x / gridSize);
//         int gridY = Mathf.FloorToInt(playerPosition.y / gridSize);
//         playerGridPos = new Vector2Int(gridX, gridY);
//     }

//     // Get the flow direction for a given world position
//     public Vector2 GetFlowDirection(Vector2 worldPos)
//     {
//         // Convert the world position to a grid position
//         Vector2 localPos = worldPos - (Vector2) startPos;
//         Vector2Int gridPos = new Vector2Int(
//             Mathf.Clamp(Mathf.FloorToInt(localPos.x / gridSize), 0, flowField.GetLength(0) - 1),
//             Mathf.Clamp(Mathf.FloorToInt(localPos.y / gridSize), 0, flowField.GetLength(1) - 1)
//         );

//         // Get the flow direction for the grid cell
//         return flowField[gridPos.x, gridPos.y];
//     }

//     private void OnDrawGizmos()
//     {
//         Gizmos.color = Color.white;

//         for (int x = 0; x < gridSizeX; x++)
//         {
//             for (int y = 0; y < gridSizeY; y++)
//             {
//                 Vector2 pos = startPos + new Vector2(x * gridSize, y * gridSize);
//                 Vector2 dir = GetFlowDirection(pos);

//                 Gizmos.DrawLine(pos, pos + dir.normalized * 0.5f);
//             }
//         }
//     }
// }

using System.Collections.Generic;
using UnityEngine;

public class FlowFieldGenerator : MonoBehaviour
{
    public int gridSizeX = 20;
    public int gridSizeY = 20;
    public float gridSize = 0.5f;
    //public Vector2 mapOffset = Vector2.zero;

    public Vector2[,] flowField;
    private List<Vector2> obstacles = new List<Vector2>();
    public Vector2Int playerGridPosition;
    public Transform player;
    private int[,] costField;
    private Queue<Vector2Int> openList = new Queue<Vector2Int>();
    public LayerMask obstacleLayer;
    private bool[,] visitedNodes;
    private float timer;
    public float interval;
    public Vector2 playerPosition;
    // public Vector2 sampleFlow;
    // public Vector2Int sampleInput;
    // public bool visited;

    void Start()
    {
        GenerateFlowField();
    }

    private void Update() 
    {
        //every n seconds regenerate flow field
        timer += Time.deltaTime;
        if(timer >= interval)   //check also if position has changed since last time
        {
            GenerateFlowField();
            timer = 0;
        }
    }

    private void GenerateFlowField()
    {
        // Initialize flow field, cost field, and visited nodes array
        flowField = new Vector2[gridSizeX, gridSizeY];
        costField = new int[gridSizeX, gridSizeY];
        visitedNodes = new bool[gridSizeX, gridSizeY];

        // Set player grid position
        playerPosition = player.position;
        // playerGridPosition = new Vector2Int(Mathf.FloorToInt(gridSizeX/2), Mathf.FloorToInt(gridSizeY/2));
        playerGridPosition = WorldToGrid(playerPosition);

        //WorldToGrid(playerPosition);

        // Initialize cost field and visited nodes array
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                costField[x, y] = int.MaxValue;
                visitedNodes[x, y] = false;
            }
        }

        // Set cost of player node to 0 and add to open list
        costField[playerGridPosition.x, playerGridPosition.y] = 0;
        openList.Enqueue(playerGridPosition);

        // Visit nodes in open list and update costs and flow field
        while (openList.Count > 0)
        {
            Vector2Int currentNode = openList.Dequeue();

            List<Vector2Int> directions = new List<Vector2Int>()
            {
                new Vector2Int(1, 0), // Right
                new Vector2Int(-1, 0), // Left
                new Vector2Int(0, 1), // Up
                new Vector2Int(0, -1), // Down
            };
            // Visit neighbors
            foreach(Vector2Int direction in directions)
            {
                int x = currentNode.x + direction.x;
                int y = currentNode.y + direction.y;

                if (x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY && !visitedNodes[x, y])
                {
                    Vector2Int neighbourNode = new Vector2Int(x, y);

                    Collider2D[] colliders = Physics2D.OverlapBoxAll(GridToWorld(neighbourNode), new Vector2(gridSize, gridSize), 0, obstacleLayer);
                    if (colliders.Length > 0)
                    {
                        // Neighbor overlaps with obstacle, skip it
                        visitedNodes[neighbourNode.x, neighbourNode.y] = true;
                        continue;
                    }
                    // Add neighbor to open list
                    openList.Enqueue(neighbourNode);

                    // Calculate cost to neighbor
                    int cost = costField[currentNode.x, currentNode.y] + 1;

                    // Update cost and flow field if cost is lower
                    if (cost < costField[x, y])
                    {
                        costField[x, y] = cost;
                        flowField[x, y] = GridToWorld(currentNode) - GridToWorld(neighbourNode);
                    }
                }
            }

            // Mark current node as visited
            visitedNodes[currentNode.x, currentNode.y] = true;
            // int xs = sampleInput.x;
            // int ys = sampleInput.y;
            // sampleFlow = flowField[xs,ys];

            // visited = visitedNodes[xs, ys];
        }
    }

    public Vector2Int WorldToGrid(Vector2 worldPosition)
    {
        int x = Mathf.RoundToInt((worldPosition.x - playerPosition.x) / (gridSize * 2) + gridSizeX/2);
        int y = Mathf.RoundToInt((worldPosition.y - playerPosition.y) / (gridSize * 2) + gridSizeY/2);
        return new Vector2Int(x, y);
    }

    public Vector2 GridToWorld(Vector2Int gridPosition)
    {
        int x = Mathf.FloorToInt(gridPosition.x - gridSizeX/2);
        int y = Mathf.FloorToInt(gridPosition.y - gridSizeY/2 + 0.5f);

        float worldX = playerPosition.x + x * gridSize;
        float worldY = playerPosition.y + y * gridSize;
        return new Vector2(worldX, worldY);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 pos = GridToWorld(new Vector2Int(x,y));
                Vector2 dir = flowField[x, y];

                Gizmos.DrawLine(pos, pos + dir.normalized * 0.3f);
            }
        }
    }
}

