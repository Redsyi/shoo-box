using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorGate : MonoBehaviour
{
    public Vector3 closedPos;
    public Vector3 openPos;
    public float speed = 1f;
    private Vector3 originalPos;

    private void Start()
    {
        originalPos = transform.position;
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, originalPos + (ConveyorBelt.active ? closedPos : openPos), speed * Time.fixedDeltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + closedPos, transform.position + openPos);
    }
}
