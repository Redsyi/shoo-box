using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConveyorScanner : MonoBehaviour
{
    public Vector3 playerCatchPosition;
    public float playerCatchWaitTime;
    private Player player;
    private bool caught;
    private Light[] warningLights;
    public float minLightIntensity;
    public float maxLightIntensity;
    public float lightFluxSpeed = 1f;
    private float alarmTriggerTime;
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
        while (remainingWaitTime > 0)
        {
            float lightIntensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, (Mathf.Sin((Time.time - alarmTriggerTime) * lightFluxSpeed) + 1) / 2f);
            foreach (Light light in warningLights)
            {
                light.intensity = lightIntensity;
            }
            remainingWaitTime -= Time.deltaTime;
            player.transform.position = transform.position + playerCatchPosition;
            ConveyorBelt.active = false;
            yield return null;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + playerCatchPosition, 0.25f);
    }
}
