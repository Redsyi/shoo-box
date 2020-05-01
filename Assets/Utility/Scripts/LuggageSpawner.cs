using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// spawns luggage for the conveyors area
/// </summary>
public class LuggageSpawner : MonoBehaviour
{
    public float spawnFrequency;
    public GameObject[] luggagePrefabs;
    public Vector3[] rotations;
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
            Instantiate(luggagePrefabs[randomIdx], transform.position, luggagePrefabs[randomIdx].transform.rotation);
        }
    }
}
