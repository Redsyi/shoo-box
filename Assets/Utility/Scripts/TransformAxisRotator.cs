using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class to allow fine-tuned animation control over a transform's rotation
/// </summary>
public class TransformAxisRotator : MonoBehaviour
{
    public bool affectXAxis;
    public bool affectYAxis;
    public bool affectZAxis;
    public float xAxisAngle;
    public float yAxisAngle;
    public float zAxisAngle;
    private Vector3 rotation;

    void Update()
    {
        rotation.x = (affectXAxis ? xAxisAngle : transform.localEulerAngles.x);
        rotation.y = (affectYAxis ? yAxisAngle : transform.localEulerAngles.y);
        rotation.z = (affectZAxis ? zAxisAngle : transform.localEulerAngles.z);
        transform.localEulerAngles = rotation;
    }
}
