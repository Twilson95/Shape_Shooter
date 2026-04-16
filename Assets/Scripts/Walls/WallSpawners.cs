using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallSpawners : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject wallPrefab;

    [Header("Grid Settings")]
    public float squareSize;
    public float openingSize;
    public int spacing;

    void OnEnable() 
    {
        //WallVariant1();

    }

    void Update()
    {
 
    }

    public void WallVariant1()
    {
        BuildSquareWalls(new Vector2(0, 0));

        float stretch = 20 * (squareSize - openingSize/2);

        CreateHorizontalWall(new Vector2(0, 3 * squareSize), wallPrefab, stretch);
        CreateVerticalWall(new Vector2(3 * squareSize, 0), wallPrefab, stretch);

        CreateHorizontalWall(new Vector2(0, -3 * squareSize), wallPrefab, stretch);
        CreateVerticalWall(new Vector2(-3 * squareSize, 0), wallPrefab, stretch);
    }

    public void WallVariant2()
    {
        BuildSquareWalls(new Vector2(0, 0));
 
        BuildSquareWalls(new Vector2(1.5f * spacing, 1.5f * spacing));
        BuildSquareWalls(new Vector2(1.5f * spacing, -1.5f * spacing));
        BuildSquareWalls(new Vector2(-1.5f * spacing, 1.5f * spacing));
        BuildSquareWalls(new Vector2(-1.5f * spacing, -1.5f * spacing));
    }

    public void WallVariant3()
    {
        BuildSquareWalls(new Vector2(0, 0));

        BuildSquareWalls(new Vector2(spacing, 0));
        BuildSquareWalls(new Vector2(0, spacing));
        BuildSquareWalls(new Vector2(-spacing, 0));
        BuildSquareWalls(new Vector2(0, -spacing));
        
        BuildSquareWalls(new Vector2(1.5f * spacing, 1.5f * spacing));
        BuildSquareWalls(new Vector2(1.5f * spacing, -1.5f * spacing));
        BuildSquareWalls(new Vector2(-1.5f * spacing, 1.5f * spacing));
        BuildSquareWalls(new Vector2(-1.5f * spacing, -1.5f * spacing));
        
        BuildSquareWalls(new Vector2(2f * spacing, 2f * spacing));
        BuildSquareWalls(new Vector2(2f * spacing, -2f * spacing));
        BuildSquareWalls(new Vector2(-2f * spacing, 2f * spacing));
        BuildSquareWalls(new Vector2(-2f * spacing, -2f * spacing));

        BuildSquareWalls(new Vector2(2.5f * spacing, 0));
        BuildSquareWalls(new Vector2(0, 2.5f * spacing));
        BuildSquareWalls(new Vector2(-2.5f * spacing, 0));
        BuildSquareWalls(new Vector2(0, -2.5f * spacing));

        BuildSquareWalls(new Vector2(3.5f * spacing, spacing));
        BuildSquareWalls(new Vector2(3.5f * spacing, -spacing));
        BuildSquareWalls(new Vector2(-3.5f * spacing, spacing));
        BuildSquareWalls(new Vector2(-3.5f * spacing, -spacing));

    }

    void CreateHorizontalWall(Vector2 position, GameObject prefab, float stretch)
    {
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
        float scale = 5f;
        GameObject wallObject = Instantiate(prefab, position, Quaternion.identity, transform);
        SpriteRenderer spriteRenderer = wallObject.GetComponent<SpriteRenderer>();
        spriteRenderer.size = new Vector2 (0.8f*scale, 5f*stretch) * 0.3f; 
        BoxCollider2D boxCollider = wallObject.GetComponent<BoxCollider2D>();
        boxCollider.size = spriteRenderer.bounds.size;
        wallObject.transform.localScale = new Vector3(1f, 1f, 1f) * 1/scale;
    }

    void BuildSquareWalls(Vector2 centre)
    {
        float stretch = 4 * (squareSize - openingSize/2);
        float offDistance = squareSize/2 + openingSize/4;

        CreateHorizontalWall(centre + new Vector2(offDistance, squareSize), wallPrefab, stretch);
        CreateVerticalWall(centre + new Vector2(squareSize, offDistance), wallPrefab, stretch);

        CreateHorizontalWall(centre + new Vector2(-offDistance, squareSize), wallPrefab, stretch);
        CreateVerticalWall(centre + new Vector2(-squareSize, offDistance), wallPrefab, stretch);

        CreateHorizontalWall(centre + new Vector2(offDistance, -squareSize), wallPrefab, stretch);
        CreateVerticalWall(centre + new Vector2(squareSize, -offDistance), wallPrefab, stretch);

        CreateHorizontalWall(centre + new Vector2(-offDistance, -squareSize), wallPrefab, stretch);
        CreateVerticalWall(centre + new Vector2(-squareSize, -offDistance), wallPrefab, stretch);
    }

    void OnDisable()
    {
        StopAllCoroutines();
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}