using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public int score = 0;

    public void UpdateScore(int value)
    {
        score += value;
        // scoreText.text = "Score: " + score.ToString();
        scoreText.text = score.ToString();
    }

    public void ResetScore()
    {
        score = 0;
        // scoreText.text = "Score: " + score.ToString(); 
        scoreText.text = score.ToString();
    }
    
    public void SetSaveData(GameData gameData)
    {
        UpdateScore(gameData.score);
    }
}
