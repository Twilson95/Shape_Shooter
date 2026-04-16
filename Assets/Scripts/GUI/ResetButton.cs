using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResetButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public GameController gameController;
    public int pressCounter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnable() 
    {
        buttonText.text = "Reset Game";
        pressCounter = 0;
    }

    public void ResetButtonPressed()
    {
        pressCounter += 1;
        if(pressCounter == 1)
        {
            buttonText.text = "Are you sure, all progress will be lost?";
        }
        else if(pressCounter == 2)
        {
            buttonText.text = "Are you positive you want to start from the beginning?";
        }
        else if(pressCounter == 3)
        {
            pressCounter = 0;
            buttonText.text = "Reset Game Data";
            gameController.ResetGameData();
        }
    }

}
