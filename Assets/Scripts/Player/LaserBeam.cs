using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public float beamLength = 100f;
    public float beamWidth = 0.1f;
    public float beamDuration = 0.1f;
    public float damagePerSecond = 10f;
    public LayerMask collidableLayers;
    public ParticleSystem hitParticle;
    private Transform turret;
    private Vector3 beamStartPosition;

    private Vector3 beamEndPosition;
    private LineRenderer lineRenderer;
    public bool isFiring = false;
    private float timer = 0;
    private float particleCooldown = 0;
    public float cooldownLimit = 0.05f;

    void Start()
    {
        turret = transform.GetChild(0);
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = beamWidth;
        lineRenderer.endWidth = beamWidth;
    }

    void Update()
    {
        if(turret == null)
        {
            turret = transform.GetChild(0);
        }

        beamStartPosition = turret.position;
        if (isFiring)
        {
            Vector3 startPosition = new Vector3(0.8f, 0, 0);
            RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.right.normalized * 0.8f, transform.right, beamLength, collidableLayers);
            if (hit.collider != null)
            {
                beamEndPosition = hit.collider.transform.position - (transform.right.normalized * 0.2f);
                EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {   
                    enemyHealth.TakeDamage(damagePerSecond * Time.deltaTime);

                    particleCooldown += Time.deltaTime;
                    if(particleCooldown > cooldownLimit)
                    {
                        particleCooldown = 0;
                        HitParticle hitParticle = hit.collider.gameObject.GetComponentInChildren<HitParticle>();
                        hitParticle.SetPosition(beamEndPosition);
                        hitParticle.EmitParticles(1);
                    }
                }
            }
            else
            {
                beamEndPosition = transform.position + transform.right.normalized * beamLength;
            }
            
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, turret.position + transform.right.normalized * 0.4f);
            lineRenderer.SetPosition(1, beamEndPosition);

            timer += Time.deltaTime;
            if(timer >= beamDuration)
            {
                StopFiring();
                timer = 0;
            }
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    public void StartFiring()
    {
        isFiring = true;
    }

    public void StopFiring()
    {
        // lineRenderer.enabled = false;
        isFiring = false;
    }
}