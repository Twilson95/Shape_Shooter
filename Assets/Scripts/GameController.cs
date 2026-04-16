using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Threading;
using System.Collections;

public class GameController : MonoBehaviour
{
    private GameData gameData;
    public LevelManager levelManager;
    public PlayerController player;
    public CoinMagnet coinMagnet;
    public ScoreManager scoreManager;
    public PremiumManager premiumManager;
    public Button UpgradeButton1;
    public Button UpgradeButton2;
    public Button UpgradeButton3;

    private Thread saveThread;

    private void Awake()
    {
        // Load the saved player data
        gameData = SaveScript.LoadGameData();
        if(gameData != null)
        {
            levelManager.SetSaveData(gameData);
            scoreManager.SetSaveData(gameData);
            premiumManager.SetSaveData(gameData);
            player.SetSaveData(gameData);
            coinMagnet.SetSaveData(gameData);
        
            // SetWeapons();
        }
    }

    void Start()
    {
        SetWeapons();
    }

    public void SaveData()
    {
        SetGameData();
        // SaveScript.SaveGameDataAsync(gameData);
        StartCoroutine(SaveScript.SaveGameDataAsync(gameData));
    }

    // private void OnApplicationQuit()
    // {
    //     SaveData();
    // }

    private void SetGameData()
    {
        gameData = new GameData();
        gameData.baseLevel = levelManager.baseLevel;
        gameData.currentLevel = levelManager.level;
        gameData.highestLevel = levelManager.highestLevel;
        gameData.score = scoreManager.score;
        gameData.premiumScore = premiumManager.premium;
        gameData.weaponLevel = player.weaponLevel;
        gameData.baseStatsLevel = player.baseLevel;
        gameData.magnetLevel = coinMagnet.magnetLevel;
    }

    public void ResetGameData()
    {
        levelManager.EndLevel();

        gameData.baseLevel = 1;
        gameData.currentLevel = 1;
        gameData.highestLevel = 1;
        gameData.score = 0;
        gameData.premiumScore = 0;
        gameData.weaponLevel = 1;
        gameData.baseStatsLevel = 1;
        gameData.magnetLevel = 0;

        SaveScript.SaveGameData(gameData);
        gameData = SaveScript.LoadGameData();

        levelManager.SetSaveData(gameData);
        levelManager.level = gameData.baseLevel;
    
        scoreManager.ResetScore();
        premiumManager.ResetPremium();

        player.SetSaveData(gameData);
        coinMagnet.SetSaveData(gameData);

        SetWeapons();

        HandleUpgrade[] handleUpgradesList = FindObjectsOfType<HandleUpgrade>();
        foreach(HandleUpgrade handleUpgrade in handleUpgradesList)
        {
            handleUpgrade.baseLevel = 1;
            handleUpgrade.ResetUpgrades();
            handleUpgrade.button.interactable = true;
        }

        HandlePremiumUpgrade[] handlePremiumUpgradesList = FindObjectsOfType<HandlePremiumUpgrade>();
        foreach(HandlePremiumUpgrade handlePremiumUpgrade in handlePremiumUpgradesList)
        {
            // handlePremiumUpgrade.upgradeLevel = 1;
            handlePremiumUpgrade.upgradeLevel = handlePremiumUpgrade.initialLevel;
            handlePremiumUpgrade.ResetUpgrades();
            handlePremiumUpgrade.thisButton.interactable = true;
        }
    }

    private void SetWeapons()
    {
        // Debug.Log("SettingWeapons");
        // yield return new WaitForSeconds(1f);
        // Debug.Log("Wait Finished");

        if(player.weaponLevel == 1)
        {
            player.BuildSingleTurret();
            UpgradeButton1.interactable = true;
            UpgradeButton2.interactable = false;
            UpgradeButton3.interactable = false;
        }
        else if(player.weaponLevel == 2)
        {
            player.BuildDoubleTurrets();
            UpgradeButton1.interactable = false;
            UpgradeButton2.interactable = true;
            UpgradeButton3.interactable = false;
        }
        else if(player.weaponLevel == 3)
        {
            player.BuildTripleTurrets();
            UpgradeButton1.interactable = false;
            UpgradeButton2.interactable = false;
            UpgradeButton3.interactable = true;
        }
        else if(player.weaponLevel == 4)
        {
            player.BuildLaserTurret();
            UpgradeButton1.interactable = false;
            UpgradeButton2.interactable = false;
            UpgradeButton3.interactable = false;
        }
        // Debug.Log("Weapons Set");
    }

    public void QuiteGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
