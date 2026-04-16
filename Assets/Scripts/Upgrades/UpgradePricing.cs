using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradePricing : MonoBehaviour
{
    // [SerializeField] private float baseSpeedLevel = 1;
    // [SerializeField] private float baseRangeLevel = 1;
    // [SerializeField] private float baseFireRateLevel = 1;
    // [SerializeField] private float baseDamageLevel = 1;

    public int initialPrice;
    private int price = 1;
    public float priceIncrease = 1.5f;
    private ScoreManager scoreManager;
    private TextMeshProUGUI priceText;
    private PlayerController player;

    void Start()
    {
        GameObject ScoreManager = GameObject.Find("ScoreManager");
        scoreManager = ScoreManager.GetComponent<ScoreManager>();
        priceText = this.GetComponent<TextMeshProUGUI>();  
        player = GameObject.Find("PlayerCircle").GetComponent<PlayerController>();
    }


    private void UpdatePrice()
    {
        scoreManager.UpdateScore(-price);
        price = (int) Mathf.Floor((float) price * priceIncrease) + 1;
        // priceText.text = "$" + price.ToString();
        priceText.text = price.ToString();
    }

    private void UpdateFireRate()
    {
        if(scoreManager.score >= price)
        {
            UpdatePrice();
            player.fireRate += 1;
        }
    }

    private void UpdateSpeed()
    {
        if(scoreManager.score >= price)
        {
            UpdatePrice();
            player.speed += 10;
        }
    }

    private void UpdateRange()
    {
        if(scoreManager.score >= price)
        {
            UpdatePrice();
            player.range += 1;
        }
    }

    private void UpdateDamage()
    {
        if(scoreManager.score >= price)
        {
            UpdatePrice();
            player.damage += 1;
        }
    }
}
