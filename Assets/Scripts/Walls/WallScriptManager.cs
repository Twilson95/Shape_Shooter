using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScriptManager : MonoBehaviour
{
    public WallGrid wallGrid;
    public WallSpawners wallSpawners;
    public WallSequence wallSequence;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void EnableWallScript(int level)
    {
        int remainder = level % 5;
        if(remainder == 1)
        {
            //plain level
            wallSpawners.enabled = true;
            wallSpawners.WallVariant1();
        }
        if(remainder == 2)
        {
            //grid level
            wallGrid.enabled = true;
        }
        if(remainder == 3)
        {
            //laser level
            wallSpawners.enabled = true;
            wallSpawners.WallVariant3();
        }
        if(remainder == 4)
        {
            //rush level
            wallSequence.enabled = true;
        }
        if(remainder == 0)
        {
            //boss level
            wallSpawners.enabled = true;
            wallSpawners.WallVariant2();
        }
    }
    
    public void DisableScripts()
    {
        wallGrid.enabled = false;
        wallSpawners.enabled = false;
        wallSequence.enabled = false;
    }

}
