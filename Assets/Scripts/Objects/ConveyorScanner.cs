using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConveyorScanner : MonoBehaviour
{
    public float playerCatchWaitTime;
    private Player player;
    private bool caught;
    private Light[] warningLights;
    public float minLightIntensity;
    public float maxLightIntensity;
    public float lightFluxSpeed = 1f;
    private float alarmTriggerTime;
    bool doFalseAlarm;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
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

    IEnumerator CatchPlayer()
    {
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
            remainingWaitTime -= Time.deltaTime;
            ConveyorBelt.active = false;
            yield return null;
        }
        LevelBridge.Reload("You were scanned by TSA");
    }

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
}
