using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// city director that spawns tanks and helicopters at the player
/// </summary>
public class CityDirector : MonoBehaviour
{
    public int intensity;
    float spawnDelay => 20 - intensity;
    private bool shouldSpawn => intensity > 0;
    private int tankCap => ((intensity - 1) / 2) + (PlayerTank.isTank ? (intensity + 1) / 2 : 0);
    private int heliCap => (PlayerTank.isTank ? 0 : ((intensity - 3) / 2));
    private int copCap => (PlayerTank.isTank ? 0 : (intensity > 0 ? 3 : 0));
    private float internalIntensity;
    public static int numTanks;
    public static int numHelis;
    public static int numCops;
    public AITank tankPrefab;
    public AIHeli heliPrefab;
    public AIPolice policePrefab;
    public static CityDirector current;
    public float maxDist;
    private float maxDistSqrd => maxDist * maxDist;
    private Player player;
    public AK.Wwise.Event music;
    public CanvasGroup healthGroup;
    public Image healthFill;

    void Start()
    {
        numTanks = 0;
        numHelis = 0;
        numCops = 0;
        StartCoroutine(Spawn());
        current = this;
        player = Player.current;
    }

    /// <summary>
    /// periodically spawns tanks and helicopters up to their current cap
    /// </summary>
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
            while (numCops < copCap)
                SpawnCop();
            yield return new WaitForSeconds(Mathf.Max(5, spawnDelay));
        }
    }

    /// <summary>
    /// spawns a single tank, selecting a random road near the player but off-screen
    /// </summary>
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

    /// <summary>
    /// spawns a single cop car, selecting a random road near the player but off-screen
    /// </summary>
    void SpawnCop()
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
        AIPolice police = Instantiate(policePrefab, CityRoad.roads[idx].transform.position, Quaternion.identity);
        police.EngagePlayer();
    }

    /// <summary>
    /// spawns a single helicopter, selecting a random road near the player but off-screen
    /// </summary>
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
    
    /// <summary>
    /// effectively !Utilities.OnScreen(), this was made before that was a thing
    /// </summary>
    bool coordinatesOffScreen(Vector3 worldCoords)
    {
        Vector3 screenCoords = CameraScript.current.camera.WorldToScreenPoint(worldCoords);
        Rect screenDimensions = CameraScript.current.camera.pixelRect;
        return (screenCoords.x < 0 || screenCoords.x > screenDimensions.width || screenCoords.y < 0 || screenCoords.y > screenDimensions.height);
    }

    /// <summary>
    /// manually set intensity to a given value, only if the new value is higher than the current intesnity
    /// </summary>
    public void SetIntensity(int newIntensity)
    {
        if (newIntensity > intensity)
        {
            //take special action if we are setting from 0 intensity
            if (intensity == 0)
            {
                music.Post(gameObject);
                StartCoroutine(AnimateHealthBarIn());
            }
            intensity = newIntensity;
            internalIntensity = newIntensity;
        }
    }

    /// <summary>
    /// increase the internal intensity by a decimal amount
    /// </summary>
    public void IncreaseIntensity(float amount)
    {
        internalIntensity += amount;
        SetIntensity(Mathf.FloorToInt(internalIntensity));
    }

    /// <summary>
    /// alert police if player has been off-road for 1.5 seconds
    /// </summary>
    private void Update()
    {
        RoadPlayerDetector.timeOffRoad += Time.deltaTime;
        if (RoadPlayerDetector.timeOffRoad > 1.5f)
        {
            SetIntensity(1);
        }
    }

    /// <summary>
    /// animates in the health bar
    /// </summary>
    IEnumerator AnimateHealthBarIn()
    {
        float healthAppearTime = 0.3f;
        float healthFillTime = 1.5f;
        float timePassed = 0;
        float fillProgress = 0;
        while (fillProgress < 1)
        {
            yield return null;
            timePassed += Time.deltaTime;
            healthGroup.alpha = Mathf.Clamp01(timePassed / healthAppearTime);
            fillProgress = Mathf.Clamp01(timePassed / healthFillTime);
            healthFill.fillAmount = fillProgress;
        }
    }
}
