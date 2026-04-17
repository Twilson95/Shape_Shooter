using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HandlePremiumUpgrade : MonoBehaviour
{
    private PremiumManager premiumManager;
    private TextMeshProUGUI priceText;
    private TextMeshProUGUI upgradeLevelText;
    private PlayerController player;
    private LevelManager levelManager;
    public int initialPrice;
    public int price = 5;
    public float priceIncrease = 1.5f;
    public int additionalIncrease = 1;
    public int initialLevel = 1;
    public int maxLevel = 20;
    public int upgradeLevel = 1;
    public GameObject turretPrefab;
    public CoinMagnet coinMagnet;
    public Button nextUpgradeButton;
    public Button thisButton;

    void Start()
    {
        player = GameObject.Find("PlayerCircle").GetComponent<PlayerController>();

        premiumManager = GameObject.Find("PremiumManager").GetComponent<PremiumManager>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();

        upgradeLevelText = transform.Find("Level").GetComponent<TextMeshProUGUI>();
        priceText = transform.Find("Price").GetComponent<TextMeshProUGUI>();
        
        TextMeshProUGUI upgradeName = transform.Find("Name").GetComponent<TextMeshProUGUI>();
        if(upgradeName.text == "Base Level")
        {
            upgradeLevel = levelManager.baseLevel;
            //Debug.Log(upgradeLevel.ToString());
        }
        else if(upgradeName.text == "Magnet")
        {
            upgradeLevel = coinMagnet.magnetLevel;
        }
        else
        {
            upgradeLevel = player.baseLevel;
        }
        
        ResetUpgrades();
        CheckMaxLevelReached();
    }

    public void CheckMaxLevelReached()
    {
        if(upgradeLevel >= maxLevel)
        {
            priceText.text = "";
            thisButton.interactable = false;
        }
        else
        {
            thisButton.interactable = true;
        }
    }

    private void UpdateUpgradeLevel()
    {
        upgradeLevel += 1;
        upgradeLevelText.text = "(" + upgradeLevel.ToString() + " / " + maxLevel.ToString() + ")";
        CheckMaxLevelReached();
    }

    private void UpdatePrice()
    {
        premiumManager.UpdatePremium(-price);
        price = (int) Mathf.Floor((float) price * priceIncrease) + additionalIncrease;;
        // priceText.text = "$" + price.ToString();
        priceText.text = price.ToString();
        CheckMaxLevelReached();
    }

    public void UpdateBaseStats()
    {
        if(premiumManager.premium >= price)
        {
            UpdateUpgradeLevel();
            UpdatePrice();
            player.RaiseBaseLevel();
            player.ResetStatLevels();

            HandleUpgrade[] handleUpgradesList = FindObjectsByType<HandleUpgrade>(FindObjectsSortMode.None);
            foreach(HandleUpgrade handleUpgrade in handleUpgradesList)
            {
                handleUpgrade.baseLevel += 1;
                handleUpgrade.ResetUpgrades();
            }
        }
    }

    public void UpdateBaseLevel()
    {
        if(premiumManager.premium < price)
        {
            //warning on screen with reason - insufficient funds
            return;
        }
        if(levelManager.highestLevel <= levelManager.baseLevel)
        {
            //warning on screen with reason - 
            return;
        }

        UpdateUpgradeLevel();
        UpdatePrice();
        levelManager.RaiseBaseLevel();   
    }

    public void UpdateMagnetLevel()
    {
        if(premiumManager.premium < price)
        {
            //warning on screen with reason - insufficient funds
            return;
        }
        UpdateUpgradeLevel();
        UpdatePrice();
        coinMagnet.RaiseMagnetLevel();
    }


    public void ResetUpgrades()
    {
        upgradeLevelText.text = "(" + upgradeLevel.ToString() + " / " + maxLevel.ToString() + ")";

        price = initialPrice;
        for (int i = initialLevel; i < upgradeLevel; i++)
            price = (int) Mathf.Floor((float) price * priceIncrease) + additionalIncrease;

        priceText.text = price.ToString();
        CheckMaxLevelReached();
    }
}
