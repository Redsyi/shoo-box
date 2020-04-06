using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CameraAngleZone : MonoBehaviour
{
    public float cameraAngle;
    private float originalCameraAngle;
    private float timeInBounds;
    private float timeOutOfBounds;
    private bool inBounds => timeInBounds > 0;
    public float timeToAdjust = 1f;

    private void Start()
    {
        originalCameraAngle = CameraScript.current.cameraAngle;
    }

    private void OnTriggerStay(Collider other)
    {
        if (StealFocusWhenSeen.activeThief == null)
            timeInBounds += Time.deltaTime;
        timeOutOfBounds = 0f;
    }

    private void OnTriggerExit(Collider other)
    {
        timeOutOfBounds = Mathf.Clamp01(timeToAdjust - timeInBounds);
        timeInBounds = 0f;
    }

    private void Update()
    {
        if (StealFocusWhenSeen.activeThief == null)
        {
            timeOutOfBounds += Time.deltaTime;
            if (inBounds && CameraScript.current.cameraAngle != cameraAngle)
            {
                CameraScript.current.cameraAngle = Mathf.LerpAngle(originalCameraAngle, cameraAngle, Mathf.Clamp01(timeInBounds / timeToAdjust));
            } else if (!inBounds && CameraScript.current.cameraAngle != originalCameraAngle)
            {
                CameraScript.current.cameraAngle = Mathf.LerpAngle(cameraAngle, originalCameraAngle, Mathf.Clamp01(timeOutOfBounds / timeToAdjust));
            }
        }
    }
}
