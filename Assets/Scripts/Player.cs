using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Rigidbody rigidbody;
    public Animator animator;
    public float speed;
    public float walkRotation;

    private Vector3 currentMovement;
    private float currRotation;

    public void OnMove(InputValue value)
    {
        Vector2 movement = Utilities.RotateVectorDegrees(value.Get<Vector2>().normalized * speed, 135);
        currRotation = -Utilities.VectorToDegrees(movement);
        transform.eulerAngles = new Vector3(walkRotation, currRotation, transform.eulerAngles.z);

        currentMovement.x = movement.x;
        currentMovement.z = movement.y;

        animator.SetBool("walking", movement.sqrMagnitude > 0);
    }

    private void FixedUpdate()
    {
        transform.position += currentMovement;
    }
}
