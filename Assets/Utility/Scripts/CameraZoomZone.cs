using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that overrides the camera zoom level when the player is inside it (combine with a trigger collider on the PlayerCatcher layer)
/// </summary>
[RequireComponent(typeof(Collider))]
public class CameraZoomZone : MonoBehaviour
{
    public float cameraZoom;
    private float originalCameraZoom => Player.current.legForm ? CameraScript.current.farZoomLevel : CameraScript.current.closeZoomLevel;
    private float timeInBounds;
    private float timeOutOfBounds;
    private bool inBounds => timeInBounds > 0;
    public float timeToAdjust = 1f;
    public static bool active;
    float oobTimeout = 1f;

    private void Start()
    {
        active = false;
        timeOutOfBounds = timeToAdjust;
    }

    private void OnTriggerStay(Collider other)
    {
        oobTimeout = 0;
        if (StealFocusWhenSeen.activeThief == null)
            timeInBounds += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        active = true;
        if (timeInBounds < 0.1f)
            timeInBounds = Mathf.Clamp(timeToAdjust - timeOutOfBounds, 0, timeToAdjust);
        timeOutOfBounds = 0f;
    }

    private void Update()
    {
        //this bit of code is essentially OnTriggerExit. we can't use OnTriggerExit directly because then we get jerky movement
        //when we toggle forms or kick, doing it this way ensures the game has time to transition hitboxes
        if (oobTimeout < 0.12f)
        {
            oobTimeout += Time.deltaTime;
            if (oobTimeout > 0.12f)
            {
                active = false;
                timeOutOfBounds = Mathf.Clamp(timeToAdjust - timeInBounds, 0, timeToAdjust);
                timeInBounds = 0f;
            }
        }

        if (StealFocusWhenSeen.activeThief == null && !CameraScript.current.cinematicMode)
        {
            if (inBounds && CameraScript.current.camera.orthographicSize != cameraZoom)
            {
                CameraScript.current.camera.orthographicSize = Mathf.Lerp(originalCameraZoom, cameraZoom, Mathf.Clamp01(timeInBounds / timeToAdjust));
            } else if (!inBounds && CameraScript.current.camera.orthographicSize != originalCameraZoom && timeOutOfBounds < timeToAdjust)
            {
                CameraScript.current.camera.orthographicSize = Mathf.Lerp(cameraZoom, originalCameraZoom, Mathf.Clamp01(timeOutOfBounds / timeToAdjust));
            }
            timeOutOfBounds += Time.deltaTime;
        }
    }
}
