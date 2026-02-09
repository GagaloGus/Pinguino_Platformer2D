using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner1 : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnTime = 5f;
    public int maxEnemies = 2;

    public List<GameObject> enemies = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        if (enemies.Count < maxEnemies)
        {
            GameObject enemy = Instantiate(
                enemyPrefab,
                transform.position,
                Quaternion.identity
            );

            enemies.Add(enemy);
        }

        yield return new WaitForSeconds(spawnTime);
    }
}