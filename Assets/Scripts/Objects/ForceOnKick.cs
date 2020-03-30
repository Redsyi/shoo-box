using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ForceOnKick : MonoBehaviour, IKickable
{
    public float force;
    private Rigidbody rigidbody;
    public bool kickEnabled = true;
    public Vector3 additionalForceVector;

    public void OnKick(GameObject kicker)
    {
        if (kickEnabled)
            rigidbody.AddForce(((transform.position - kicker.transform.position).normalized + additionalForceVector) * force);
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponentInChildren<Rigidbody>();
    }
}
