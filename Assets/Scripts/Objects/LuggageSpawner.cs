using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuggageSpawner : MonoBehaviour
{
    public float spawnFrequency;
    public GameObject[] luggagePrefabs;
    private float currSpawnTimer;

    void Update()
    {
        if (ConveyorBelt.active)
        {
            currSpawnTimer += Time.deltaTime;
        }

        if (currSpawnTimer >= spawnFrequency)
        {
            currSpawnTimer -= spawnFrequency;
            int randomIdx = Random.Range(0, luggagePrefabs.Length);
            Instantiate(luggagePrefabs[randomIdx], transform.position, Quaternion.identity);
        }
    }
}
