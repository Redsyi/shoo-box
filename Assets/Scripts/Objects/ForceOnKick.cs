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
    public bool addRandomTorque;

    public void OnKick(GameObject kicker)
    {
        if (kickEnabled)
        {
            if (rigidbody.isKinematic)
                rigidbody.isKinematic = false;
            rigidbody.AddForce(((transform.position - kicker.transform.position).normalized + additionalForceVector) * force);
            if (addRandomTorque)
            {
                rigidbody.AddTorque(new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f), Random.Range(-20f, 20f)));
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponentInChildren<Rigidbody>();
    }
}
