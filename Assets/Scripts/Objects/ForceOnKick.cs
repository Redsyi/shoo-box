using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceOnKick : MonoBehaviour, IKickable
{
    public float force;
    private Rigidbody rigidbody;

    public void OnKick(GameObject kicker)
    {
        rigidbody.AddForce((transform.position - kicker.transform.position).normalized * force);
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponentInChildren<Rigidbody>();
        if (rigidbody == null)
            Debug.LogError("ForceOnKick component cannot find rigidbody");
    }
}
