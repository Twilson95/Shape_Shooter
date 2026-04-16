using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System;

public class WallSequence : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject wallPrefab;
    public GameObject gatePrefab;

    // [Header("Grid Settings")]
    // public float squareSize;
    // public float openingSize;
    // public int columns;
    // public int rows;

    [Header("Gate Settings")]
    public int maxGates = 3; // Maximum number of gates to create
    public float spawnInterval = 5f; // Time interval between gate spawns
  
    private List<GameObject> gates = new List<GameObject>();


    private float timer = 0f;

    void OnEnable() 
    {
        List<GameObject> gates = new List<GameObject>();
        timer = 25f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            int index = Random.Range(0, 6);
            Vector2 centre;
            if(index == 0)
            {
                centre = new Vector2(0,0);
                StartCoroutine(ShrinkingSquares(centre));
            }
            else if(index == 1)
            {
                centre = new Vector2(0,0);
                StartCoroutine(GrowingSquares(centre));
            }
            else if(index == 2)
            {
                StartCoroutine(SweepRight());
            }
            else if(index == 3)
            {
                StartCoroutine(SweepLeft());
            }
            else if(index == 4)
            {
                StartCoroutine(SweepUp());
            }
            else if(index == 5)
            {
                StartCoroutine(SweepDown());
            }

            timer = 0f;
        }

        CheckForGateRemoval();
    }

    void CheckForGateRemoval()
    {
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
        }
    }

   
    void CreateHorizontalGate(Vector2 position, GameObject prefab, float stretch)
    {
        float scale = 5f;
        GameObject gateObject = Instantiate(prefab, position, Quaternion.identity, transform);
        SpriteRenderer spriteRenderer = gateObject.GetComponent<SpriteRenderer>();
        spriteRenderer.size = new Vector2 (scale * stretch, 0.3f * 0.8f * scale); 
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
        spriteRenderer.size = new Vector2 (0.3f * 0.8f * scale, scale * stretch); 
        BoxCollider2D boxCollider = gateObject.GetComponent<BoxCollider2D>();
        boxCollider.size = spriteRenderer.bounds.size;
        gateObject.transform.localScale = new Vector3(1f, 1f, 1f) * 1/scale;
        
        gates.Add(gateObject);
    }

    void BuildGate(Vector2 centre, float squareSize)
    {
        // float stretch = 4 * openingSize;
        float stretch = 2f * squareSize;
        CreateHorizontalGate(centre + new Vector2(0, squareSize), gatePrefab, stretch);
        CreateHorizontalGate(centre + new Vector2(0, -squareSize), gatePrefab, stretch);
        CreateVerticalGate(centre + new Vector2(squareSize, 0), gatePrefab, stretch);
        CreateVerticalGate(centre + new Vector2(-squareSize, 0), gatePrefab, stretch);
    }

    void BuildGateRectangle(Vector2 centre, float squareSize)
    {
        float stretch = 2f * squareSize;
        CreateHorizontalGate(centre + new Vector2(0, 0.5f * squareSize), gatePrefab, stretch);
        CreateHorizontalGate(centre + new Vector2(0, -0.5f * squareSize), gatePrefab, stretch);
        CreateVerticalGate(centre + new Vector2(squareSize, 0), gatePrefab, 0.5f * stretch);
        CreateVerticalGate(centre + new Vector2(-squareSize, 0), gatePrefab, 0.5f * stretch);
    }

    IEnumerator ShrinkingSquares(Vector2 centre)
    {
        BuildGateRectangle(centre, 20);
        yield return new WaitForSeconds(5);
        BuildGateRectangle(centre, 16);
        yield return new WaitForSeconds(5);
        BuildGateRectangle(centre, 12);
        yield return new WaitForSeconds(5);
        BuildGateRectangle(centre, 8);
        yield return new WaitForSeconds(5);
        BuildGateRectangle(centre, 4);
    }

    IEnumerator GrowingSquares(Vector2 centre)
    {
        BuildGateRectangle(centre, 4);
        yield return new WaitForSeconds(5);
        BuildGateRectangle(centre, 8);
        yield return new WaitForSeconds(5);
        BuildGateRectangle(centre, 12);
        yield return new WaitForSeconds(5);
        BuildGateRectangle(centre, 16);
        yield return new WaitForSeconds(5);
        BuildGateRectangle(centre, 20);
    }

    IEnumerator SweepRight()
    {
        List<Vector2Int> vectorList = new List<Vector2Int>();
        vectorList.Add(new Vector2Int(-16,0));
        vectorList.Add(new Vector2Int(-8,0));
        vectorList.Add(new Vector2Int(0,0));
        vectorList.Add(new Vector2Int(8,0));
        vectorList.Add(new Vector2Int(16,0));

        foreach(Vector2Int position in vectorList)
        {
            for(int i = 0; i < 4; i++)
            {
                CreateVerticalGate(position, gatePrefab, 100);
            }
            yield return new WaitForSeconds(5);
        }
    }

    IEnumerator SweepLeft()
    {
        List<Vector2Int> vectorList = new List<Vector2Int>();
        vectorList.Add(new Vector2Int(-16,0));
        vectorList.Add(new Vector2Int(-8,0));
        vectorList.Add(new Vector2Int(0,0));
        vectorList.Add(new Vector2Int(8,0));
        vectorList.Add(new Vector2Int(-16,0));

        foreach(Vector2Int position in vectorList)
        {
            for(int i = 0; i < 4; i++)
            {
                CreateVerticalGate(position, gatePrefab, 100);
            }
            yield return new WaitForSeconds(5);
        }
    }

    IEnumerator SweepUp()
    {
        List<Vector2Int> vectorList = new List<Vector2Int>();
        vectorList.Add(new Vector2Int(0,-10));
        vectorList.Add(new Vector2Int(0,-5));
        vectorList.Add(new Vector2Int(0,0));
        vectorList.Add(new Vector2Int(0,5));
        vectorList.Add(new Vector2Int(0,10));

        foreach(Vector2Int position in vectorList)
        {
            for(int i = 0; i < 4; i++)
            {
                CreateHorizontalGate(position, gatePrefab, 300);
            }
            yield return new WaitForSeconds(5);
        }
    }

    IEnumerator SweepDown()
    {
        List<Vector2Int> vectorList = new List<Vector2Int>();
        vectorList.Add(new Vector2Int(0,10));
        vectorList.Add(new Vector2Int(0,5));
        vectorList.Add(new Vector2Int(0,0));
        vectorList.Add(new Vector2Int(0,-5));
        vectorList.Add(new Vector2Int(0,-10));

        foreach(Vector2Int position in vectorList)
        {
            for(int i = 0; i < 4; i++)
            {
                CreateHorizontalGate(position, gatePrefab, 300);
            }
            yield return new WaitForSeconds(5);
        }
    }

    void OnDisable() 
    {
        StopAllCoroutines();
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        gates.Clear();
    }
}