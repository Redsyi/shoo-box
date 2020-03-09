using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteObjectiveWhenSeen : MonoBehaviour
{
    private bool complete;
    public int objectiveNum;
    public float padding;
    void Update()
    {
        if (!complete)
        {
            Camera camera = CameraScript.current.camera;
            Vector3 screenPos = camera.WorldToScreenPoint(transform.position);
            if (screenPos.x >= padding && screenPos.x <= camera.pixelWidth - padding && screenPos.y >= padding && screenPos.y <= camera.pixelHeight - padding)
            {
                complete = true;
                ObjectiveTracker.instance.CompleteObjective(objectiveNum);
            }
        }
    }
}
