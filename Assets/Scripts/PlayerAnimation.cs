using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Temporary class until we have "real" animations
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    public float walkRotation;
    public float kickRotation;

    void Update()
    {
        transform.localEulerAngles = new Vector3(walkRotation, transform.localEulerAngles.y, kickRotation);
    }
}
