using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinMagnet : MonoBehaviour
{
    public float initialAttractDistance = 1f;
    public float initialAttractSpeed = 1f;
    public float attractDistance = 1f;
    public float attractSpeed = 1f;
    public float distanceIncrease = 1f;
    public float speedIncrease = 1f;
    public int magnetLevel = 0;
    public GameObject aura1;
    public GameObject aura2;
    public GameObject aura3;
    private GameObject coinPool;
    private List<GameObject> coins;

    void Start()
    {
        coinPool = GameObject.Find("CurrencyPool");
        coins = new List<GameObject>();
        foreach (Transform child in coinPool.transform)
        {
            coins.Add(child.gameObject);
        }
    }

    void Update()
    {
        foreach (Transform child in coinPool.transform)
        {
            if(magnetLevel == 0)
            {
                continue;
            }
            if (!child.gameObject.activeSelf)
            {
                continue;
            }
            float distance = Vector3.Distance(transform.position, child.position);
            if (distance < attractDistance)
            {
                child.position = Vector3.MoveTowards(child.position, transform.position, attractSpeed * Time.deltaTime);
            }
        }
    }

    public void RaiseMagnetLevel()
    {
        magnetLevel += 1;
        UpdateStats();
    }

    public void ResetMagnetLevel()
    {
        magnetLevel = 0;
        UpdateStats();
    }

    public void UpdateStats()
    {
        attractDistance = initialAttractDistance + distanceIncrease * (magnetLevel-1);
        attractSpeed = initialAttractSpeed + speedIncrease * (magnetLevel-1);

        if(magnetLevel == 1)
        {
            aura1.SetActive(true);
            aura2.SetActive(false);
            aura3.SetActive(false);
        }
        else if(magnetLevel == 2)
        {
            aura1.SetActive(true);
            aura2.SetActive(true);
            aura3.SetActive(false);
        }
        else if(magnetLevel == 3)
        {
            aura1.SetActive(true);
            aura2.SetActive(true);
            aura3.SetActive(true);
        }
        else
        {
            aura1.SetActive(false);
            aura2.SetActive(false);
            aura3.SetActive(false);
        }
        
    }

    public void SetSaveData(GameData gameData)
    {
        try
        {  
            magnetLevel = gameData.magnetLevel;
        }
        catch
        {
            magnetLevel = 0;
        }
        UpdateStats();
    }

    //  void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Coin"))
    //     {
    //         StartCoroutine(AttractCoin(other.gameObject));
    //     }
    // }

    // IEnumerator AttractCoin(GameObject coin)
    // {
    //     while (Vector3.Distance(transform.position, coin.transform.position) > 0.1f)
    //     {
    //         coin.transform.position = Vector3.MoveTowards(coin.transform.position, transform.position, attractSpeed * Time.deltaTime);
    //         yield return null;
    //     }
    //     // coin has reached the player, do something with it (e.g., increase score, play sound, etc.)
    // }

    // void Update()
    // {
    //     foreach (GameObject coin in coins)
    //     {
    //         if (coin.activeInHierarchy)
    //         {
    //             float distance = Vector3.Distance(transform.position, coin.transform.position);
    //             if (distance < attractDistance)
    //             {
    //                 coin.transform.position = Vector3.MoveTowards(coin.transform.position, transform.position, attractSpeed * Time.deltaTime);
    //             }
    //         }
    //     }
    // }
}