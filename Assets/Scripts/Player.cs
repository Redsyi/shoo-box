using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Rigidbody rigidbody;
    public Animator animator;
    public float speed;
    public GameObject model;
    public BoxCollider hitBox;
    public float boxSpeedMultiplier;
    public Collider succArea;

    private Vector3 currentMovement;
    private float currRotation;
    private bool legForm;
    private CameraScript myCamera;
    

    private void Start()
    {
        myCamera = FindObjectOfType<CameraScript>();
        //legForm = false;
    }

    public void OnMove(InputValue value)
    {
        Vector2 movement = Utilities.RotateVectorDegrees(value.Get<Vector2>().normalized * speed * (legForm ? 1: boxSpeedMultiplier), 135 - myCamera.transform.eulerAngles.y);
        
        if(movement.sqrMagnitude != 0)
        {
            currRotation = -Utilities.VectorToDegrees(movement);
        }
        currentMovement.x = movement.x;
        currentMovement.z = movement.y;

        animator.SetBool("walking", movement.sqrMagnitude > 0);
    }

    private void FixedUpdate()
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, currRotation, transform.localEulerAngles.z);
        transform.position += currentMovement;
    }

    public void OnChangeForm(InputValue value)
    {
        if(!legForm)
        {
            if (Physics.Raycast(transform.position, Vector3.up, 1, LayerMask.GetMask("Obstacle")))
                return;
        }

        legForm = !legForm;
        print("Legform = " + legForm);

        model.transform.localPosition = new Vector3(model.transform.localPosition.x, (legForm ? 0: -.65f), model.transform.localPosition.z);
        hitBox.transform.localPosition = new Vector3(hitBox.transform.localPosition.x, (legForm ? .5f : .2f), hitBox.transform.localPosition.z);
        hitBox.size = new Vector3(hitBox.size.x, (legForm ? 1 : .3f), hitBox.size.z);

    }

    public void OnRotate(InputValue value)
    {
        CameraScript.RotationDirection direction = (value.Get<float>() > 0 ? CameraScript.RotationDirection.COUNTERCLOCKWISE : CameraScript.RotationDirection.CLOCKWISE);
        myCamera.Rotate(direction);
    }

}
