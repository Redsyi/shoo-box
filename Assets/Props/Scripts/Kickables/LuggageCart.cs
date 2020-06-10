using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// toggles between two positions
/// </summary>
public class LuggageCart : MonoBehaviour, IKickable
{
    public Vector3 move;
    public float speed;
    public float dist;
    public LuggageJostle jostle;
    private float distMoved;
    private bool kicked = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + move * dist);
    }

    public void OnKick(GameObject kicker)
    {
        kicked = true;
    }

    /*
     * Move's this object's PARENT 
     */
    private void FixedUpdate()
    {
        if (kicked && distMoved < dist)
        {
            distMoved += speed * Time.fixedDeltaTime;
            transform.parent.position += (speed * Time.fixedDeltaTime * move); // Assumes you want to move its parent
        }
        else if (distMoved >= dist)
        {
            distMoved = 0;
            kicked = false;
            jostle.SwitchJostles();
        }
    }

}
