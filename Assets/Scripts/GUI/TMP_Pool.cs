using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMP_Pool : MonoBehaviour
{
    public static TMP_Pool Instance;

    [SerializeField] private TMP_Text prefab;
    [SerializeField] private int poolSize = 10;

    private Queue<TMP_Text> objectPool;
    private Queue<TMP_Text> objectPoolOrder;

    private void Awake()
    {
        Instance = this;
        // SetObject.Method.MethodHandle.GetFunctionPointer();

        objectPool = new Queue<TMP_Text>();
        objectPoolOrder = new Queue<TMP_Text>();
        for (int i = 0; i < poolSize; i++)
        {
            TMP_Text obj = Instantiate(prefab, transform);
            // obj.gameObject.SetActive(false);
            obj.enabled = false;
            objectPool.Enqueue(obj);
            objectPoolOrder.Enqueue(obj);
        }
        SetObject(transform.position, 1);
    }

    public TMP_Text GetObject()
    {
        TMP_Text obj;
        if (objectPool.Count == 0)
        {
            obj = GetOldestObject();
            ReturnObject(obj);
        }
        obj = objectPool.Dequeue();
        // obj.gameObject.SetActive(true);
        obj.enabled = true;

        return obj;
    }

    public void SetObject(Vector3 position, int score)
    {
        TMP_Text textObject = GetObject();
        textObject.transform.position = position;
        
        textObject.gameObject.GetComponent<ScoreText>().ShowTextCoroutine(score);
        // ScoreText scoreText = textObject.gameObject.GetComponent<ScoreText>();
        // scoreText.ShowTextCoroutine(score);
    }

    private TMP_Text GetOldestObject()
    {
        if(objectPoolOrder.Count == 0)
        {
            return null;
        }

        TMP_Text oldestObject = objectPoolOrder.Peek();
        return oldestObject;
    }

    public void ReturnObject(TMP_Text obj)
    {
        // obj.gameObject.SetActive(false);
        obj.enabled = false;
        objectPool.Enqueue(obj);
        objectPoolOrder.Enqueue(objectPoolOrder.Dequeue());
    }
}
