using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemySpawner : MonoBehaviourPun
{
    public string enemyPrefabPath;
    public float maxEnemies;
    public float spawnRadius;
    public float spawnCheckTime;
    private float lastSpawnCheckTime;
    public List<GameObject> currentEnemies = new List<GameObject>();



    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (Time.time - lastSpawnCheckTime > spawnCheckTime)
        {
            lastSpawnCheckTime = Time.time;
            TrySpawn();
        }
    }


    void TrySpawn()
    {
        //remove any dead enemies from list
        for (int i = 0; i < currentEnemies.Count; i++)
        {
            if (!currentEnemies[i])
            {
                currentEnemies.RemoveAt(i);
            }
        }

        //if we have max number of enemy then return
        if (currentEnemies.Count > maxEnemies)
        {
            return;
        }

        //otherwise spawn enemies
        Vector3 randomIncircle = Random.insideUnitCircle * spawnRadius;
        GameObject enemy = PhotonNetwork.Instantiate(enemyPrefabPath, transform.position + randomIncircle, Quaternion.identity);
        currentEnemies.Add(enemy);


    }

}
