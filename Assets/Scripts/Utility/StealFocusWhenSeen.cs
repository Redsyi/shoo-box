﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealFocusWhenSeen : MonoBehaviour
{
    private bool focusStolen;
    [Tooltip("How far from the edge of the screen until this object is considered \"seen\"")]
    public float padding;
    public static StealFocusWhenSeen activeThief;
    [Tooltip("Target zoom level")]
    public float cameraSize;
    [Tooltip("Time (in seconds) for camera to reach here")]
    public float cameraScrollSpeed;
    [Tooltip("Time (in seconds) that camera lingers")]
    public float cameraStealTime;
    [Tooltip("Custom methods to run when this camera steal triggers")]
    public UnityEngine.Events.UnityEvent onSteal;
    [Tooltip("Custom methods to run when this camera steal ends")]
    public UnityEngine.Events.UnityEvent onStealEnd;
    [Tooltip("Freeze time while this steal is active?")]
    public bool freezeTime;
    [Tooltip("Automatically steal focus after this many seconds has passed, -1 for no time-based steal")]
    public float stealAfterTime = -1;
    [Tooltip("Should this stealer lock player movement?")]
    public bool lockMovement = true;
    [Tooltip("Destination camera rotation around the Y axis")]
    public float destCameraYRotation;
    [Tooltip("Destination camera rotation around the X axis")]
    public float destCameraAngle;
    [Tooltip("How close the camera should be to the source. Default is 20, move closer if there's something in the way")]
    public float destCameraDist = 20f;
    private bool skip;

    private void Start()
    {
        activeThief = null;
    }
    void Update()
    {
        if (!focusStolen && activeThief == null)
        {
            Camera camera = CameraScript.current.camera;
            Vector3 screenPos = camera.WorldToScreenPoint(transform.position);
            if ((stealAfterTime != -1 && stealAfterTime <= 0f) || (screenPos.x >= padding && screenPos.x <= camera.pixelWidth - padding && screenPos.y >= padding && screenPos.y <= camera.pixelHeight - padding))
            {
                StartCoroutine(StealFocus());
            }

            if (stealAfterTime != -1 && stealAfterTime > 0)
                stealAfterTime -= Time.deltaTime;
        }
    }

    public void Trigger()
    {
        if (!focusStolen && activeThief == null)
        {
            StartCoroutine(StealFocus());
        }
    }

    public void Skip()
    {
        if (focusStolen)
        {
            skip = true;
        }
    }

    //sorry for how messy this looks
    IEnumerator StealFocus()
    {
        focusStolen = true;
        activeThief = this;
        CameraScript cameraScript = CameraScript.current;
        Camera camera = cameraScript.camera;
        float originalZoomLevel = camera.orthographicSize;
        if (freezeTime)
            Time.timeScale = 0f;
        float panProgress = 0f;
        float timePassed = 0f;
        Vector3 originalPosition = cameraScript.transform.position;
        float originalCameraRotation = cameraScript.cameraRotation;
        float originalCameraAngle = cameraScript.cameraAngle;
        float originalCameraDist = cameraScript.cameraDist;

        onSteal.Invoke();

        //part 1: translate camera to current position
        while (panProgress < 1 && !skip)
        {
            cameraScript.transform.position = Vector3.Lerp(originalPosition, transform.position, panProgress);
            camera.orthographicSize = Mathf.Lerp(originalZoomLevel, cameraSize, panProgress);
            cameraScript.cameraRotation = Mathf.LerpAngle(originalCameraRotation, destCameraYRotation, panProgress);
            cameraScript.cameraAngle = Mathf.LerpAngle(originalCameraAngle, destCameraAngle, panProgress);
            cameraScript.cameraDist = Mathf.LerpAngle(originalCameraDist, destCameraDist, panProgress);

            yield return null;
            panProgress = Mathf.Clamp01(timePassed / cameraScrollSpeed);
            timePassed += Time.unscaledDeltaTime;
        }

        //part 2: remain focused on current position
        float focusTimeLeft = cameraStealTime;
        while (focusTimeLeft > 0 && !skip)
        {
            camera.orthographicSize = cameraSize;
            CameraScript.current.transform.position = transform.position;
            focusTimeLeft -= Time.unscaledDeltaTime;
            yield return null;
        }

        //part 3: go back to player
        Player player = FindObjectOfType<Player>();
        float finalZoomLevel = (player.legForm ? cameraScript.farZoomLevel : cameraScript.closeZoomLevel);
        panProgress = 1 - panProgress;
        timePassed = cameraScrollSpeed - timePassed;
        while (panProgress < 1)
        {
            cameraScript.transform.position = Vector3.Lerp(transform.position, player.transform.position, panProgress);
            camera.orthographicSize = Mathf.Lerp(cameraSize, finalZoomLevel, panProgress);
            cameraScript.cameraRotation = Mathf.LerpAngle(destCameraYRotation, originalCameraRotation, panProgress);
            cameraScript.cameraAngle = Mathf.LerpAngle(destCameraAngle, originalCameraAngle, panProgress);
            cameraScript.cameraDist = Mathf.LerpAngle(destCameraDist, originalCameraDist, panProgress);

            yield return null;
            panProgress = Mathf.Clamp01(timePassed / cameraScrollSpeed);
            timePassed += Time.unscaledDeltaTime;
        }

        cameraScript.cameraRotation = originalCameraRotation;
        cameraScript.cameraAngle = originalCameraAngle;
        cameraScript.cameraDist = originalCameraDist;
        Time.timeScale = 1;
        onStealEnd.Invoke();
        activeThief = null;
    }

    public static void SkipActive()
    {
        foreach (StealFocusWhenSeen instance in FindObjectsOfType<StealFocusWhenSeen>())
        {
            instance.Skip();
        }
    }
}
