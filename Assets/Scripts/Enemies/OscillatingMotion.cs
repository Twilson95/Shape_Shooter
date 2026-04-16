using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillatingMotion : MonoBehaviour
{
    private Transform player;
    private LevelManager levelManager;

    public float initialSpeed = 5.0f;
    public float speed = 5.0f;
    public float amplitude = 1.0f;
    public float initialFrequency = 1.0f;
    public float frequency = 1.0f;
    public float bossSpeedMultiplier = 1.0f;
    public bool isFrozen;

    private Rigidbody2D rb2d;

    void Awake()
    {
        // StartCoroutine(FindLevelManager());
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
    }

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
        // levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
    }

    void OnEnable() 
    {
        StartCoroutine(ResetStatsCoroutine());
        // ResetStats();
    }

    IEnumerator ResetStatsCoroutine()
    {
        while(levelManager == null)
        {
            // yield return new WaitForSeconds(0.1f);
            yield return null;
        }
        ResetStats();
    }

    void FixedUpdate()
    {
        if(isFrozen)
        {
            return;
        }
        HandleOscillatingMotion();
    }

    public IEnumerator FreezeMovement(float delayTime)
    {
        isFrozen = true;
        yield return new WaitForSeconds(delayTime);
        isFrozen = false;
    }

    private void HandleOscillatingMotion()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 perpendicular = new Vector2(direction.y, -direction.x);

        Vector2 oscillation = perpendicular * Mathf.Sin(Time.time * frequency) * amplitude;
        
        rb2d.linearVelocity = Vector2.MoveTowards(rb2d.linearVelocity, direction * speed + oscillation, Time.fixedDeltaTime * frequency * 5);
    }

    private void ResetStats()
    {
        isFrozen = false;
        float speedScaler = 1.0f + (levelManager.level - 1) * 0.05f;

        speed = initialSpeed * speedScaler;
        frequency = initialFrequency * speedScaler;
    }

    public void SetBoss()
    {
        speed *= bossSpeedMultiplier;
        frequency *= bossSpeedMultiplier;
    }
}
