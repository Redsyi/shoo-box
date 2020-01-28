using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Rigidbody rigidbody;
    public float speed;

    private Vector3 currentMovement;
    private float currRotation;

    public void OnMove(InputValue value)
    {
        Vector2 movement = Utilities.RotateVectorDegrees(value.Get<Vector2>().normalized * speed, 135);
        currRotation = -Utilities.VectorToDegrees(movement);
        transform.eulerAngles = new Vector3(0, currRotation, 0);

        currentMovement.x = movement.x;
        currentMovement.z = movement.y;
    }

    private void FixedUpdate()
    {
        transform.position += currentMovement;
    }
}
