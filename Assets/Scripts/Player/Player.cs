using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Component References")]
    public Rigidbody rigidbody;
    public Animator[] animators;
    public BoxCollider legformHitbox;
    public BoxCollider legformHitbox2;
    public BoxCollider boxformHitbox;
    public ShoeSniffer shoeSniffer;
    public PlayerShoeManager shoeManager;
    public PlayerInput inputSystem;
    public ParticleSystem walkingParticleSystem;
    public Transform AISpotPoint;
    public GameObject[] leggs;
    public ShoeSight shoeSight;

    [Header("Stats")]
    public ShoeType[] startingShoes;
    public float baseSpeed;
    public float boxSpeedMultiplier;
    public float boxSlideSlowdownRate;
    public float rotationSpeed;
    public bool legForm;
    public float timeToFling;

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

    [Header("Starting Properties")]
    public int wigglesRequired;
    public bool lockChangeForm;
    public bool lockShoeSight;
    [HideInInspector]
    public bool lockMovement;

    private const float wiggleCD = 0.7f;
    private float currWiggleCD;
    
    private Vector2 currMovementInput;
    private float currRotation;
    private CameraScript myCamera;
    //private Shoe currShoe;
    private static float intenseRumbleTime;
    private static float mediumRumbleTime;
    private static float weakRumbleTime;
    private float currBoxSpeed;
    private UIShoeTag shoeTagUI;
    [HideInInspector]
    public float verticalBoost;
    public bool usingController => inputSystem.currentControlScheme.Equals("Gamepad");
    private int _npcsChasing;
    public int npcsChasing
    {
        get => _npcsChasing;
        set
        {
            if (_npcsChasing == 0 && value > 0)
                OnChaseStarted();
            else if (_npcsChasing > 0 && value == 0)
                OnChaseEnded();
            _npcsChasing = value;
        }
    }
    private UITutorialManager tutorial;
    private SandalTutorial sandalTutorial;
    private static Player instance;
    bool holdingAction;
    bool inFlingRoutine;
    bool holdingForceReload;
    public float minY = -10f;


    private void Start()
    {
        myCamera = FindObjectOfType<CameraScript>();
        if (myCamera == null)
            Debug.LogError("Player couldn't find camera script");
        currBoxSpeed = boxSpeedMultiplier;
        shoeTagUI = FindObjectOfType<UIShoeTag>();
        if (shoeTagUI == null)
            Debug.LogError("Player couldn't find shoe tag UI");

        EquipStartingShoes();

        tutorial = FindObjectOfType<UITutorialManager>();
        sandalTutorial = FindObjectOfType<SandalTutorial>();
        instance = this;
    }

    public void EquipStartingShoes()
    {
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

    public void OnSwap()
    {
        if (legForm)
        {
            if (sandalTutorial)
                sandalTutorial.HideSwapControls();
            if (shoeManager.currShoe == ShoeType.BOOTS)
                shoeManager.SwitchTo(ShoeType.FLIPFLOPS);
            else if (shoeManager.currShoe == ShoeType.FLIPFLOPS)
                shoeManager.SwitchTo(ShoeType.BOOTS);
        }
    }

    private void FixedUpdate()
    {
        if (StealFocusWhenSeen.activeThief == null || !StealFocusWhenSeen.activeThief.lockMovement)
        {
            if (!inFlingRoutine)
            {
                Vector3 movementVector = CalculateMovementVector();
                if (wigglesRequired == 0)
                {
                    if (!lockMovement)
                    {
                        rigidbody.velocity = movementVector;
                        if (verticalBoost != 0 && movementVector != Vector3.zero)
                        {
                            transform.Translate(Vector3.up * verticalBoost);
                        }
                    }
                }
                else if (currWiggleCD <= 0f && movementVector != Vector3.zero)
                {
                    currWiggleCD = wiggleCD;
                    foreach (Animator animator in animators)
                    {
                        animator?.SetTrigger("Wiggle");
                    }
                }
                else if (currWiggleCD > 0f)
                {
                    currWiggleCD -= Time.fixedDeltaTime;
                    if (currWiggleCD <= 0f)
                    {
                        wigglesRequired--;
                        if (wigglesRequired == 0)
                        {
                            if (UITutorialManager.instance)
                            {
                                UITutorialManager.instance.initialFocusStealer.Skip();
                            }
                        }
                    }
                }
            } else
            {
                rigidbody.velocity = Vector3.zero;
            }
        } else
        {
            rigidbody.velocity = Vector3.zero;
        }
    }

    private Vector3 CalculateMovementVector()
    {
        Vector2 adjustedMovement = Utilities.RotateVectorDegrees(currMovementInput * baseSpeed * (legForm ? 1 : currBoxSpeed) * Time.fixedDeltaTime, 180 - myCamera.transform.eulerAngles.y);
        return new Vector3(adjustedMovement.x, rigidbody.velocity.y, adjustedMovement.y);
    }

    private void Update()
    {
        UpdateRumble();
        if (!legForm) Slide();
        InterpolateRotation();
        UpdateAnimator();
        UpdateParticles();
        UpdateFling();

        if (shoeSniffer.detectedShoe && legForm)
        {
            shoeManager.Acquire(shoeSniffer.detectedShoe.shoeType);
            EquipShoe(shoeSniffer.detectedShoe.shoeType);
            Destroy(shoeSniffer.detectedShoe.gameObject);
            if (tutorial)
            {
                tutorial.DoKickTutorial();
            }
            if (sandalTutorial)
            {
                sandalTutorial.TeachFling();
            }
        }

        if (transform.position.y < minY)
        {
            LevelBridge.Reload("Oops, that's our fault.");
        }
    }

    private void UpdateAnimator()
    {
        foreach (Animator animator in animators)
        {
            if (animator)
            {
                if (moving && wigglesRequired == 0)
                {
                    if (legForm)
                    {

                        animator.SetBool("Walking", true);
                        animator.SetBool("Shuffling", false);
                    }
                    else
                    {
                        animator.SetBool("Shuffling", true);
                        animator.SetBool("Walking", false);
                    }
                }
                else
                {
                    animator.SetBool("Shuffling", false);
                    animator.SetBool("Walking", false);
                }
                animator.SetFloat("Idle Speed", (legForm ? 1f : 0f));
            }
        }
    }

    private void UpdateParticles()
    {
        ParticleSystem.EmissionModule walkingParticleEmissions = walkingParticleSystem.emission;
        walkingParticleEmissions.rateOverTime = (moving ? (legForm ? walkingParticleEmissionRate : shuffleParticleEmissionRate) : 0);
    }

    private void InterpolateRotation()
    {
        if (StealFocusWhenSeen.activeThief == null || !StealFocusWhenSeen.activeThief.lockMovement)
        {
            if (shoeManager.currShoe == ShoeType.FLIPFLOPS && shoeManager.sandalSlinger.currTarget && inFlingRoutine)
            {
                transform.LookAt(shoeManager.sandalSlinger.currTarget);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y - 90, 0);
            }
            else if (currMovementInput != Vector2.zero && wigglesRequired == 0 && !lockMovement)
            {
                float desiredRotation = Utilities.ClampAngle0360(-Utilities.VectorToDegrees(Utilities.RotateVectorDegrees(currMovementInput, 180 - myCamera.transform.eulerAngles.y)));
                float rotationDiff = desiredRotation - currRotation;
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
    }

    //slows down current box speed multiplier until it reaches the base multiplier
    private void Slide()
    {
        currBoxSpeed = Mathf.Max(boxSpeedMultiplier, currBoxSpeed - Time.deltaTime / boxSlideSlowdownRate * (1-boxSpeedMultiplier));
    }

    /// <summary>
    /// Toggle between box and leg form
    /// </summary>
    public void OnChangeForm()
    {
        if (!lockChangeForm)
        {
            if (!legForm)
            {
                if (Physics.Raycast(AISpotPoint.transform.position, Vector3.up, 1, LayerMask.GetMask("Obstructions", "Transparent Obstructions")))
                    return;
            }

            if (tutorial)
            {
                tutorial.DidLegForm();
            }
            legForm = !legForm;

            legformHitbox.enabled = legForm;
            if (legformHitbox2)
                legformHitbox2.enabled = legForm;
            boxformHitbox.enabled = !legForm;

            foreach (GameObject legg in leggs)
                legg.SetActive(legForm);
            if (legForm)
            {
                transform.position += new Vector3(0, 0.65f);
            }
            else
            {
                transform.position -= new Vector3(0, 0.65f);
            }
            walkingParticleSystem.transform.localPosition = (legForm ? legParticlesPosition.localPosition : boxParticlesPosition.localPosition);


            currBoxSpeed = 1;
        }
    }

    /// <summary>
    /// Rotate camera 90 degrees around player
    /// </summary>
    public void OnRotate(InputValue value)
    {
        if (wigglesRequired == 0 && StealFocusWhenSeen.activeThief == null)
        {
            RotationDirection direction = (value.Get<float>() > 0 ? RotationDirection.CLOCKWISE : RotationDirection.COUNTERCLOCKWISE);
            myCamera.Rotate(direction);
        }
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
    public void OnAction(InputValue value)
    {
        holdingAction = value.Get<float>() >= 0.5f;
        if (legForm && holdingAction)
        {
            switch(shoeManager.currShoe)
            {
                case ShoeType.BAREFOOT:
                    break;
                case ShoeType.BOOTS:
                    foreach(Animator animator in animators)
                        animator.SetTrigger("Kick");
                    shoeManager.UseShoes();
                    break;
                case ShoeType.FLIPFLOPS:
                    break;
                default:
                    Debug.Log("you dun fucked up boi");
                    break;
            }
        }
    }

    void UpdateFling()
    {
        if (holdingAction && !shoeManager.sandalSlinger.slinging && !inFlingRoutine && shoeManager.currShoe == ShoeType.FLIPFLOPS && legForm)
            StartCoroutine(DoFling());
        if (inFlingRoutine)
        {
            if (currMovementInput != Vector2.zero)
            {
                shoeManager.sandalSlinger.desiredForward = Utilities.RotateVectorDegrees(currMovementInput, 180 - myCamera.transform.eulerAngles.y);
            }
        }
    }

    IEnumerator DoFling()
    {
        inFlingRoutine = true;
        shoeManager.sandalSlinger.holdingShot = true;
        foreach (Animator animator in animators)
        {
            if (animator)
            {
                animator.SetTrigger("Fling");
                animator.SetFloat("FlingSpeed", 1.2f);
            }
        }
        yield return new WaitForSeconds(timeToFling);
        foreach (Animator animator in animators)
        {
            if (animator)
            {
                animator.SetFloat("FlingSpeed", 0);
            }
        }
        while (holdingAction)
        {
            if (shoeManager.currShoe != ShoeType.FLIPFLOPS || !legForm)
            {
                inFlingRoutine = false;
                shoeManager.sandalSlinger.holdingShot = false;
                yield break;
            }
            yield return null;
        }
        foreach (Animator animator in animators)
        {
            if (animator)
            {
                animator.SetFloat("FlingSpeed", 1);
            }
        }
        shoeManager.UseShoes();
        shoeManager.sandalSlinger.holdingShot = false;
        inFlingRoutine = false;
    }

    //tell UI to pause or unpause
    public void OnPauseMenu(InputValue value)
    {
        if (StealFocusWhenSeen.activeThief == null)
        {
            UIPauseMenu.instance.TogglePause();
            if (UIPauseMenu.instance.paused)
                inputSystem.SwitchCurrentActionMap("UI");
            else if (!UIPopup.popupActive)
                inputSystem.SwitchCurrentActionMap("Player");
        } else
        {
            StealFocusWhenSeen.SkipActive();
        }
    }

    public void OnShoeSight()
    {
        if (!lockShoeSight)
        {
            shoeSight.ActivateSight();
            UIShoeSightReminder.instance.ShoeSightUsed();
        }
    }

    public void OnTutorialContinue()
    {
        if (UIPopup.popupActive)
            UIPopup.activePopup.Dismiss();
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
                    lowFreqSpeed = 0.2f;
                    if (weakRumbleTime <= 0f && mediumRumbleTime <= 0f && intenseRumbleTime <= 0f)
                        (inputSystem.devices[0] as Gamepad).SetMotorSpeeds(lowFreqSpeed, highFreqSpeed);
                    weakRumbleTime = Mathf.Max(time, weakRumbleTime);
                    break;
                case RumbleStrength.MEDIUM:
                    lowFreqSpeed = 0.35f;
                    highFreqSpeed = 0.65f;
                    if (mediumRumbleTime <= 0f && intenseRumbleTime <= 0f)
                        (inputSystem.devices[0] as Gamepad).SetMotorSpeeds(lowFreqSpeed, highFreqSpeed);
                    mediumRumbleTime = Mathf.Max(time, mediumRumbleTime);
                    break;
                case RumbleStrength.INTENSE:
                    lowFreqSpeed = 1f;
                    highFreqSpeed = 1f;
                    if (intenseRumbleTime <= 0f)
                        (inputSystem.devices[0] as Gamepad).SetMotorSpeeds(lowFreqSpeed, highFreqSpeed);
                    intenseRumbleTime = Mathf.Max(time, intenseRumbleTime);
                    break;
            }
        }
    }

    public static void ControllerRumble(RumbleStrength strength, float time)
    {
        if (instance)
            instance.Rumble(strength, time);
    }

    //automatically turns off controller rumble once time is up
    private void UpdateRumble() {
        if (inputSystem?.currentControlScheme == "Gamepad")
        {
            if (intenseRumbleTime > 0f)
            {
                intenseRumbleTime -= Time.unscaledDeltaTime;
                if (intenseRumbleTime <= 0f)
                {
                    if (mediumRumbleTime > 0f)
                        (inputSystem.devices[0] as Gamepad).SetMotorSpeeds(0.35f, 0.65f);
                    else if (weakRumbleTime > 0f)
                        (inputSystem.devices[0] as Gamepad).SetMotorSpeeds(0.2f, 0f);
                    else
                        (inputSystem.devices[0] as Gamepad).SetMotorSpeeds(0f, 0f);
                }
            }
            if (mediumRumbleTime > 0f)
            {
                mediumRumbleTime -= Time.unscaledDeltaTime;
                if (mediumRumbleTime <= 0f && intenseRumbleTime <= 0f)
                {
                    if (weakRumbleTime > 0f)
                        (inputSystem.devices[0] as Gamepad).SetMotorSpeeds(0.2f, 0f);
                    else
                        (inputSystem.devices[0] as Gamepad).SetMotorSpeeds(0f, 0f);
                }
            }
            if (weakRumbleTime > 0f)
            {
                weakRumbleTime -= Time.unscaledDeltaTime;
                if (weakRumbleTime <= 0f && intenseRumbleTime <= 0f && mediumRumbleTime <= 0f)
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
            intenseRumbleTime = 0f;
            mediumRumbleTime = 0f;
            weakRumbleTime = 0f;
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

#if UNITY_EDITOR
    public void OnTest()
    {

    }

    public void OnCheckpointLoad(InputValue value)
    {
        int checkpoint = Mathf.FloorToInt(value.Get<float>());
        CheckpointManager checkpointManager = FindObjectOfType<CheckpointManager>();
        checkpointManager.SetCheckpoint(checkpoint, true);
        if (!holdingForceReload)
            checkpointManager.ReloadCheckpointItems();
        else
            LevelBridge.Reload($"restarting from checkpoint {checkpoint}");
    }

    public void OnCheckpointForceReload(InputValue value)
    {
        holdingForceReload = (value.Get<float>()) > 0.5f;
    }
#endif

    private void OnChaseStarted()
    {
        Debug.Log("player chase begins");
    }

    private void OnChaseEnded()
    {
        Debug.Log("player chase ends");
    }
}
