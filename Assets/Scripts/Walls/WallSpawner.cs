using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpawner : MonoBehaviour
{
    public GameObject wallPrefab;
    public int width = 10;
    public int height = 10;
    public int openingWidth = 3;
    public int openingHeight = 3;
    public LevelManager levelManager;

    void Awake()
    {
        BuildWalls();
    }

    public void BuildWalls()
    {
        Layer1Walls();
        Layer2Walls();
        Layer3Walls();
        Layer4Walls();
        Layer5Walls();
        Layer6Walls();
    }

    public void DestroyWalls()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void SquareCorners(int layer)
    {
        float scaling = wallPrefab.transform.localScale.x * 2;
        // float scaling = 1;
        // Create 2D array to represent level grid
        GameObject[,] walls = new GameObject[width * layer, height * layer];

        // Instantiate wall prefabs at the appropriate positions
        for (int x = 0; x < width * layer; x++)
        {
            for (int y = 0; y < height * layer; y++)
            {
                if((x > (width/2 - openingWidth) * layer && x < (width/2 + openingWidth) * layer) 
                || (y > (height/2 - openingHeight) * layer && y < (height/2 + openingHeight) * layer))
                {
                    continue;
                }

                if(
                    ((x == 0 || x == width * layer - 1) && y < height * layer) || ((y == 0 || y == height * layer -1) && x < width * layer)
                )
                {
                    float x1 = (float) (x - (width/2) * layer) * scaling;
                    float y1 = (float) (y - (height/2) * layer) * scaling;
                    walls[x, y] = Instantiate(wallPrefab, new Vector3(x1, y1, 0), Quaternion.identity);
                    walls[x, y].transform.SetParent(transform);
                }
            }
        }
    }

    private void SquareSides(int layer)
    {
        float scaling = wallPrefab.transform.localScale.x * 2;
        
        GameObject[,] walls = new GameObject[width * layer, height * layer];

        // Instantiate wall prefabs at the appropriate positions
        for (int x = 0; x < width * layer; x++)
        {
            for (int y = 0; y < height * layer; y++)
            {
                if(((x > (width/2 - openingWidth) * layer && x < (width/2 + openingWidth) * layer && (y == 0 || y == height * layer - 1)))
                || (y > (height/2 - openingHeight) * layer && y < (height/2 + openingHeight) * layer && (x == 0 || x == width * layer - 1)))
                {
                    float x1 = (float) (x-(width/2) * layer) * scaling;
                    float y1 = (float) (y-(height/2) * layer) * scaling;
                    walls[x, y] = Instantiate(wallPrefab, new Vector3(x1, y1, 0), Quaternion.identity);
                    walls[x, y].transform.SetParent(transform);
                }

            }
        }
    }

    private void Layer1Walls()
    {
        int layer = 1;
        int offsetLevel = levelManager.level - 0;
        if(offsetLevel < 0)
        {
            return;
        }
        if(offsetLevel % 3 == 0)
        {
            return;
        }
        if(offsetLevel % 3 == 1)
        {
            SquareCorners(layer);
        }
        if(offsetLevel % 3 == 2)
        {
            SquareSides(layer);
        }
    }

    private void Layer2Walls()
    {
        int layer = 2;
        int offsetLevel = levelManager.level - 1;
        if(offsetLevel < 0)
        {
            return;
        }
        if(offsetLevel % 3 == 0)
        {
            return;
        }
        if(offsetLevel % 3 == 1)
        {
            SquareCorners(layer);
        }
        if(offsetLevel % 3 == 2)
        {
            SquareSides(layer);
        }
    }

    private void Layer3Walls()
    {
        int layer = 3;
        int offsetLevel = levelManager.level - 2;
        if(offsetLevel < 0)
        {
            return;
        }
        if(offsetLevel % 3 == 0)
        {
            return;
        }
        if(offsetLevel % 3 == 1)
        {
            SquareCorners(layer);
        }
        if(offsetLevel % 3 == 2)
        {
            SquareSides(layer);
        }
    }

    private void Layer4Walls()
    {
        int layer = 4;
        int offsetLevel = levelManager.level - 3;
        if(offsetLevel < 0)
        {
            return;
        }
        if(offsetLevel % 3 == 0)
        {
            return;
        }
        if(offsetLevel % 3 == 1)
        {
            SquareCorners(layer);
        }
        if(offsetLevel % 3 == 2)
        {
            SquareSides(layer);
        }
    }
    
    private void Layer5Walls()
    {
        int layer = 5;
        int offsetLevel = levelManager.level - 4;
        if(offsetLevel < 0)
        {
            return;
        }
        if(offsetLevel % 3 == 0)
        {
            return;
        }
        if(offsetLevel % 3 == 1)
        {
            SquareCorners(layer);
        }
        if(offsetLevel % 3 == 2)
        {
            SquareSides(layer);
        }
    }
    
    private void Layer6Walls()
    {
        int layer = 6;
        int offsetLevel = levelManager.level - 5;
        if(offsetLevel < 0)
        {
            return;
        }
        if(offsetLevel % 3 == 0)
        {
            return;
        }
        if(offsetLevel % 3 == 1)
        {
            SquareCorners(layer);
        }
        if(offsetLevel % 3 == 2)
        {
            SquareSides(layer);
        }
    }
}