using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerController : MonoBehaviour
{
    public GameObject bulletPrefab;
    // public Transform firePoint;
    
    private float timeToFire = 0;
    // private CharacterController characterController;
    
    [Header("Base Stats")]
    [SerializeField] private float baseSpeed = 100f;
    [SerializeField] private float baseRange = 20.0f;
    [SerializeField] private float baseFireRate = 0.5f;
    [SerializeField] private float baseDamage = 4;

    [Header(" Stats")]
    public float speed = 80f;
    public float range = 20.0f;
    public float fireRate = 0.5f;
    public float damage = 4f;

    public float speedIncrease = 1.05f;
    public float rangeIncrease = 1.05f;
    public float fireRateIncrease = 0.5f;
    public float damageIncrease = 0.75f;

    public int speedLevel = 1;
    public int rangeLevel = 1;
    public int fireRateLevel = 1;
    public int damageLevel = 1;
    public int baseLevel = 1;
    public int weaponLevel = 1;

    private Vector2 touchStart;
    private Vector2 moveDirection;
    public float bulletSpeed = 20.0f;
    public float minDistance = 2.0f;

    private Rigidbody2D rigidBody;

    public List<GameObject> enemies;

    public Transform nearestEnemy;
    private ScoreManager scoreManager;
    public LevelManager levelManager;
    public Button baseStatsButton;
    public Button baseLevelButton;
    public LayerMask obstacleLayerMask;
    public BulletPool bulletPool;
    public GameObject turretPrefab;

    public Camera mainCamera;
    private AudioSource audioSource;
    public GameObject turrets;

    public Achievements achievements;
    public string achievementID1;
    public string achievementID2;
    public string achievementID3;


    void Awake()
    {
        achievements = GameObject.Find("Achievements").GetComponent<Achievements>();
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        float soundValue = PlayerPrefs.GetFloat("soundVolume", 1f);
        audioSource.volume = 0.1f * soundValue;
        ResetStatLevels();
        rigidBody = GetComponent<Rigidbody2D>();
        enemies = levelManager.enemies;
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        FindNearestEnemy();
    }

    void FixedUpdate()
    {
        HandleInput();
        rigidBody.linearVelocity = moveDirection;
        ClampPosition();
        HandleFiring();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            nearestEnemy = null;
            baseStatsButton.gameObject.GetComponent<HandlePremiumUpgrade>().CheckMaxLevelReached();
            baseLevelButton.gameObject.GetComponent<HandlePremiumUpgrade>().CheckMaxLevelReached();
            
            scoreManager.ResetScore();
            levelManager.ResetLevel();
            
            LaserBeam laserBeam = this.GetComponent<LaserBeam>();
            laserBeam.StopFiring();
            ResetStatLevels();
            GameObject.Find("GameController").GetComponent<GameController>().SaveData();
            //find all childen of gameobject
            HandleUpgrade[] handleUpgradesList = FindObjectsOfType<HandleUpgrade>();
            foreach(HandleUpgrade handleUpgrade in handleUpgradesList)
            {
                handleUpgrade.ResetUpgrades();
            }     
        }
    }

    void Shoot()
    {
        if(weaponLevel == 4)
        {
            LaserBeam laserBeam = this.GetComponent<LaserBeam>();
            laserBeam.beamLength = range;
            laserBeam.damagePerSecond = damage * 12;
            laserBeam.StartFiring();
            return;
        }
        
        //only update target with firingrate but aim everyframe
        int turretCount = turrets.transform.childCount;
        for (int i = 0; i < turretCount; i++)
        {
            Transform turret = turrets.transform.GetChild(i);

            GameObject bullet = bulletPool.GetBullet();
            bullet.transform.position = turret.position;
            bullet.transform.rotation = turret.rotation;
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            bullet.GetComponent<Bullet>().damage = damage;
            bullet.GetComponent<Bullet>().startPosition = turret.position;
            rb.linearVelocity = turret.right * bulletSpeed;
  
        }
        audioSource.Play();
    }

    void InitialSoundPlay()
    {
        audioSource.mute = true;
        // for (int i = 0; i < 5; i++)
        // {
        //     audioSource.Play();
        // }
        audioSource.Play();
        audioSource.mute = false;
    }

    void FindNearestEnemy()
    {
        float distance = float.MaxValue;

        nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float currentDistance = Vector2.Distance(transform.position, enemy.transform.position);

            if ((currentDistance > distance) || (currentDistance > range) || (!enemy.activeSelf))
            {
                continue;
            }
            RaycastHit2D lineOfSight = Physics2D.Raycast(transform.position, (enemy.transform.position - transform.position), currentDistance, obstacleLayerMask);
            if(lineOfSight.collider == null)
            {
                nearestEnemy = enemy.transform;
                distance = currentDistance;
            }
        }
    }

    private void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStart = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 direction = touch.position - touchStart;
                float distanceMeasured = direction.magnitude;
                float distance = Mathf.Min(minDistance, distanceMeasured)/minDistance;

                Vector2 normDirection = direction.normalized;

                float horizontal = normDirection.x * distance * speed * Time.deltaTime;
                float vertical = normDirection.y * distance * speed * Time.deltaTime;

                moveDirection = new Vector2(horizontal, vertical);
            }
        }
        else
        {
            moveDirection = Vector2.zero;
        }
    }

    private void HandleFiring()
    {
        if(weaponLevel == 4 & nearestEnemy != null)
        {
            float angle = Mathf.Atan2(nearestEnemy.position.y - transform.position.y, nearestEnemy.position.x - transform.position.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        
        if (Time.time < timeToFire)
        {
            return;
        }

        FindNearestEnemy();
        if (nearestEnemy != null)
        {
            //aim at enemy
            float angle = Mathf.Atan2(nearestEnemy.position.y - transform.position.y, nearestEnemy.position.x - transform.position.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            
            timeToFire = Time.time + 1 / fireRate;
            Shoot();
        }
    }

    private void ClampPosition()
    {
        // Clamp the position of the this
        Vector2 clampedPosition = rigidBody.position;
        CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
        float maxX = cameraFollow.maxX * 2;
        float maxY = cameraFollow.maxY * 2;

        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -maxX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -maxY, maxY);
        rigidBody.position = clampedPosition;
    }

    public void RaiseBaseStats()
    {
        RaiseBaseLevel();
        ResetStatLevels();
    }

    public void RaiseBaseLevel()
    {
        baseLevel += 1;
    }

    public void ResetStatLevels()
    {
        speedLevel = baseLevel;
        rangeLevel = baseLevel;
        fireRateLevel = baseLevel;
        damageLevel = baseLevel;
        UpdateStats();
    }

    public void UpdateStats()
    {
        speed = baseSpeed * (float) Math.Pow(speedIncrease,(speedLevel-1));
        range = baseRange * (float) Math.Pow(rangeIncrease,(rangeLevel-1));
        fireRate = baseFireRate + fireRateIncrease * (fireRateLevel-1);
        damage = baseDamage + damageIncrease * (damageLevel-1);
    }

    public void RaiseSpeed()
    {
        speedLevel += 1;
        UpdateStats();
    }
    
    public void RaiseRange()
    {
        rangeLevel += 1;
        UpdateStats();
    }
    
    public void RaiseFireRate()
    {
        fireRateLevel += 1;
        UpdateStats();
    }
    
    public void RaiseDamage()
    {
        damageLevel += 1;
        UpdateStats();
    }

    public void SetSaveData(GameData gameData)
    {
        baseLevel = gameData.baseStatsLevel;
        weaponLevel = gameData.weaponLevel;
        ResetStatLevels();
    }

    public void BuildSingleTurret()
    {
        DestroyTurrets();
        float x = 0.33f;
        float y = 0f;

        GameObject newTurret;
        this.transform.rotation = Quaternion.identity;

        newTurret = Instantiate(turretPrefab, this.transform.position + new Vector3(x, y, 0), this.transform.rotation, turrets.transform);
    }

    public void BuildDoubleTurrets()
    {
        DestroyTurrets();
        float x = 0.3f;
        float y = 0.15f;
        GameObject newTurret;
        this.transform.rotation = Quaternion.identity;
        // newTurret = Instantiate(turretPrefab, this.transform, this.Quaternion);
        newTurret = Instantiate(turretPrefab, this.transform.position + new Vector3(x, y, 0), this.transform.rotation, turrets.transform);
        newTurret = Instantiate(turretPrefab, this.transform.position + new Vector3(x, -y, 0), this.transform.rotation, turrets.transform);

        // achievements.UnlockAchievement(achievementID1);
        // StartCoroutine(achievements.WaitAndUnlock(achievementID1));
        achievements.VerifyAchievement(achievementID1);
    }

    public void BuildTripleTurrets()
    {
        DestroyTurrets();
        float x = 0.25f;
        float y = 0.16f;

        GameObject newTurret;
        this.transform.rotation = Quaternion.identity;

        //create 3 equally spaced turrets
        for(int i = -1; i < 2; i++)
        {   
            float j = 0.06f;
            if(i != 0)
            {
                j = 0;
            }
            newTurret = Instantiate(turretPrefab, this.transform.position + new Vector3(x + j, i * y, 0), this.transform.rotation, turrets.transform);
        }

        // achievements.UnlockAchievement(achievementID2);
        // StartCoroutine(achievements.WaitAndUnlock(achievementID1));
        // StartCoroutine(achievements.WaitAndUnlock(achievementID2));
        achievements.VerifyAchievement(achievementID1);
        achievements.VerifyAchievement(achievementID2);
    }

    public void BuildLaserTurret()
    {
        DestroyTurrets();
        float x = 0.3f;
        float y = 0f;
        GameObject newTurret;
        this.transform.rotation = Quaternion.identity;
        
        // newTurret = Instantiate(turretPrefab, this.transform, this.Quaternion);
        newTurret = Instantiate(turretPrefab, this.transform.position + new Vector3(x, y, 0), this.transform.rotation, turrets.transform);
        newTurret.transform.localScale = new Vector3(0.5f, 0.6f, 0.5f);

        // achievements.UnlockAchievement(achievementID3);
        // StartCoroutine(achievements.WaitAndUnlock(achievementID1));
        // StartCoroutine(achievements.WaitAndUnlock(achievementID2));
        // StartCoroutine(achievements.WaitAndUnlock(achievementID3));
        achievements.VerifyAchievement(achievementID1);
        achievements.VerifyAchievement(achievementID2);
        achievements.VerifyAchievement(achievementID3);
    }

    private void DestroyTurrets()
    {
        float turretCount = turrets.transform.childCount;
        for (int i = 0; i < turretCount; i++)
        {
            Transform turret = turrets.transform.GetChild(i);
            Destroy(turret.gameObject);
        }
    }

}