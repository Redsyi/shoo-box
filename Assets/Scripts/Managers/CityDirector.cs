using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityDirector : MonoBehaviour
{
    public int intensity;
    float spawnDelay => 20 - intensity;
    private bool shouldSpawn => intensity > 0;
    private int tankCap => (intensity / 2) + 1 + (PlayerTank.isTank ? intensity / 2 : 0);
    private int heliCap => (PlayerTank.isTank ? 0 : (intensity / 2));
    private float internalIntensity;
    public static int numTanks;
    public static int numHelis;
    public AITank tankPrefab;
    public AIHeli heliPrefab;
    public static CityDirector current;
    public float maxDist;
    private float maxDistSqrd => maxDist * maxDist;
    private Player player;
    public AK.Wwise.Event music;

    void Start()
    {
        numTanks = 0;
        numHelis = 0;
        StartCoroutine(Spawn());
        current = this;
        player = Player.current;
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
        int idx = Random.Range(0, CityRoad.roads.Length);
        int maxLoops = 1000;
        int currLoops = 0;
        while ((!coordinatesOffScreen(CityRoad.roads[idx].transform.position) || (CityRoad.roads[idx].transform.position - player.transform.position).sqrMagnitude > maxDistSqrd || CityRoad.roads[idx].assignedTank) && currLoops < maxLoops)
        {
            idx = Random.Range(0, CityRoad.roads.Length);
            ++currLoops;
        }

        numTanks++;
        Instantiate(tankPrefab, CityRoad.roads[idx].transform.position, Quaternion.identity);
    }

    void SpawnHeli()
    {
        int idx = Random.Range(0, CityRoad.roads.Length);
        int maxLoops = 1000;
        int currLoops = 0;
        while ((!coordinatesOffScreen(CityRoad.roads[idx].transform.position) || (CityRoad.roads[idx].transform.position - player.transform.position).sqrMagnitude > maxDistSqrd || CityRoad.roads[idx].assignedHeli) && currLoops < maxLoops)
        {
            idx = Random.Range(0, CityRoad.roads.Length);
            ++currLoops;
        }

        numHelis++;
        Instantiate(heliPrefab, CityRoad.roads[idx].transform.position, Quaternion.identity);
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
            if (intensity == 0)
                music.Post(gameObject);
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
