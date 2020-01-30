using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public float walkRotation;

    void Update()
    {
        transform.localEulerAngles = new Vector3(walkRotation, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }
}
