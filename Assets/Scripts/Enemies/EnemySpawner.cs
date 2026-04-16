using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyPool enemyPool;
    public float initialSpawnInterval = 5.0f;
    public float spawnInterval = 5.0f;
    public float screenPadding = 4.0f;

    public float spawnRateIncrease = 0.95f;
    //public float increaseInterval = 10.0f;
    //public float increaseTimer = 0.0f;

    private float spawnTimer = 0.0f;
    public int levelStart; 

    public float baseMaxEnemies = 0;
    public float maxEnemies = 10;
    public int spawnedEnemies = 0;
    public float raidMaxEnemyMultiplier = 3f;
    public float raidSpawnRateMultiplier = 2f;
    public LevelManager levelManager;
    public int bossStartRound = 5;
    public int bossFrequency = 15;
    public bool spawning = false;
    public Transform player;



    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
    }
    
    void Update()
    {
        if((levelManager.level - bossStartRound + 1) % bossFrequency == 0)
        {
            HandleRaidSpawning();
            return;
        }
        else if(levelManager.level % 5 == 4)
        {
            spawning = false;
            return;
        }
        else
        {
            HandleSpawning();
            return;
        }
    }

    private void HandleSpawning()
    {
        if(spawnedEnemies >= maxEnemies | levelManager.level < levelStart)
        {
            spawnedEnemies = (int) maxEnemies;
            spawning = false;
            return;
        }
        if(spawning == false){
            return;
        }
        spawnTimer += Time.deltaTime;
        
        // increaseTimer += Time.deltaTime;
        // if(increaseTimer >= increaseInterval)
        // {
        //     spawnInterval *= spawnRateIncrease;
        //     increaseTimer = 0.0f;
        // }

        if (spawnTimer >= spawnInterval)
        {
            Vector2 spawnLocation = RandomSpawnLocation();
            GameObject enemy = enemyPool.GetEnemy();
            
            //if enemy pool is empty do not create an enemy this time
            if(enemy == null)
            {
                spawnTimer = 0.0f;
                return;
            }
            enemy.transform.position = (Vector3) spawnLocation;

            // float healthScaler = 1.0f + (levelManager.level - 1) * 0.1f;
            // float speedScaler = 1.0f + (levelManager.level - 1) * 0.05f;
            // EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            // enemyHealth.isDead = false;
            // enemyHealth.maxHealth *= healthScaler;
            // enemyHealth.currentHealth = enemyHealth.maxHealth; 
            // try
            // {
            //     enemy.GetComponent<EnemyMovement>().speed *= speedScaler;
            // }
            // catch
            // {
            //     enemy.GetComponent<OscillatingMotion>().speed *= speedScaler;
            //     enemy.GetComponent<OscillatingMotion>().frequency *= speedScaler;
            // }

            // levelManager.AddToEnemiesList(enemy);
            spawnedEnemies += 1;

            spawnTimer = 0.0f;
        }
    }

    private void HandleRaidSpawning()
    {
        if(spawnedEnemies >= maxEnemies * raidMaxEnemyMultiplier | levelManager.level < levelStart)
        {
            spawnedEnemies = (int) (maxEnemies * raidMaxEnemyMultiplier);
            spawning = false;
            return;
        }
        if(spawning == false){
            return;
        }
        spawnTimer += Time.deltaTime;
        
        if (spawnTimer >= spawnInterval / raidSpawnRateMultiplier)
        {
            Vector2 spawnLocation = RandomSpawnLocation();
            GameObject enemy = enemyPool.GetEnemy();
            
            //if enemy pool is empty do not create an enemy this time
            if(enemy == null)
            {
                spawnTimer = 0.0f;
                return;
            }
            enemy.transform.position = (Vector3) spawnLocation;

            spawnedEnemies += 1;

            spawnTimer = 0.0f;
        }
    }

    Vector2 RandomSpawnLocation()
    {
        float camHeight = Camera.main.orthographicSize * 1.1f;
        float camWidth = camHeight * Camera.main.aspect;

        // Get a random point on the unit circle
        Vector2 randomPoint = Random.insideUnitCircle.normalized;

        // Convert the point to a direction relative to the player's position
        Vector3 direction = new Vector3(randomPoint.x, randomPoint.y, 0);
        Vector2 spawnPosition = (Vector2) (player.position + direction * camWidth * 1.2f);

        return spawnPosition;
    }

    public void SpawnBoss()
    {   
        Vector2 spawnLocation = RandomSpawnLocation();
        if(levelManager.level < bossStartRound)
        {
            return;
        }
        if((levelManager.level - bossStartRound) % bossFrequency == 0)
        {
            GameObject largerEnemy = enemyPool.GetEnemy();
            largerEnemy.transform.position = (Vector3) spawnLocation;

            BossShooting bossShooting = largerEnemy.GetComponent<BossShooting>();
            bossShooting.enabled = true;
            bossShooting.enemyPool = enemyPool;
            
        }
    }

    public void StartRound(int level)
    {
        spawnInterval = initialSpawnInterval * Mathf.Pow(spawnRateIncrease,(level - levelStart - 1));
        maxEnemies = baseMaxEnemies * MaxEnemiesScaling(level);
        spawnedEnemies = 0;
        spawning = true;
        SpawnBoss();
    }

    private int MaxEnemiesScaling(int level)
    {
        return (int) Mathf.Floor(level * 1.2f);
    }

}