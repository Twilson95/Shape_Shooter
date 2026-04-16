using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyPool : MonoBehaviour
{
    public static CurrencyPool Instance;
    public GameObject currencyPrefab;
    public int poolSize = 10;
    private Queue<GameObject> currencyQueue;
    private Queue<GameObject> currencyOrderQueue;
    

    void Start()
    {
        Instance = this;
        StartCoroutine(FillPool());
    }

    IEnumerator FillPool()
    {
        currencyQueue = new Queue<GameObject>(poolSize);
        currencyOrderQueue = new Queue<GameObject>(poolSize);
        for (int i = 0; i < poolSize; i++)
        {
            GameObject currency = Instantiate(currencyPrefab, transform);
            currency.SetActive(false);
            currencyQueue.Enqueue(currency);
            currencyOrderQueue.Enqueue(currency);
            yield return null;
        }
    }

    public GameObject GetItem()
    {
        if (currencyQueue.Count == 0)
        {
            GameObject queueObject = GetOldestItem();
            ReturnItem(queueObject);
            // return null;
        }

        GameObject currency = currencyQueue.Dequeue();
        currency.SetActive(true);
        return currency;
    }

    private GameObject GetOldestItem()
    {
        if(currencyOrderQueue.Count == 0)
        {
            return null;
        }

        GameObject oldestQueueObject = currencyOrderQueue.Peek();
        return oldestQueueObject;
    }

    public void ReturnItem(GameObject currency)
    {
        currency.SetActive(false);
        currencyQueue.Enqueue(currency);
        currencyOrderQueue.Enqueue(currencyOrderQueue.Dequeue());
    }


    public void ReturnAllItems()
    {
        for(int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            GameObject currency = this.transform.GetChild(i).gameObject;
            if(currency.activeSelf)
            {
                currency.GetComponent<DroppedCurrency>().CollectCoin();
                ReturnItem(currency);
            }
        }
    }

}