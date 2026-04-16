using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShooting : MonoBehaviour
{
    private float timer;
    private float timeToFire;
    public float fireRate;
    public float fireSpeed;
    public float fireRange;
    public float delayTime = 1f;
    public EnemyPool enemyPool;
    private Transform player;
    public LayerMask obstacleLayerMask;
    private UnityEngine.Rendering.Universal.Light2D bossLight;
    private AudioSource audioSource;
    public AudioClip audioClip1;
    public AudioClip audioClip2;
    private MusicUpdater musicUpdater;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // find the player object in the scene
        musicUpdater = GameObject.Find("Music").GetComponent<MusicUpdater>();        
        audioSource = GetComponent<AudioSource>();
        timeToFire = Time.time + timeToFire;
        // GameObject enemy = enemyPool.enemyPool.Peek();
        // SpriteRenderer spriteRenderer = otherGameObject.GetComponent<SpriteRenderer>();
        // Sprite sprite = spriteRenderer.sprite;
        bossLight = this.GetComponent<UnityEngine.Rendering.Universal.Light2D>();   
    }

    void OnEnable() 
    {
        bossLight = this.GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        bossLight.enabled = true;

        EnemyMovement enemyMovement = GetComponent<EnemyMovement>();
        enemyMovement.SetBoss();
        OscillatingMotion oscillatingMotion = GetComponent<OscillatingMotion>();
        oscillatingMotion.SetBoss();
        EnemyHealth enemyHealth = GetComponent<EnemyHealth>();
        enemyHealth.SetBoss();
    }

    void OnDisable() 
    {
        bossLight.enabled = false;
    }

    void Update()
    {
        HandleShooting();
    }

    void HandleShooting()
    {
        Vector3 playerDirection = (player.position - transform.position).normalized;
        float playerDistance = Vector2.Distance(transform.position, player.position);
        if (playerDistance > fireRange)
        {
            ResetCharge();
            return;
        }

        RaycastHit2D lineOfSight = Physics2D.Raycast(transform.position, playerDirection, playerDistance, obstacleLayerMask);
        if(lineOfSight.collider != null)
        {
            ResetCharge();
            return;
        }

        if (Time.time < timeToFire)
        {
            // if (!audioSource.isPlaying)
            // {
            //     audioSource.Play();
            // }
            float difference = timeToFire - Time.time;
            bossLight.pointLightOuterRadius = Mathf.Lerp(1.5f, 0 , difference);
            return;
        }
        ResetCharge();

        GameObject enemy = enemyPool.GetEnemy();
        if(enemy == null)
        {
            return;
        }

        DelayMotion(enemy);

        enemy.transform.position = (Vector3) transform.position + (playerDirection * 0.5f);
        Rigidbody2D enemyRigidBody = enemy.GetComponent<Rigidbody2D>();
        enemyRigidBody.linearVelocity = fireSpeed * playerDirection;
        audioSource.volume = musicUpdater.bossVolume;
        // audioSource.clip = audioClip2;
        audioSource.Play();
        // audioSource.clip = audioClip1;
    }

    void ResetCharge()
    {
        // audioSource.Stop();
        bossLight.pointLightOuterRadius = 0;
        timeToFire = Time.time + 1/fireRate;
    }

    // IEnumerator DelayMotion(GameObject enemy)
    private void DelayMotion(GameObject enemy)
    {
        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        OscillatingMotion oscillatingMotion = enemy.GetComponent<OscillatingMotion>();

        if(enemyMovement.enabled)
        {
            StartCoroutine(enemyMovement.FreezeMovement(delayTime));
        }

        if(oscillatingMotion.enabled)
        {
            StartCoroutine(oscillatingMotion.FreezeMovement(delayTime));
        }
    }
}
