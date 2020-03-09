using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Component References")]
    public Rigidbody rigidbody;
    public Animator animator;
    public GameObject model;
    public BoxCollider legformHitbox;
    public BoxCollider boxformHitbox;
    public ShoeSniffer shoeSniffer;
    public PlayerShoeManager shoeManager;
    public PlayerInput inputSystem;
    public ParticleSystem walkingParticleSystem;
    public Transform AISpotPoint;
    public GameObject leggs;

    [Header("Stats")]
    public ShoeType[] startingShoes;
    public float baseSpeed;
    public float boxSpeedMultiplier;
    public float boxSlideSlowdownRate;
    public float rotationSpeed;
    public bool legForm;

    [Header("Footsteps")]
    public float footstepTiming;
    public float footstepSoundOffset;
    public bool moving => currMovementInput != Vector2.zero;
    private bool makingFootsteps;
    public AK.Wwise.Event onStep;

    [Header("Effects")]
    public Transform legParticlesPosition;
    public Transform boxParticlesPosition;
    public int walkingParticleEmissionRate;
    public int shuffleParticleEmissionRate;

    private Vector2 currMovementInput;
    private float currRotation;
    private CameraScript myCamera;
    //private Shoe currShoe;
    private float rumbleTime;
    private float currBoxSpeed;
    private UIShoeTag shoeTagUI;
    [HideInInspector]
    public float verticalBoost;
    public bool usingController => inputSystem.currentControlScheme.Equals("Gamepad");


    private void Start()
    {
        myCamera = FindObjectOfType<CameraScript>();
        if (myCamera == null)
            Debug.LogError("Player couldn't find camera script");
        ClearRumble();
        currBoxSpeed = boxSpeedMultiplier;
        shoeTagUI = FindObjectOfType<UIShoeTag>();
        if (shoeTagUI == null)
            Debug.LogError("Player couldn't find shoe tag UI");

        foreach (ShoeType shoeType in startingShoes)
        {
            shoeManager.Acquire(shoeType);
            EquipShoe(shoeType);
        }
    }

    /// <summary>
    /// Sets the movement vector for the player
    /// </summary>
    public void OnMove(InputValue value)
    {
        bool wasMoving = (currMovementInput == Vector2.zero);
        currMovementInput = value.Get<Vector2>();
        if (currMovementInput != Vector2.zero && !wasMoving && !makingFootsteps)
            StartCoroutine(DoFootsteps());

        if (currMovementInput.sqrMagnitude > 1)
            currMovementInput = currMovementInput.normalized;
    }

    private void FixedUpdate()
    {
        if (StealFocusWhenSeen.activeThief == null)
        {
            rigidbody.velocity = CalculateMovementVector();
            if (verticalBoost != 0 && rigidbody.velocity != Vector3.zero)
            {
                transform.Translate(Vector3.up * verticalBoost);
            }
        }
    }

    private Vector3 CalculateMovementVector()
    {
        Vector2 adjustedMovement = Utilities.RotateVectorDegrees(currMovementInput * baseSpeed * (legForm ? 1 : currBoxSpeed) * Time.fixedDeltaTime, 135 - myCamera.transform.eulerAngles.y);
        return new Vector3(adjustedMovement.x, rigidbody.velocity.y, adjustedMovement.y);
    }

    private void Update()
    {
        UpdateRumble();
        if (!legForm) Slide();
        InterpolateRotation();
        UpdateAnimator();
        UpdateParticles();

        if (shoeSniffer.detectedShoe && legForm)
        {
            shoeManager.Acquire(shoeSniffer.detectedShoe.shoeType);
            EquipShoe(shoeSniffer.detectedShoe.shoeType);
            Destroy(shoeSniffer.detectedShoe.gameObject);
        }
    }

    private void UpdateAnimator()
    {
        if (moving)
        {
            if (legForm)
            {
                animator.SetBool("Walking", true);
                animator.SetBool("Shuffling", false);
            } else
            {
                animator.SetBool("Shuffling", true);
                animator.SetBool("Walking", false);
            }
        } else
        {
            animator.SetBool("Shuffling", false);
            animator.SetBool("Walking", false);
        }
    }

    private void UpdateParticles()
    {
        ParticleSystem.EmissionModule walkingParticleEmissions = walkingParticleSystem.emission;
        walkingParticleEmissions.rateOverTime = (moving ? (legForm ? walkingParticleEmissionRate : shuffleParticleEmissionRate) : 0);
    }

    private void InterpolateRotation()
    {
        if (currMovementInput != Vector2.zero)
        {
            float desiredRotation = Utilities.ClampAngle0360(-Utilities.VectorToDegrees(Utilities.RotateVectorDegrees(currMovementInput, 135 - myCamera.transform.eulerAngles.y)));
            float rotationDiff = desiredRotation - currRotation;
            //print($"{currRotation}, {desiredRotation}");
            if (Mathf.Abs(rotationDiff) < 10f)
            {
                currRotation = desiredRotation;
            }
            else
            {
                currRotation = Utilities.ClampAngle0360(currRotation + rotationSpeed * Time.deltaTime * Utilities.DirectionToRotate(currRotation, desiredRotation));
            }
            transform.eulerAngles = new Vector3(0, currRotation);
        }
    }

    //slows down current box speed multiplier until it reaches the base multiplier
    private void Slide()
    {
        currBoxSpeed = Mathf.Max(boxSpeedMultiplier, currBoxSpeed - Time.deltaTime / boxSlideSlowdownRate * (1-boxSpeedMultiplier));
    }

    /// <summary>
    /// Toggle between box and leg form
    /// </summary>
    public void OnChangeForm(InputValue value)
    {
        
        if(!legForm)
        {
            if (Physics.Raycast(AISpotPoint.transform.position, Vector3.up, 1, LayerMask.GetMask("Obstructions", "Transparent Obstructions")))
                return;
        }

        legForm = !legForm;

        legformHitbox.enabled = legForm;
        boxformHitbox.enabled = !legForm;

        leggs.SetActive(legForm);
        if (legForm)
        {
            transform.position += new Vector3(0, 0.65f);
        }
        else
        {
            transform.position -= new Vector3(0, 0.65f);
        }
        walkingParticleSystem.transform.localPosition = (legForm ? legParticlesPosition.localPosition : boxParticlesPosition.localPosition);

        animator.SetFloat("Idle Speed", (legForm ? 1f : 0f));

        currBoxSpeed = 1;
    }

    /// <summary>
    /// Rotate camera 90 degrees around player
    /// </summary>
    public void OnRotate(InputValue value)
    {
        RotationDirection direction = (value.Get<float>() > 0 ? RotationDirection.CLOCKWISE : RotationDirection.COUNTERCLOCKWISE);
        myCamera.Rotate(direction);
    }

    public void OnChangeShoes(InputValue value)
    {
        float shoeVal = value.Get<float>();
        ShoeType shoeType = (ShoeType)shoeVal;
        EquipShoe(shoeType);
    }

    private void EquipShoe(ShoeType shoeType)
    {
        shoeManager.SwitchTo(shoeType);
    }

    /// <summary>
    /// Action button pressed
    /// </summary>
    public void OnAction()
    {
        if (legForm)
        {
            switch(shoeManager.currShoe)
            {
                case ShoeType.BAREFOOT:
                    break;
                case ShoeType.BOOTS:
                    animator.SetTrigger("Kick");
                    shoeManager.UseShoes();
                    break;
                case ShoeType.FLIPFLOPS:
                    animator.SetTrigger("Kick");
                    shoeManager.UseShoes();
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
        UIPauseMenu.instance.TogglePause();
        if(UIPauseMenu.instance.paused)
            inputSystem.SwitchCurrentActionMap("UI");
        else
            inputSystem.SwitchCurrentActionMap("Player");
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
        makingFootsteps = true;
        if (footstepSoundOffset > 0f)
            yield return new WaitForSeconds(footstepSoundOffset);
        float timeSinceLast = footstepTiming;
        while (moving)
        {
            timeSinceLast += Time.deltaTime;
            if (legForm && timeSinceLast >= footstepTiming)
            {
                AudioManager.MakeNoise(transform.position, 1.3f, null, 0);
                timeSinceLast = 0f;
                onStep.Post(gameObject);
            }
            yield return null;
        }
        makingFootsteps = false;
    }

    public void OnTest()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }
}
