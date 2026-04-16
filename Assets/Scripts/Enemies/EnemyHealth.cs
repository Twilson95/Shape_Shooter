using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float currentHealth;
    public float initialMaxHealth = 100f;
    public float maxHealth = 100f;
    public float bossHealthMultiplier = 10f;
    public int bossScoreMultiplier = 50;
    public Vector3 initialScale;
    public Image healthBar;
    public GameObject healthBarObject;
    private GameObject player;

    private ScoreManager scoreManager;
    private PremiumManager premiumManager;
    public LevelManager levelManager;
    private Achievements achievements;
    public string achievementID1;
    public string achievementID2;
    public string achievementID3;
    
    private EnemyPool enemyPool;
    private ScorePool scorePool;
    public int score = 3;
    public GameObject scorePrefab;

    public float recalculateInterval;
    public float screenBound = 1f;

    public bool isDead;
    public bool isBoss;

    void Awake()
    {
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        initialScale = transform.localScale;
    }

    void Start()
    {
        currentHealth = maxHealth;
        healthBarObject.SetActive(false);
        player = GameObject.Find("PlayerCircle");
        enemyPool = transform.parent.gameObject.GetComponent<EnemyPool>();
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        premiumManager = GameObject.Find("PremiumManager").GetComponent<PremiumManager>();
        scorePool = GameObject.Find("ScorePool").GetComponent<ScorePool>();
        achievements = GameObject.Find("Achievements").GetComponent<Achievements>();
        // mainCamera = GameObject.Find("ScorePool").GetComponent<ScorePool>();

        InvokeRepeating("CheckOutOFBounds", 0f, recalculateInterval);
    }

    void OnEnable()
    {
        ResetStats();
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.fillAmount = currentHealth / maxHealth;

        if (currentHealth < maxHealth)
        {
            healthBarObject.SetActive(true);
        }
        if (currentHealth > 0)
        {
            return;
        }
        if (isDead)
        {
            return;
        }
        isDead = true;
        // premiumManager.UpdatePremium(score);
        scoreManager.UpdateScore(score);
        // scorePool.GetScoreObject(transform.position, score);
        TMP_Pool.Instance.SetObject(transform.position, score);

        GameObject currencyObject = CurrencyPool.Instance.GetItem();
        currencyObject.transform.position = transform.position;
        DroppedCurrency droppedCurrency = currencyObject.GetComponent<DroppedCurrency>();
        
        int givenScore = score;
        BossShooting bossShooting = GetComponent<BossShooting>();
        if(bossShooting.enabled)
        {
            givenScore *= bossScoreMultiplier;
        }
        
        droppedCurrency.SetScore(givenScore);

        achievements.IncrementAchievement(achievementID1);
        achievements.IncrementAchievement(achievementID2);
        achievements.IncrementAchievement(achievementID3);

        StartCoroutine(Killed());
    }

    IEnumerator Killed()
    {
        player.GetComponent<PlayerController>().nearestEnemy = null;
        player.GetComponent<LaserBeam>().StopFiring();
        
        transform.position = new Vector3(0, -100, 0);

        yield return new WaitForSeconds(1.0f);
        
        Die();
    }


    public void Die()
    {
        currentHealth = maxHealth;
        healthBar.fillAmount = 1.0f;
        healthBarObject.SetActive(false);
        
        BossShooting bossShooting = this.GetComponent<BossShooting>();
        bossShooting.enabled = false;

        enemyPool.ReturnEnemy(gameObject);
    }

    private void ResetStats()
    {
        float healthScaler = 1.0f + (levelManager.level - 1) * 0.1f;
        maxHealth = initialMaxHealth * healthScaler;
        transform.localScale = initialScale;

        currentHealth = maxHealth;
        isDead = false;
    }

    public void SetBoss()
    {
        maxHealth *= bossHealthMultiplier;
        currentHealth = maxHealth;
        transform.localScale *= 2.0f;
    }

    private void CheckOutOFBounds()
    {
        float camHeight = Camera.main.orthographicSize * 1.1f;
        float camWidth = camHeight * Camera.main.aspect;
        screenBound = Screen.width;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        if (screenPosition.x < -screenBound || screenPosition.x > Screen.width + screenBound ||
            screenPosition.y < -screenBound || screenPosition.y > Screen.height + screenBound)
        {
            Debug.Log("Enemy went out of bounds");
            Die();
        }
    }
}





