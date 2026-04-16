using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponsUpgrades : MonoBehaviour
{
    private PremiumManager premiumManager;
    private TextMeshProUGUI priceText;
    private PlayerController player;
    private LevelManager levelManager;
    public int price = 5;
    public GameObject turretPrefab;
    public Button nextUpgradeButton;
    public int turretLevel2Price;
    public int turretLevel3Price;
    public int turretLevel4Price;
    public string turretLevel2Name;
    public string turretLevel3Name;
    public string turretLevel4Name;
    public Sprite turretLevel2Image;
    public Sprite turretLevel3Image;
    public Sprite turretLevel4Image;
    public Image weaponImage;
    public TextMeshProUGUI upgradeName;

    // private Achievements achievements;
    // public string achievementID;

    void Start()
    {
        premiumManager = GameObject.Find("PremiumManager").GetComponent<PremiumManager>();
        player = GameObject.Find("PlayerCircle").GetComponent<PlayerController>();
        priceText = transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        // achievements = GameObject.Find("Achievements").GetComponent<Achievements >();
        // upgradeName.text = turretLevel2Name;
        // weaponImage.sprite = turretLevel2Image;
        // price = turretLevel2Price;
        priceText.text = price.ToString();

    }

    private void DestroyTurrents()
    {
        float turretCount = player.transform.childCount;
        for (int i = 0; i < turretCount; i++)
        {
            Transform turret = player.transform.GetChild(i);
            Destroy(turret.gameObject);
        }
    }

    private void UpdatePrice()
    {
        // premiumManager.UpdatePremium(-price);
        // priceText.text = "$" + price.ToString();
    }

    public void UpgradeTurret()
    {
        if(player.weaponLevel == 1)
        {
            DoubleTurretUpgrade();
            price = turretLevel3Price;
            weaponImage.sprite = turretLevel3Image;
            
        }
        else if(player.weaponLevel == 2)
        {
            TripleTurretUpgrade();
            price = turretLevel4Price;
            weaponImage.sprite = turretLevel4Image;
        }
        else if(player.weaponLevel == 3)
        {
            LaserUpgrade();
        }
    }

    public void DoubleTurretUpgrade()
    {
        if(premiumManager.premium >= price)
        {
            premiumManager.UpdatePremium(-price);
            player.weaponLevel += 1;
            
            player.BuildDoubleTurrets();

            if(nextUpgradeButton != null)
            {
                nextUpgradeButton.interactable = true;
            }

            this.GetComponent<Button>().interactable = false;
        }
    }

    public void TripleTurretUpgrade()
    {
        if(premiumManager.premium >= price)
        {
            premiumManager.UpdatePremium(-price);
            player.weaponLevel += 1;
            
            player.BuildTripleTurrets();

            if(nextUpgradeButton != null)
            {
                nextUpgradeButton.interactable = true;
            }
            
            this.GetComponent<Button>().interactable = false;
        }
    }

    public void LaserUpgrade()
    {
        if(premiumManager.premium >= price)
        {
            premiumManager.UpdatePremium(-price);
            player.weaponLevel += 1;
            
            player.BuildLaserTurret();

            if(nextUpgradeButton != null)
            {
                nextUpgradeButton.interactable = true;
            }
            
            this.GetComponent<Button>().interactable = false;
        }
    }

}
