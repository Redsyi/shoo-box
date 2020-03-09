using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealFocusWhenSeen : MonoBehaviour
{
    private bool focusStolen;
    public float padding;
    public static StealFocusWhenSeen activeThief;
    public float cameraSize;
    public float cameraScrollSpeed;
    public float cameraZoomSpeed;
    public float cameraStealTime;

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
            if (screenPos.x >= padding && screenPos.x <= camera.pixelWidth - padding && screenPos.y >= padding && screenPos.y <= camera.pixelHeight - padding)
            {
                StartCoroutine(StealFocus());
            }
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

        //part 1: translate camera to current position
        while ((vectToDest).sqrMagnitude > 0.3f)
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
        while (focusTimeLeft > 0)
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
        vectToDest = player.transform.position - CameraScript.current.transform.position;
        while ((vectToDest).sqrMagnitude > 0.3f)
        {
            CameraScript.current.transform.position += vectToDest.normalized * Time.deltaTime * (1 / cameraScrollSpeed) * dist;

            //zoom camera if necessary
            if (camera.orthographicSize < originalZoomLevel)
                camera.orthographicSize = Mathf.Min(originalZoomLevel, camera.orthographicSize + Time.deltaTime * (1 / cameraZoomSpeed) * zoomDiff);
            else if (camera.orthographicSize > originalZoomLevel)
                camera.orthographicSize = Mathf.Max(originalZoomLevel, camera.orthographicSize - Time.deltaTime * (1 / cameraZoomSpeed) * zoomDiff);
            
            yield return null;
            vectToDest = player.transform.position - CameraScript.current.transform.position;
        }
        activeThief = null;
    }
}
