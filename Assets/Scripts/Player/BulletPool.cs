using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public GameObject bulletPrefab;
    public int poolSize = 10;
    private Queue<GameObject> bulletPool;

    void Awake()
    {
        bulletPool = new Queue<GameObject>(poolSize);

        for (int i = 0; i < poolSize; i++)
        {
            CreateBullet();
        }
    }

    public GameObject GetBullet()
    {
        if (bulletPool.Count == 0)
        {
            CreateBullet();
        }

        GameObject bulletObject = bulletPool.Dequeue();
        bulletObject.SetActive(true);
        return bulletObject;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }

    private void CreateBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform);
        bullet.GetComponent<Bullet>().bulletPool = this;
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }
}