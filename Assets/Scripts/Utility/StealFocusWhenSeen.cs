using System.Collections;
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
    [Tooltip("Time (in seconds) for camera to zoom in")]
    public float cameraZoomSpeed;
    [Tooltip("Time (in seconds) that camera lingers")]
    public float cameraStealTime;
    [Tooltip("Custom methods to run when this camera steal triggers")]
    public UnityEngine.Events.UnityEvent onSteal;
    [Tooltip("Custom methods to run when this camera steal ends")]
    public UnityEngine.Events.UnityEvent onStealEnd;
    [Tooltip("(NOT YET IMPLEMENTED) Freeze time while this steal is active?")]
    public bool freezeTime;
    [Tooltip("Automatically steal focus after this many seconds has passed, -1 for no time-based steal")]
    public float stealAfterTime = -1;
    [Tooltip("Should this stealer lock player movement?")]
    public bool lockMovement = true;
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
        }
        if (stealAfterTime != -1 && stealAfterTime > 0)
            stealAfterTime -= Time.deltaTime;
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
        Camera camera = CameraScript.current.camera;
        float originalZoomLevel = camera.orthographicSize;
        Vector3 vectToDest = transform.position - CameraScript.current.transform.position;
        float dist = vectToDest.magnitude;
        float zoomDiff = Mathf.Abs(cameraSize - originalZoomLevel);

        onSteal.Invoke();

        //part 1: translate camera to current position
        while ((vectToDest).sqrMagnitude > 0.4f && !skip)
        {
            CameraScript.current.transform.position += vectToDest.normalized * Time.deltaTime * (1/cameraScrollSpeed) * dist;
            
            //zoom camera if necessary
            if (camera.orthographicSize < cameraSize)
                camera.orthographicSize = Mathf.Min(cameraSize, camera.orthographicSize + Time.deltaTime * (1/cameraZoomSpeed) * zoomDiff);
            else if (camera.orthographicSize > cameraSize)
                camera.orthographicSize = Mathf.Max(cameraSize, camera.orthographicSize - Time.deltaTime * (1/cameraZoomSpeed) * zoomDiff);
            
            yield return null;
            vectToDest = transform.position - CameraScript.current.transform.position;
        }

        //part 2: remain focused on current position
        float focusTimeLeft = cameraStealTime;
        while (focusTimeLeft > 0 && !skip)
        {
            CameraScript.current.transform.position = transform.position;

            //zoom camera if necessary
            if (camera.orthographicSize < cameraSize)
                camera.orthographicSize = Mathf.Min(cameraSize, camera.orthographicSize + Time.deltaTime * (1 / cameraZoomSpeed) * zoomDiff);
            else if (camera.orthographicSize > cameraSize)
                camera.orthographicSize = Mathf.Max(cameraSize, camera.orthographicSize - Time.deltaTime * (1 / cameraZoomSpeed) * zoomDiff);
            
            focusTimeLeft -= Time.deltaTime;
            yield return null;
        }

        //part 3: go back to player
        Player player = FindObjectOfType<Player>();
        float finalZoomLevel = (player.legForm ? CameraScript.current.farZoomLevel : CameraScript.current.closeZoomLevel);
        zoomDiff = Mathf.Abs(finalZoomLevel - cameraSize);
        vectToDest = player.transform.position - CameraScript.current.transform.position;
        while ((vectToDest).sqrMagnitude > 0.4f)
        {
            CameraScript.current.transform.position += vectToDest.normalized * Time.deltaTime * (1 / cameraScrollSpeed) * dist;

            //zoom camera if necessary
            if (camera.orthographicSize < finalZoomLevel)
                camera.orthographicSize = Mathf.Min(finalZoomLevel, camera.orthographicSize + Time.deltaTime * (1 / cameraZoomSpeed) * zoomDiff);
            else if (camera.orthographicSize > finalZoomLevel)
                camera.orthographicSize = Mathf.Max(finalZoomLevel, camera.orthographicSize - Time.deltaTime * (1 / cameraZoomSpeed) * zoomDiff);
            
            yield return null;
            vectToDest = player.transform.position - CameraScript.current.transform.position;
        }

        //finish any leftover zoom
        while (camera.orthographicSize != finalZoomLevel)
        {
            if (camera.orthographicSize < finalZoomLevel)
                camera.orthographicSize = Mathf.Min(finalZoomLevel, camera.orthographicSize + Time.deltaTime * (1 / cameraZoomSpeed) * zoomDiff);
            else if (camera.orthographicSize > finalZoomLevel)
                camera.orthographicSize = Mathf.Max(finalZoomLevel, camera.orthographicSize - Time.deltaTime * (1 / cameraZoomSpeed) * zoomDiff);
            yield return null;
        }
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
