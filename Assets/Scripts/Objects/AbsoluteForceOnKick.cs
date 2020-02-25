using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsoluteForceOnKick : MonoBehaviour, IKickable
{
    public Vector3 force;
    private Rigidbody rigidbody;

    public void OnKick(GameObject kicker)
    {
        rigidbody.AddRelativeForce(force);
    }

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
}
