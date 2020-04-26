﻿using System.Collections;
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
    float oobTimeout = 1f;

    private void Start()
    {
        originalCameraAngle = CameraScript.current.cameraAngle;
    }

    private void OnTriggerStay(Collider other)
    {
        oobTimeout = 0f;
        if (StealFocusWhenSeen.activeThief == null)
            timeInBounds += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
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
                timeOutOfBounds = Mathf.Clamp01(timeToAdjust - timeInBounds);
                timeInBounds = 0f;
            }
        }

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
