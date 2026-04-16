using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int poolSize = 10;
    private Queue<GameObject> enemyPool;
    public LevelManager levelManager;

    void Start()
    {
        StartCoroutine(FillEnemyPool());
    }

    IEnumerator FillEnemyPool()
    {
        enemyPool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, transform);
            enemy.SetActive(false);
            enemyPool.Enqueue(enemy);
            yield return null;
        }
    }

    public GameObject GetEnemy()
    {
        // Search for an inactive enemy in the pool and return it
        if (enemyPool.Count == 0)
        {
            return null;
        }

        GameObject enemy = enemyPool.Dequeue();
        enemy.SetActive(true);
        levelManager.AddToEnemiesList(enemy);
        return enemy;
    }

    public void ReturnEnemy(GameObject enemy)
    {
        levelManager.RemoveFromEnemyList(enemy);
        enemyPool.Enqueue(enemy);
        enemy.SetActive(false);
    }

}