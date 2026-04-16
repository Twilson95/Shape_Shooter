using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float maxDistance = 25.0f;
    public float damage = 10;
    public Vector3 startPosition;
    public BulletPool bulletPool;
    //public float returnTime; 
    // private AudioSource audioSource;

    void Start()
    {
        startPosition = transform.position;
        // audioSource = GetComponent<AudioSource>();
        // bulletPool = GameObject.Find("BulletPool").GetComponent<BulletPool>();

        // bulletPool = transform.gameObject.GetComponent<BulletPool>();
    }

    void OnEnable() 
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float distance = Vector3.Distance(startPosition, transform.position);
        if (distance >= maxDistance)
        {
            RemoveBullet();
        }
    }

    // private void OnTriggerEnter2D(Collider2D collision)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collider = collision.collider;
        if (collider.CompareTag("Enemy"))
        {
            HitParticle hitParticle = collider.gameObject.GetComponentInChildren<HitParticle>();
            if (hitParticle != null)
            {
                hitParticle.SetPosition(collision.GetContact(0).point);
                // hitParticle.StartParticles();
                hitParticle.EmitParticles(1);
            }
        
            // Deal damage to the enemy
            EnemyHealth enemy = collider.GetComponent<EnemyHealth>();
            enemy.TakeDamage(damage);
            // audioSource.Play();
        }

        RemoveBullet();
    }

    private void RemoveBullet()
    {
        // Destroy(gameObject);
        gameObject.SetActive(false);
        bulletPool.ReturnBullet(gameObject);
    }
}