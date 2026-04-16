using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System;

public class WallGrid : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject wallPrefab;
    public GameObject gatePrefab;
    public GameObject warningPrefab;

    [Header("Grid Settings")]
    public float squareSize;
    public float openingSize;
    public int columns;
    public int rows;

    [Header("Gate Settings")]
    public int maxGates = 3; // Maximum number of gates to create
    public float spawnInterval = 5f; // Time interval between gate spawns
    public float warningTime = 2f; // Time to display warning before wall spawn
    public float warningFrequency = 0.5f;
    
    // public Color warningColor; // Color to use for warning background
    private List<GameObject> gates = new List<GameObject>();
    public List<Vector2Int> potentialPlaces = new List<Vector2Int>();
    // private Queue<Vector2Int> gatesQueue;

    private float timer = 0f;

    void OnEnable() 
    {
        List<GameObject> gates = new List<GameObject>();
        List<Vector2Int> potentialPlaces = new List<Vector2Int>(); // List of all wall objects

        GridPattern();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            Vector2 centre = GetRandomGridPosition();
            StartCoroutine(WaitAndSpawnGate(centre));
            timer = 0f;
        }

        // Check if we need to remove the earliest wall object
        if (gates.Count > 4 * maxGates)
        {
            //remove first 4 objects
            int numGates = 4;
            Vector2 gatePositions = new Vector2(0,0);
            for(int i = 0; i < numGates; i++)
            {
                gatePositions += (Vector2) gates[0].transform.position;
                Destroy(gates[0]);
                gates.RemoveAt(0);
            }
            Vector2 averagePosition = gatePositions / numGates;
            Vector2Int location = PositionToGrid(averagePosition);
            potentialPlaces.Add(location);
        }
    }

    void CreateHorizontalWall(Vector2 position, GameObject prefab, float stretch)
    {
        // GameObject wallObject = Instantiate(prefab, position, Quaternion.identity, transform);
        // wallObject.transform.localScale = new Vector3(stretch, 1f, 1f) * 0.1f;

        float scale = 5f;
        GameObject wallObject = Instantiate(prefab, position, Quaternion.identity, transform);
        SpriteRenderer spriteRenderer = wallObject.GetComponent<SpriteRenderer>();
        spriteRenderer.size = new Vector2 (5f*stretch, 0.8f*scale) * 0.3f; 
        BoxCollider2D boxCollider = wallObject.GetComponent<BoxCollider2D>();
        boxCollider.size = spriteRenderer.bounds.size;
        wallObject.transform.localScale = new Vector3(1f, 1f, 1f) * 1/scale;
    }

    void CreateVerticalWall(Vector2 position, GameObject prefab, float stretch)
    {
        // GameObject wallObject = Instantiate(prefab, position, Quaternion.identity, transform);
        // wallObject.transform.localScale = new Vector3(1f, stretch, 1f) * 0.1f;

        float scale = 5f;
        GameObject wallObject = Instantiate(prefab, position, Quaternion.identity, transform);
        SpriteRenderer spriteRenderer = wallObject.GetComponent<SpriteRenderer>();
        spriteRenderer.size = new Vector2 (0.8f*scale, 5f*stretch) * 0.3f; 
        BoxCollider2D boxCollider = wallObject.GetComponent<BoxCollider2D>();
        boxCollider.size = spriteRenderer.bounds.size;
        wallObject.transform.localScale = new Vector3(1f, 1f, 1f) * 1/scale;
    }

    void CreateHorizontalGate(Vector2 position, GameObject prefab, float stretch)
    {
        float scale = 5f;
        GameObject gateObject = Instantiate(prefab, position, Quaternion.identity, transform);
        SpriteRenderer spriteRenderer = gateObject.GetComponent<SpriteRenderer>();
        spriteRenderer.size = new Vector2 (4.4f*stretch, 0.8f*scale) * 0.3f; 
        BoxCollider2D boxCollider = gateObject.GetComponent<BoxCollider2D>();
        boxCollider.size = spriteRenderer.bounds.size;
        gateObject.transform.localScale = new Vector3(1f, 1f, 1f) * 1/scale;

        gates.Add(gateObject);
    }

    void CreateVerticalGate(Vector2 position, GameObject prefab, float stretch)
    {
        float scale = 5f;
        GameObject gateObject = Instantiate(prefab, position, Quaternion.identity, transform);
        SpriteRenderer spriteRenderer = gateObject.GetComponent<SpriteRenderer>();
        spriteRenderer.size = new Vector2 (0.8f*scale, 4.4f*stretch) * 0.3f; 
        BoxCollider2D boxCollider = gateObject.GetComponent<BoxCollider2D>();
        boxCollider.size = spriteRenderer.bounds.size;
        gateObject.transform.localScale = new Vector3(1f, 1f, 1f) * 1/scale;

        gates.Add(gateObject);
    }

    void BuildSquareWalls(Vector2 centre)
    {
        float stretch = 4 * (squareSize - openingSize/2);
        float offDistance = squareSize/2 + openingSize/4;

        if(stretch == 0)
        {
            return;
        }

        CreateHorizontalWall(centre + new Vector2(offDistance, squareSize), wallPrefab, stretch);
        CreateVerticalWall(centre + new Vector2(squareSize, offDistance), wallPrefab, stretch);

        CreateHorizontalWall(centre + new Vector2(-offDistance, squareSize), wallPrefab, stretch);
        CreateVerticalWall(centre + new Vector2(-squareSize, offDistance), wallPrefab, stretch);

        CreateHorizontalWall(centre + new Vector2(offDistance, -squareSize), wallPrefab, stretch);
        CreateVerticalWall(centre + new Vector2(squareSize, -offDistance), wallPrefab, stretch);

        CreateHorizontalWall(centre + new Vector2(-offDistance, -squareSize), wallPrefab, stretch);
        CreateVerticalWall(centre + new Vector2(-squareSize, -offDistance), wallPrefab, stretch);
    }

    void GridPattern()
    {
        int colRange = (int) columns / 2;
        int rowRange = (int) rows / 2;
        for(int i = -colRange; i < colRange + 1; i++)
        {
            for(int j = -rowRange; j < rowRange + 1; j++)
            {
                potentialPlaces.Add(new Vector2Int(i,j));
                Vector2 centre = GridToPosition(new Vector2Int(i, j));
                BuildSquareWalls(centre);
            }
        }
    }

    Vector2 GridToPosition(Vector2Int gridLocation)
    {
        int x = gridLocation.x;
        int y = gridLocation.y;
        Vector2 position = new Vector2(2 * squareSize * x, 2 * squareSize * y);
        return position;
    }

    Vector2Int PositionToGrid(Vector2 position)
    {
        int x = (int) Mathf.Round(position.x / (2 * squareSize));
        int y = (int) Mathf.Round(position.y / (2 * squareSize));
        Vector2Int gridLocation = new Vector2Int(x, y);
        return gridLocation;
    }

    Vector2 GetRandomGridPosition()
    {
        int index = Random.Range(0, potentialPlaces.Count);
        Vector2Int location = potentialPlaces[index];
        potentialPlaces.Remove(location);
        Vector2 centre = GridToPosition(location);

        return centre;
    }

    void BuildGate(Vector2 centre)
    {
        float stretch = 4 * openingSize;
        CreateHorizontalGate(centre + new Vector2(0, squareSize), gatePrefab, stretch);
        CreateHorizontalGate(centre + new Vector2(0, -squareSize), gatePrefab, stretch);
        CreateVerticalGate(centre + new Vector2(squareSize, 0), gatePrefab, stretch);
        CreateVerticalGate(centre + new Vector2(-squareSize, 0), gatePrefab, stretch);
    }

    IEnumerator WaitAndSpawnGate(Vector2 centre)
    {
        StartCoroutine(FlashWarning(centre));
        yield return StartCoroutine(FlashWarning(centre));
        
        BuildGate(centre);
    }
    
    IEnumerator FlashWarning(Vector2 centre)
    {
        float warningTimer = 0f;
        float alpha = 0f;

        GameObject warningObject = Instantiate(warningPrefab, centre, Quaternion.identity, transform);
        warningObject.transform.localScale *= 2*squareSize;
        SpriteRenderer spriteRenderer = warningObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(1f, 0f, 0f, 0.4f*alpha);

        while(warningTimer < warningTime)
        {
            warningTimer += Time.deltaTime;
            alpha = Mathf.PingPong(Time.time / warningFrequency, 1f);
            spriteRenderer.color = new Color(1f, 0f, 0f, 0.4f*alpha);
            yield return null;
        }

        Destroy(warningObject);
    }

    void OnDisable() 
    {
        StopAllCoroutines();
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        gates.Clear();
        potentialPlaces.Clear();
    }
}