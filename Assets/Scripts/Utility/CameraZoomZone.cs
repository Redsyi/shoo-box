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

    private void Start()
    {
        active = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (StealFocusWhenSeen.activeThief == null)
            timeInBounds += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        active = true;
        timeInBounds = Mathf.Clamp01(timeToAdjust - timeOutOfBounds);
        timeOutOfBounds = 0f;
    }

    private void OnTriggerExit(Collider other)
    {
        active = false;
        timeOutOfBounds = Mathf.Clamp01(timeToAdjust - timeInBounds);
        timeInBounds = 0f;
    }

    private void Update()
    {
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
