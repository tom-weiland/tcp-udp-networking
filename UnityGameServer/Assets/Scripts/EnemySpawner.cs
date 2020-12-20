using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float frequency = 3f;

    private void Start()
    {
        Debug.Log("enemies disabled due to bug");
        //StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(frequency);

        if (Enemy.enemies.Count < Enemy.maxEnemies)
        {
            NetworkManager.instance.InstantiateEnemy(transform.position);
        }
        StartCoroutine(SpawnEnemy());
    }
}
