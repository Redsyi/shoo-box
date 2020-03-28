using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHidePoint : MonoBehaviour
{
    public Image pointer;
    public Transform target;
    public Vector2 offset;
    public Vector2 margins;
    private Vector2 dimensions;
    private float baseAngle;

    private void Start()
    {
        StartCoroutine(FindDimensions());
        baseAngle = pointer.transform.localEulerAngles.z;
    }

    IEnumerator FindDimensions()
    {
        yield return null;
        dimensions = new Vector2(CameraScript.current.camera.pixelWidth, CameraScript.current.camera.pixelHeight);
    }

    private void LateUpdate()
    {
        Vector3 screenTarget = CameraScript.current.camera.WorldToScreenPoint(target.position);
        Vector3 newPoint = screenTarget + (Vector3)offset;
        if (newPoint.x < margins.x)
            newPoint.x = margins.x;
        else if (newPoint.x > dimensions.x - margins.x)
            newPoint.x = dimensions.x - margins.x;
        if (newPoint.y < margins.y)
            newPoint.y = margins.y;
        else if (newPoint.y > dimensions.y - margins.y)
            newPoint.y = dimensions.y - margins.y;
        transform.position = newPoint;

        Vector2 vectToTarget = new Vector2(screenTarget.x - transform.position.x, screenTarget.y - transform.position.y);
        float angle = Utilities.VectorToDegrees(vectToTarget);
        pointer.transform.localEulerAngles = new Vector3(0, 0, baseAngle + angle);
    }
}
