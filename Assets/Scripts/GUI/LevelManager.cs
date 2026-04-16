using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public GameObject shopMenu;
    public Button baseStatsButton;
    public Button baseLevelButton;
    public GameObject currencyPanel;
    public CurrencyPool currencyPool;
    public int baseLevel = 0;
    public int level = 0;
    public int highestLevel = 0;

    private int enemiesLeft;

    public TextMeshProUGUI levelText;
    public List<GameObject> enemies = new List<GameObject>();
    public GameObject spawners;
    public EnemySpawner[] spawnersArray;
    public WallSpawner wallSpawner;
    public GameController gameController;
    public WallScriptManager wallScriptManager;
    public BackgroundUpdater backgroundUpdater;
    public MusicUpdater musicUpdater;
    public Achievements achievements;
    public string achievementID1;
    public string achievementID2;
    public string achievementID3;

    private void Start()
    {
        levelText.text = "Level: " + level.ToString(); 
        spawnersArray = spawners.GetComponentsInChildren<EnemySpawner>();
    }

    public void StartRound()
    {
        gameController.SaveData();
        AdHandler.Instance.OnLevelStart();

        baseStatsButton.interactable = false;
        baseLevelButton.interactable = false;
        shopMenu.SetActive(false);

        wallScriptManager.EnableWallScript(level);

        //reset all spawners rates
        foreach (EnemySpawner spawner in spawnersArray)
        {   
            spawner.StartRound(level);
        }
    }

    public void EndLevel()
    {
        // AdHandler.Instance.StartLoadingInterstitial();
        AdHandler.Instance.OnLevelEnd();
        level += 1;
        shopMenu.SetActive(true);
        backgroundUpdater.UpdateBackground(level);
        musicUpdater.UpdateMusic(level);
        wallScriptManager.DisableScripts();
        currencyPool.ReturnAllItems();

        // Vector3 position = currencyPanel.transform.localPosition;
        // currencyPanel.transform.localPosition = new Vector3(position.x, 0, position.z);

        gameController.SaveData();
        if(level > highestLevel)
        {
            highestLevel = level;
            if(highestLevel >= 10){
                // achievements.UnlockAchievement(achievementID1);
                achievements.VerifyAchievement(achievementID1);
                }
            if(highestLevel >= 20){
                // achievements.UnlockAchievement(achievementID2);
                achievements.VerifyAchievement(achievementID2);
                }
            if(highestLevel >= 30){
                // achievements.UnlockAchievement(achievementID3);
                achievements.VerifyAchievement(achievementID3);
                }
        }
        levelText.text = "Level: " + level.ToString(); 
    }

    public void AddToEnemiesList(GameObject enemy)
    {
        enemies.Add(enemy);
    }

    public void RemoveFromEnemyList(GameObject enemy)
    {
        enemies.Remove(enemy);

        bool enemySpawnsLeft = false;
        foreach (EnemySpawner spawner in spawnersArray)
        {
            if(spawner.spawning == true)
            {
                enemySpawnsLeft = true;
            }
        }

        if (enemies.Count <= 0 & enemySpawnsLeft == false)
        {
            EndLevel();
        }
    }

    public void ResetLevel()
    {
        level = baseLevel - 1;
        levelText.text = "Level: " + baseLevel.ToString(); 
        
        foreach (EnemySpawner spawner in spawnersArray)
        {
            spawner.spawning = false;
            for(int i = 0; i < spawner.gameObject.transform.childCount; i++)
            {
                GameObject enemy = spawner.gameObject.transform.GetChild(i).gameObject;
                if(enemy.activeSelf)
                {
                    // spawner.enemyPool.ReturnEnemy(enemy);
                    EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                    enemyHealth.Die();
                }
            }
        }

        enemies.Clear();
        //EndLevel();
    }

    public void RaiseBaseLevel()
    {
        baseLevel += 1;

        if(baseLevel >= level)
        {
            level = baseLevel;
            levelText.text = "Level: " + baseLevel.ToString();
            backgroundUpdater.UpdateBackground(level);
        }
    }

    public void PauseGame()
    {
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }

    public void SetSaveData(GameData gameData)
    {
        baseLevel = gameData.baseLevel;
        level = gameData.currentLevel;
        highestLevel = gameData.highestLevel;
        levelText.text = "Level: " + level.ToString(); 
        backgroundUpdater.UpdateBackground(level);
        musicUpdater.UpdateMusic(level);
    }
}





