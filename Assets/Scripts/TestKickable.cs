using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestKickable : MonoBehaviour, IKickable, IAIInteractable
{
    public Rigidbody myRigidbody;
    public float kickForce;
    public bool broken;
    public float fixtime;
    private Vector3 orignalPos;

    private void Start()
    {
        orignalPos = transform.position;
    }

    public void AIInteract()
    {
        broken = false;
        transform.position = orignalPos;
    }

    public float AIInteractTime()
    {
        return fixtime;
    }

    public void OnKick(GameObject kicker)
    {
        myRigidbody.AddForce((transform.position - kicker.transform.position).normalized * kickForce);
        broken = true;
        AudioManager.MakeNoise(transform.position, 1.5f, null, 1);
    }

    public bool NeedsInteraction()
    {
        return broken;
    }
}
