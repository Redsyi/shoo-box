using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            timeInBounds = Mathf.Clamp01(timeToAdjust - timeOutOfBounds);
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
                timeOutOfBounds = Mathf.Clamp01(timeToAdjust - timeInBounds);
                timeInBounds = 0f;
            }
        }

        if (StealFocusWhenSeen.activeThief == null)
        {
            timeOutOfBounds += Time.deltaTime;
            if (inBounds && CameraScript.current.camera.orthographicSize != cameraZoom)
            {
                CameraScript.current.camera.orthographicSize = Mathf.Lerp(originalCameraZoom, cameraZoom, Mathf.Clamp01(timeInBounds / timeToAdjust));
            } else if (!inBounds && CameraScript.current.camera.orthographicSize != originalCameraZoom)
            {
                CameraScript.current.camera.orthographicSize = Mathf.Lerp(cameraZoom, originalCameraZoom, Mathf.Clamp01(timeOutOfBounds / timeToAdjust));
            }
        }
    }
}
