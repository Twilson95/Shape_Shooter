using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreText : MonoBehaviour
{
    public float fadeTime = 1.0f;
    
    public TMP_Text textMesh;
    public float startTime;
    public ScorePool scorePool;
    public float alpha = 1.0f;
    private Color startColor;
    //private float elapsedTime = 0f;
    
    void Start()
    {
        startColor = textMesh.color;
    }
    
    public void SetScore(int score)
    {
        // Set the score value on the TextMeshProUGUI component
        textMesh.text = "+" + score.ToString();
    }

    public void ShowTextCoroutine(int score)
    {
        SetScore(score);
        StartCoroutine(ShowText());

        // Debug.Log("Run Coroutine");
    }

    IEnumerator ShowText()
    {
        float fadeTime = 1.0f;
        float elapsedTime = 0.0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1.0f, 0.0f, elapsedTime / fadeTime);
            Color color = textMesh.color;
            color.a = alpha;
            textMesh.color = color;
            yield return null;
        }

        TMP_Pool.Instance.ReturnObject(textMesh);
        // scorePool.ReturnScoreObject(gameObject);
    }
}