using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class ScorePool : MonoBehaviour
{
    public GameObject scorePrefab;
    public int poolSize = 10;
    public float despawnTime = 5f;

    // private Queue<GameObject> scoreQueue;
    // private Queue<GameObject> scoreOrderQueue;
    private Queue<GameObject> scoreQueue;
    private Queue<GameObject> scoreOrderQueue;

    void Awake()
    {
        scoreQueue = new Queue<GameObject>(poolSize);
        scoreOrderQueue = new Queue<GameObject>(poolSize);

        for (int i = 0; i < poolSize; i++)
        {
            GameObject scoreObject = Instantiate(scorePrefab, transform);
            // GameObject scoreObject = Instantiate(scorePrefab, transform).GetComponent<GameObject>();
            scoreObject.SetActive(false);
            scoreQueue.Enqueue(scoreObject);
            scoreOrderQueue.Enqueue(scoreObject);

            //GetScoreObject(new Vector3 (-20,0,0), 0);
        }
    }

    public void GetScoreObject(Vector3 position, int score)
    {
        // GameObject scoreObject;
        GameObject scoreObject;
        if (scoreQueue.Count == 0)
        {
            scoreObject = GetOldestScoreObject();
            ReturnScoreObject(scoreObject);
        }

        scoreObject = scoreQueue.Dequeue();
        scoreObject.transform.position = position;
        scoreObject.SetActive(true);
        //Debug.Log("score text activated");
        ScoreText scoreText = scoreObject.GetComponent<ScoreText>();
        scoreText.SetScore(score);
    }

    private GameObject GetOldestScoreObject()
    {
        if(scoreOrderQueue.Count == 0)
        {
            return null;
        }

        GameObject oldestscoreObject = scoreOrderQueue.Peek();
        return oldestscoreObject;
    }

    public void ReturnScoreObject(GameObject scoreObject)
    {
        // Debug.Log("return score object");
        scoreObject.SetActive(false);
        scoreQueue.Enqueue(scoreObject);
        scoreOrderQueue.Enqueue(scoreOrderQueue.Dequeue());
    }
}