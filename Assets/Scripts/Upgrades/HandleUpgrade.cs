using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandleUpgrade : MonoBehaviour
{
    public int baseLevel = 1;

    private ScoreManager scoreManager;
    private PremiumManager premiumManager;
    private TextMeshProUGUI priceText;
    private PlayerController player;
    public int upgradeLevel = 1;
    private TextMeshProUGUI upgradeLevelText;
    public int initialPrice = 1;
    private int price = 1;
    public float priceIncrease = 1.5f;
    public int additionalIncrease = 1;
    public int maxLevel = 20;
    public bool isPremium = false;
    public Button button;


    void Start()
    {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        premiumManager = GameObject.Find("PremiumManager").GetComponent<PremiumManager>();
        player = GameObject.Find("PlayerCircle").GetComponent<PlayerController>();

        upgradeLevelText = transform.Find("Level").GetComponent<TextMeshProUGUI>();
        priceText = transform.Find("Price").GetComponent<TextMeshProUGUI>();

        baseLevel = player.baseLevel;
        ResetUpgrades();
        CheckMaxLevelReached();
    }

    void CheckMaxLevelReached()
    {
        if(upgradeLevel >= maxLevel)
        {
            priceText.text = "";
            button.interactable = false;
        }
    }

    public void UpdateUpgradeLevel()
    {
        upgradeLevel += 1;
        upgradeLevelText.text = "(" + upgradeLevel.ToString() + " / " + maxLevel.ToString() + ")";
        CheckMaxLevelReached();
    }

    public void UpdatePrice()
    {
        if(isPremium == true)
        {
            premiumManager.UpdatePremium(-price);
        }
        else
        {
            scoreManager.UpdateScore(-price);
        }
        
        price = (int) Mathf.Floor((float) price * priceIncrease) + additionalIncrease;
        // priceText.text = "$" + price.ToString();
        priceText.text = price.ToString();
    }

    public void UpdateFireRate()
    {
        if(scoreManager.score >= price)
        {
            UpdateUpgradeLevel();
            UpdatePrice();
            player.RaiseFireRate();
        }
    }

    public void UpdateSpeed()
    {
        if(scoreManager.score >= price)
        {
            UpdateUpgradeLevel();
            UpdatePrice();
            player.RaiseSpeed();
        }
    }

    public void UpdateRange()
    {
        if(scoreManager.score >= price)
        {
            UpdateUpgradeLevel();
            UpdatePrice();
            player.RaiseSpeed();
        }
    }

    public void UpdateDamage()
    {
        if(scoreManager.score >= price)
        {
            UpdateUpgradeLevel();
            UpdatePrice();
            player.RaiseDamage();
        }
    }

    public void ResetUpgrades()
    {
        //Debug.Log("handleUpgrade reset");
        upgradeLevel = baseLevel;
        upgradeLevelText.text = "(" + upgradeLevel.ToString() + " / " + maxLevel.ToString() + ")";

        price = initialPrice;
        for (int i = 1; i < baseLevel; i++)
            price = (int) Mathf.Floor((float) price * priceIncrease) + additionalIncrease;

        // priceText.text = "$" + price.ToString();
        priceText.text = price.ToString();
        CheckMaxLevelReached();
    }
}
