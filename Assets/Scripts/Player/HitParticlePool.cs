using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class HitParticlePool : MonoBehaviour
{
    public GameObject hitParticlePrefab;
    public int poolSize = 10;
    public float despawnTime = 5f;

    private Queue<GameObject> hitParticleQueue;
    private Queue<GameObject> hitParticleOrder;

    void Awake()
    {
        hitParticleQueue = new Queue<GameObject>(poolSize);
        hitParticleOrder = new Queue<GameObject>(poolSize);

        for (int i = 0; i < poolSize; i++)
        {
            GameObject hitParticle = Instantiate(hitParticlePrefab, transform);
            hitParticle.SetActive(false);
            hitParticleQueue.Enqueue(hitParticle);
            hitParticleOrder.Enqueue(hitParticle);
        }
    }

    public void GetHitParticle(Vector3 position)
    {
        GameObject hitParticle;
        if (hitParticleQueue.Count == 0)
        {
            hitParticle = GetOldestHitParticle();
            ReturnHitParticle(hitParticle);
        }

        hitParticle = hitParticleQueue.Dequeue();
        hitParticle.transform.position = position;
        hitParticle.SetActive(true);

        StartCoroutine(DelayedReturn(hitParticle));
    }

    private GameObject GetOldestHitParticle()
    {
        if(hitParticleOrder.Count == 0)
        {
            return null;
        }

        GameObject oldestHitParticle = hitParticleOrder.Peek();
        return oldestHitParticle;
    }

    IEnumerator DelayedReturn(GameObject hitParticle)
    {
        yield return new WaitForSeconds(despawnTime);
        ReturnHitParticle(hitParticle);
    }

    public void ReturnHitParticle(GameObject hitParticle)
    {
        hitParticle.SetActive(false);
        hitParticleQueue.Enqueue(hitParticle);
        hitParticleOrder.Enqueue(hitParticleOrder.Dequeue());
    }
}