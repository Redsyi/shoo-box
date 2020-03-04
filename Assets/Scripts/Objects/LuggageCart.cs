using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuggageCart : MonoBehaviour, IKickable
{
    public Vector3 move;
    public float speed;
    public float dist;
    private float distMoved;
    private bool kicked;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + move * dist);
    }

    public void OnKick(GameObject kicker)
    {
        kicked = true;
    }

    private void FixedUpdate()
    {
        if (kicked && distMoved < dist)
        {
            distMoved += speed * Time.fixedDeltaTime;
            transform.position += (speed * Time.fixedDeltaTime * move);
        }
    }

}
