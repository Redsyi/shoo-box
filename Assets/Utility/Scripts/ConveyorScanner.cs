using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// conveyor belt scanner
/// </summary>
public class ConveyorScanner : MonoBehaviour, IAIInteractable
{
    [SerializeField] private string bridgeText = "You were scanned by security";
    public float playerCatchWaitTime;
    private Player player;
    private bool caught;
    private Light[] warningLights;
    public float minLightIntensity;
    public float maxLightIntensity;
    public float lightFluxSpeed = 1f;
    private float alarmTriggerTime;
    bool doFalseAlarm;
    public AIInterest[] interestMask;
    public AIAgent investigatorPrefab;
    const float spawnDelay = 0.4f;
    public Vector3 spawnPos;

    void Start()
    {
        player = Player.current;
        warningLights = GetComponentsInChildren<Light>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!caught)
        {
            caught = true;
            alarmTriggerTime = Time.time;
            StartCoroutine(CatchPlayer());
        }
    }

    /// <summary>
    /// spawns a TSA agent to go after the player
    /// </summary>
    IEnumerator SpawnAgent()
    {
        yield return new WaitForSeconds(spawnDelay);
        AIAgent agent = Instantiate(investigatorPrefab, transform.position + spawnPos, Quaternion.identity);
        yield return null;
        agent.Interact(this);
    }

    /// <summary>
    /// triggers when the player enters the scanner. activates the alarms and spawns a tsa agent after a bit
    /// </summary>
    IEnumerator CatchPlayer()
    {
        StartCoroutine(SpawnAgent());
        float remainingWaitTime = playerCatchWaitTime;
        ConveyorBelt.active = false;
        yield return null;
        player.lockMovement = true;
        player.lockChangeForm = true;
        while (remainingWaitTime > 0)
        {
            float lightIntensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, (Mathf.Sin((Time.time - alarmTriggerTime) * lightFluxSpeed) + 1) / 2f);
            foreach (Light light in warningLights)
            {
                light.intensity = lightIntensity;
            }
            ConveyorBelt.active = false;
            yield return null;
            remainingWaitTime -= Time.deltaTime;
        }
        LevelBridge.Reload(bridgeText);
    }

    /// <summary>
    /// flashes the lights without doing anything else
    /// </summary>
    public void FakeAlarm()
    {
        doFalseAlarm = true;
        StartCoroutine(FlashLights());
    }

    public void DelayedFakeAlarm(float delay)
    {
        Invoke("FakeAlarm", delay);
    }

    public void CancelFakeAlarm()
    {
        CancelInvoke("FakeAlarm");
        doFalseAlarm = false;
        foreach (Light light in warningLights)
        {
            light.intensity = 0;
        }
    }

    IEnumerator FlashLights()
    {
        while (doFalseAlarm)
        {
            float lightIntensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, (Mathf.Sin((Time.time - alarmTriggerTime) * lightFluxSpeed) + 1) / 2f);
            foreach (Light light in warningLights)
            {
                light.intensity = lightIntensity;
            }
            yield return null;
        }
    }

    public float AIInteractTime()
    {
        return 999;
    }

    public void AIFinishInteract(AIAgent ai)
    {
    }

    public void AIInteracting(float interactProgress)
    {
    }

    public bool NeedsInteraction()
    {
        return false;
    }

    public AIInterest[] InterestingToWhatAI()
    {
        return interestMask;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + spawnPos, 0.7f);
    }
}
