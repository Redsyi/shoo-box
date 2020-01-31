using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestKickable : MonoBehaviour, IKickable
{
    public Rigidbody myRigidbody;
    public float kickForce;

    public void OnKick(GameObject kicker)
    {
        Debug.Log("I was kicked :(");
        myRigidbody.AddForce((transform.position - kicker.transform.position).normalized * kickForce);
    }
}
