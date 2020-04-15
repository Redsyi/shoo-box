using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityDirector : MonoBehaviour
{
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
    public float maxDist;
    private float maxDistSqrd => maxDist * maxDist;
    private Player player;

    void Start()
    {
        numTanks = 0;
        numHelis = 0;
        StartCoroutine(Spawn());
        current = this;
        player = FindObjectOfType<Player>();
    }

    IEnumerator Spawn()
    {
        while (!shouldSpawn)
            yield return null;
        while (true)
        {
            while (numTanks < tankCap)
                SpawnTank();
            while (numHelis < heliCap)
                SpawnHeli();
            yield return new WaitForSeconds(Mathf.Max(5, spawnDelay));
        }
    }

    void SpawnTank()
    {
        int idx = Random.Range(0, CityRoad.intersections.Length);
        int maxLoops = 1000;
        int currLoops = 0;
        while ((!coordinatesOffScreen(CityRoad.intersections[idx].transform.position) || (CityRoad.intersections[idx].transform.position - player.transform.position).sqrMagnitude > maxDistSqrd || CityRoad.intersections[idx].assignedTank) && currLoops < maxLoops)
        {
            idx = Random.Range(0, CityRoad.intersections.Length);
            ++currLoops;
        }

        numTanks++;
        Instantiate(tankPrefab, CityRoad.intersections[idx].transform.position, Quaternion.identity);
    }

    void SpawnHeli()
    {
        int idx = Random.Range(0, CityRoad.intersections.Length);
        int maxLoops = 1000;
        int currLoops = 0;
        while ((!coordinatesOffScreen(CityRoad.intersections[idx].transform.position) || (CityRoad.intersections[idx].transform.position - player.transform.position).sqrMagnitude > maxDistSqrd || CityRoad.intersections[idx].assignedHeli) && currLoops < maxLoops)
        {
            idx = Random.Range(0, CityRoad.intersections.Length);
            ++currLoops;
        }

        numHelis++;
        Instantiate(heliPrefab, CityRoad.intersections[idx].transform.position, Quaternion.identity);
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
