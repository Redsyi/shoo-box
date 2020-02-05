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
    public Detector detector;
    public PlayerInput inputSystem;
    public float footstepTiming;
    public float footstepSoundOffset;

    public Vector3 currentMovement;
    private float currRotation;
    public bool legForm;
    private CameraScript myCamera;
    private Shoe currShoe;
    private float rumbleTime;
    private float currBoxSpeed;
    public float boxSlideSlowdownRate;
    

    private void Start()
    {
        myCamera = FindObjectOfType<CameraScript>();
        ClearRumble();
    }

    /// <summary>
    /// Sets the movement vector for the player
    /// </summary>
    public void OnMove(InputValue value)
    {
        Vector2 movement = Utilities.RotateVectorDegrees(value.Get<Vector2>().normalized * speed * (legForm ? 1: currBoxSpeed), 135 - myCamera.transform.eulerAngles.y);
        
        if(movement.sqrMagnitude != 0)
        {
            if (currentMovement == Vector3.zero)
                StartCoroutine(DoFootsteps());
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

    private void Update()
    {
        UpdateRumble();
        if (!legForm) Slide();
    }

    //slows down current box speed multiplier until it reaches the base multiplier
    private void Slide()
    {
        currBoxSpeed = Mathf.Max(boxSpeedMultiplier, currBoxSpeed - Time.deltaTime * boxSlideSlowdownRate * (1-boxSpeedMultiplier));
    }

    /// <summary>
    /// Toggle between box and leg form
    /// </summary>
    public void OnChangeForm(InputValue value)
    {
        if(!legForm)
        {
            if (Physics.Raycast(transform.position, Vector3.up, 1, LayerMask.GetMask("Obstacle")))
                return;
        }

        legForm = !legForm;

        model.transform.localPosition = new Vector3(model.transform.localPosition.x, (legForm ? 0: -.65f), model.transform.localPosition.z);
        hitBox.transform.localPosition = new Vector3(hitBox.transform.localPosition.x, (legForm ? .5f : .2f), hitBox.transform.localPosition.z);
        hitBox.size = new Vector3(hitBox.size.x, (legForm ? 1 : .3f), hitBox.size.z);

        currBoxSpeed = 1;
    }

    /// <summary>
    /// Rotate camera 90 degrees around player
    /// </summary>
    public void OnRotate(InputValue value)
    {
        CameraScript.RotationDirection direction = (value.Get<float>() > 0 ? CameraScript.RotationDirection.CLOCKWISE : CameraScript.RotationDirection.COUNTERCLOCKWISE);
        myCamera.Rotate(direction);
    }

    /// <summary>
    /// Interact button pressed
    /// </summary>
    public void OnInteract(InputValue value)
    { 

        if (detector.currentItem && legForm)
        {
            UI_Inputs ui = FindObjectOfType<UI_Inputs>();
            if (ui)
                ui.WearShoes();
            detector.currentItem.GetComponent<Collider>().enabled = false;
            detector.currentItem.transform.parent.SetParent(model.transform, false);
            detector.currentItem.transform.parent.localPosition = Vector3.zero;
            currShoe = detector.currentItem.GetComponentInParent<Shoe>();
            detector.currentItem = null;
        }

    }

    /// <summary>
    /// Action button pressed
    /// </summary>
    public void OnAction()
    {
        if (legForm && currShoe)
        {
            switch(currShoe.shoeType)
            {
                case ShoeType.BOOTS:
                    animator.SetTrigger("Kick");
                    currShoe.GetComponentInChildren<Kicker>().Kick();
                    break;
                default:
                    Debug.Log("you dun fucked up boi");
                    break;
            }
        }
    }

    //tell UI to pause or unpause
    public void OnPauseMenu(InputValue value)
    {
        FindObjectOfType<UI_Inputs>().OnPauseMenu(value);
    }

    /// <summary>
    /// Rumbles the player controller (if applicable)
    /// </summary>
    /// <param name="strength">rumble strength</param>
    /// <param name="time">how long to rumble for</param>
    public void Rumble(RumbleStrength strength, float time)
    {
        if (inputSystem.currentControlScheme == "Gamepad")
        {
            float lowFreqSpeed = 0f;
            float highFreqSpeed = 0f;
            switch (strength)
            {
                case RumbleStrength.WEAK:
                    lowFreqSpeed = 0.25f;
                    break;
                case RumbleStrength.MEDIUM:
                    lowFreqSpeed = 0.35f;
                    highFreqSpeed = 0.65f;
                    break;
                case RumbleStrength.INTENSE:
                    lowFreqSpeed = 1f;
                    highFreqSpeed = 1f;
                    break;
            }
            try
            {
                (inputSystem.devices[0] as Gamepad).SetMotorSpeeds(lowFreqSpeed, highFreqSpeed);
                rumbleTime = Mathf.Max(rumbleTime, time);
            }
            catch
            {
                print("tried to rumble something that wasn't a controller");
            }
        }
    }

    //automatically turns off controller rumble once time is up
    private void UpdateRumble() {
        if (inputSystem?.currentControlScheme == "Gamepad")
        {
            if (rumbleTime > 0f)
            {
                rumbleTime -= Time.unscaledDeltaTime;
                if (rumbleTime <= 0f)
                {
                    (inputSystem.devices[0] as Gamepad).SetMotorSpeeds(0f, 0f);
                }
            }
        }
    }

    //cancels any rumbling in the controller
    private void ClearRumble() {
        if (inputSystem?.currentControlScheme == "Gamepad")
        {
            (inputSystem.devices[0] as Gamepad).SetMotorSpeeds(0f, 0f);
            rumbleTime = 0f;
        }
    }

    //makes a footstep noise every footstepTiming seconds, auto cancels once movement stops
    IEnumerator DoFootsteps()
    {
        if (footstepSoundOffset > 0f)
            yield return new WaitForSeconds(footstepSoundOffset);
        float timeSinceLast = footstepTiming;
        while (currentMovement != Vector3.zero)
        {
            timeSinceLast += Time.deltaTime;
            if (legForm && timeSinceLast >= footstepTiming)
            {
                AudioManager.MakeNoise(transform.position, 1.3f, null, 1);
                timeSinceLast = 0f;
            }
            yield return null;
        }
    }
}
