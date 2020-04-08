using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityDirector : MonoBehaviour
{
    private List<Transform> spawnPoints;
    public int intensity;
    float spawnDelay => 20 - intensity;
    private bool shouldSpawn => intensity > 0;
    private int tankCap => (intensity / 2) + 1;
    private int heliCap => (intensity / 2);
    private float internalIntensity;
    public static int numTanks;
    public static int numHelis;
    public AITank tankPrefab;
    public AIHeli heliPrefab;
    public static CityDirector current;

    void Start()
    {
        spawnPoints = new List<Transform>();
        foreach (Transform child in GameObject.FindGameObjectWithTag("Intersections").transform)
        {
            spawnPoints.Add(child);
        }
        numTanks = 0;
        numHelis = 0;
        StartCoroutine(Spawn());
        current = this;
    }

    IEnumerator Spawn()
    {
        while (!shouldSpawn)
            yield return null;
        while (true)
        {
            if (numTanks < tankCap)
                SpawnTank();
            if (numHelis < heliCap)
                SpawnHeli();
            yield return new WaitForSeconds(Mathf.Max(5, spawnDelay));
        }
    }

    void SpawnTank()
    {
        int idx = Random.Range(0, spawnPoints.Count);
        int maxLoops = 1000;
        int currLoops = 0;
        while (!coordinatesOffScreen(spawnPoints[idx].position) && currLoops < maxLoops)
        {
            idx = Random.Range(0, spawnPoints.Count);
            ++currLoops;
        }

        Instantiate(tankPrefab, spawnPoints[idx].position, Quaternion.identity);
    }

    void SpawnHeli()
    {
        int idx = Random.Range(0, spawnPoints.Count);
        int maxLoops = 1000;
        int currLoops = 0;
        while (!coordinatesOffScreen(spawnPoints[idx].position) && currLoops < maxLoops)
        {
            idx = Random.Range(0, spawnPoints.Count);
            ++currLoops;
        }

        Instantiate(heliPrefab, spawnPoints[idx].position, Quaternion.identity);
    }

    bool coordinatesOffScreen(Vector3 worldCoords)
    {
        Vector3 screenCoords = CameraScript.current.camera.WorldToScreenPoint(worldCoords);
        Rect screenDimensions = CameraScript.current.camera.pixelRect;
        return (screenCoords.x < 0 || screenCoords.x > screenDimensions.width || screenCoords.y < 0 || screenCoords.y > screenDimensions.height);
    }

    public void SetIntensity(int newIntensity)
    {
        if (newIntensity > intensity)
        {
            intensity = newIntensity;
            internalIntensity = newIntensity;
        }
    }

    public void IncreaseIntensity(float amount)
    {
        internalIntensity += amount;
        SetIntensity(Mathf.FloorToInt(internalIntensity));
    }
}
