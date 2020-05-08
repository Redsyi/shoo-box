using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// big boi class that manages the player
/// </summary>
public class Player : MonoBehaviour
{
    public class PlayerState
    {
        public Quaternion rotation;
        public bool legForm;
        public ShoeType selectedShoe;
    }

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
    public Image health;
    public GameObject boxShadow;
    public GameObject legShadow;
    public PlayerTank playerTankEasterEgg;

    [Header("Stats")]
    public ShoeType[] startingShoes;
    public float baseSpeed;
    public float boxSpeedMultiplier;
    public float boxSlideSlowdownRate;
    public float rotationSpeed;
    public bool legForm;
    public float timeToFling;
    public float heightDifference = 0.65f;

    [Header("Footsteps")]
    public float footstepTiming;
    public float footstepSoundOffset;
    public bool moving => currMovementInput != Vector2.zero;
    public float footstepRadius = 1.3f;
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
    public bool loadPreviousState;

    private const float wiggleCD = 0.9f;
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
    float currSmoothRotation;
    float currSnapRotate;
    [Header("Settings")]
    public float minY = -10f;
    public bool useSnapRotation;
    public static PlayerState prevState;
    public static Player current;
    public AK.Wwise.Event idleSound;

    private void Awake()
    {
        current = this;
    }

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

        //check if we're in a tutorial
        tutorial = FindObjectOfType<UITutorialManager>();
        sandalTutorial = FindObjectOfType<SandalTutorial>();

        instance = this;
        
        //load previous state if we can/should
        if (loadPreviousState && prevState != null)
        {
            Invoke("LoadPreviousState", 0.1f);
        }

        idleSound.Post(gameObject);

        PlayerData.CheckLoadedData();
    }

    void LoadPreviousState()
    {
        transform.rotation = prevState.rotation;
        if (legForm != prevState.legForm)
            ToggleForm();
        EquipShoe(prevState.selectedShoe);
    }

    private void OnDestroy()
    {
        prevState = new PlayerState() { rotation = transform.rotation, legForm = legForm, selectedShoe = shoeManager.currShoe };
        if (current == this)
            current = null;
    }

    /// <summary>
    /// tell the shoe manager what shoes we have
    /// </summary>
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
        //special case: moving will disqualify the player from getting the merry-go-round jibbit in Terminal II
        JibbitHorseGiver.qualified = false;
        
        bool wasMoving = (currMovementInput == Vector2.zero);
        currMovementInput = value.Get<Vector2>();
        if (currMovementInput != Vector2.zero && !wasMoving && !makingFootsteps)
            StartCoroutine(DoFootsteps());

        if (currMovementInput.sqrMagnitude > 1)
            currMovementInput = currMovementInput.normalized;
    }

    /// <summary>
    /// action performed when the swap control is pressed. 
    /// </summary>
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

    /// <summary>
    /// manage actual player movement
    /// </summary>
    private void FixedUpdate()
    {
        //don't move when a cutscene is locking player movement
        if (StealFocusWhenSeen.activeThief == null || !StealFocusWhenSeen.activeThief.lockMovement)
        {
            //don't move if we are currently flinging a sandal
            if (!inFlingRoutine)
            {
                Vector3 movementVector = CalculateMovementVector();
                //don't move if we need to wiggle, instead perform a wiggle
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

    /// <summary>
    /// returns a vector showing how the player should translate in absolute world-space depending on movement input and camera rotation
    /// </summary>
    /// <returns></returns>
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
        DoSmoothRotation();

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

    /// <summary>
    /// sets animator variables depending on state
    /// </summary>
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

    /// <summary>
    /// manages walking/shuffling particles
    /// </summary>
    private void UpdateParticles()
    {
        ParticleSystem.EmissionModule walkingParticleEmissions = walkingParticleSystem.emission;
        walkingParticleEmissions.rateOverTime = (moving ? (legForm ? walkingParticleEmissionRate : shuffleParticleEmissionRate) : 0);
    }

    /// <summary>
    /// provides smooth rotation between the player's current rotation and desired rotation
    /// </summary>
    private void InterpolateRotation()
    {
        //...unless a cutscene is locking movement...
        if (StealFocusWhenSeen.activeThief == null || !StealFocusWhenSeen.activeThief.lockMovement)
        {
            //...or we are flinging a sandal, in which case set rotation to look at target instead
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
        if (!lockChangeForm && (StealFocusWhenSeen.activeThief == null || !StealFocusWhenSeen.activeThief.lockMovement))
        {
            ToggleForm();
        }
    }

    /// <summary>
    /// toggles the player between leg and box form
    /// </summary>
    void ToggleForm()
    {
        //don't stand up if we are blocked above
        if (!legForm)
        {
            if (Physics.Raycast(AISpotPoint.transform.position, Vector3.up, 1, LayerMask.GetMask("Obstructions", "Transparent Obstructions")))
                return;
        }

        //tell tutorial we stood up! yay! we did it!~
        if (tutorial)
        {
            tutorial.DidLegForm();
        }

        //actual important line lol
        legForm = !legForm;

        //switch to correct hitbox
        legformHitbox.enabled = legForm;
        if (legformHitbox2)
            legformHitbox2.enabled = legForm;
        boxformHitbox.enabled = !legForm;

        //hide/show legs depending on form
        foreach (GameObject legg in leggs)
            legg.SetActive(legForm);

        //switch to correct shadow
        if (legShadow)
            legShadow.SetActive(legForm);
        if (boxShadow)
            boxShadow.SetActive(!legForm);

        //translate up/down
        if (legForm)
        {
            transform.position += new Vector3(0, heightDifference);
        }
        else
        {
            transform.position -= new Vector3(0, heightDifference);
        }

        //adjust movement particle position
        walkingParticleSystem.transform.localPosition = (legForm ? legParticlesPosition.localPosition : boxParticlesPosition.localPosition);
        
        //reset slide
        currBoxSpeed = 1;
    }

    /// <summary>
    /// Rotate camera
    /// </summary>
    public void OnRotate(InputValue value)
    {
        if (wigglesRequired == 0)
        {
            float val = value.Get<float>();
            //deprecated snap rotation code
            if (useSnapRotation)
            {
                if (Mathf.Abs(val) >= 0.5f && (currSnapRotate == 0 || Mathf.Sign(val) != Mathf.Sign(currSnapRotate)) && StealFocusWhenSeen.activeThief == null)
                {
                    RotationDirection direction = (val > 0 ? RotationDirection.CLOCKWISE : RotationDirection.COUNTERCLOCKWISE);
                    myCamera.Rotate(direction);
                }
                currSnapRotate = val;
            } else
            //much nicer smooth rotation code
            {
                currSmoothRotation = val;
            }
        }
    }

    /// <summary>
    /// tells the camera to smoothly rotate based on current camera rotation input
    /// </summary>
    private void DoSmoothRotation()
    {
        if (!useSnapRotation && currSmoothRotation != 0 && StealFocusWhenSeen.activeThief == null)
            myCamera.SmoothRotate(currSmoothRotation);
    }

    /// <summary>
    /// somewhat deprecated but still supported - switching to a specific shoe
    /// </summary>
    public void OnChangeShoes(InputValue value)
    {
        float shoeVal = value.Get<float>();
        ShoeType shoeType = (ShoeType)shoeVal;
        EquipShoe(shoeType);
    }

    /// <summary>
    /// attempt to equip the specified shoe type
    /// </summary>
    private void EquipShoe(ShoeType shoeType)
    {
        shoeManager.SwitchTo(shoeType);
    }

    /// <summary>
    /// Action button pressed, pretty much just passes message to shoe manager
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

    /// <summary>
    /// manages player's desired fling direction while flinging a sandal
    /// </summary>
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

    /// <summary>
    /// manages the steps in performing a sandal fling
    /// </summary>
    IEnumerator DoFling()
    {
        inFlingRoutine = true;
        shoeManager.sandalSlinger.holdingShot = true;

        //sets animators...
        foreach (Animator animator in animators)
        {
            if (animator)
            {
                animator.SetTrigger("Fling");
                animator.SetFloat("FlingSpeed", 1.2f);
            }
        }
        yield return new WaitForSeconds(timeToFling);

        //freezes animators while waiting for actual fing
        foreach (Animator animator in animators)
        {
            if (animator)
            {
                animator.SetFloat("FlingSpeed", 0);
            }
        }

        //manage aborting action by switching forms or shoes
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

        //resume animators
        foreach (Animator animator in animators)
        {
            if (animator)
            {
                animator.SetFloat("FlingSpeed", 1);
            }
        }

        //fire
        shoeManager.UseShoes();
        shoeManager.sandalSlinger.holdingShot = false;
        inFlingRoutine = false;
    }

    //tell UI to pause or unpause, or skip active cutscene
    public void OnPauseMenu(InputValue value)
    {
        if (StealFocusWhenSeen.activeThief == null)
        {
            if (!UIPopup.popupActive)
                UIPauseMenu.instance.TogglePause();
        } else
        {
            //can always skip cutscenes while in editor
            bool editorOverride = false;
#if UNITY_EDITOR
            editorOverride = true;
#endif
            if (StealFocusWhenSeen.activeThief.skippable || editorOverride || Devmode.active)
                StealFocusWhenSeen.SkipActive();
        }

        //"start" can also be the final konami entry. unfortunately, because of the way it switches the input map, the
        //tank easter egg never recieves the signal that "start" was pressed, so we have to tell it ourselves
        if (playerTankEasterEgg != null && Controls.usingController)
        {
            playerTankEasterEgg.OnKonamiStart();
        }
    }

    /// <summary>
    /// called when player presses shoe sight button
    /// </summary>
    public void OnShoeSight()
    {
        if (!lockShoeSight)
        {
            shoeSight.ActivateSight();
            UIShoeSightReminder.instance.ShoeSightUsed();
        }
    }

    /// <summary>
    /// dismisses current tutorial popup
    /// </summary>
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
        if (Controls.usingController)
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

    /// <summary>
    /// tell current player instance to rumble controller
    /// </summary>
    public static void ControllerRumble(RumbleStrength strength, float time)
    {
        if (instance)
            instance.Rumble(strength, time);
    }

    //automatically turns off controller rumble once time is up
    private void UpdateRumble() {
        if (Controls.usingController)
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
        if (Controls.usingController)
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
                AudioManager.MakeNoise(transform.position, footstepRadius, null, 0);
                timeSinceLast = 0f;
                onStep.Post(gameObject);
            }
            yield return null;
        }
        makingFootsteps = false;
    }

    /// <summary>
    /// originally used for random crap, now officially the "hide UI button" (only in devmode)
    /// </summary>
    public void OnTest()
    {
        bool editorOverride = false;
#if UNITY_EDITOR
        editorOverride = true;
#endif
        if (Devmode.active || editorOverride)
        {
            Canvas UI = GameObject.Find("UI").GetComponent<Canvas>();
            if (UI)
            {
                UI.enabled = !UI.enabled;
            }
            GameObject tutorialObj = GameObject.Find("Tutorial");
            if (tutorialObj)
            {
                Canvas Tutorial = tutorialObj.GetComponent<Canvas>();
                Tutorial.enabled = !Tutorial.enabled;
            }
        }
    }

    /// <summary>
    /// editor/devmode-only load checkpoint button
    /// </summary>
    public void OnCheckpointLoad(InputValue value)
    {
        bool editorOverride = false;
#if UNITY_EDITOR
        editorOverride = true;
#endif
        if (Devmode.active || editorOverride)
        {
            int checkpoint = Mathf.FloorToInt(value.Get<float>());
            CheckpointManager checkpointManager = FindObjectOfType<CheckpointManager>();
            checkpointManager.SetCheckpoint(checkpoint, true);
            if (!holdingForceReload)
                checkpointManager.ReloadCheckpointItems();
            else
                LevelBridge.Reload($"restarting from checkpoint {checkpoint}");
        }
    }

    /// <summary>
    /// modifies the above checkpoint load functionality to force a full reload rather than a quick reload
    /// </summary>
    public void OnCheckpointForceReload(InputValue value)
    {
            holdingForceReload = (value.Get<float>()) > 0.5f;
    }

    //cinematic camera controls start
    public void OnCinematicAngle(InputValue value)
    {
        bool editorOverride = false;
#if UNITY_EDITOR
        editorOverride = true;
#endif
        if (Devmode.active || editorOverride)
        {
            myCamera.cinematicAngleDelta = value.Get<float>();
        }
    }

    public void OnCinematicZoom(InputValue value)
    {
        bool editorOverride = false;
#if UNITY_EDITOR
        editorOverride = true;
#endif
        if (Devmode.active || editorOverride)
        {
            myCamera.cinematicZoomDelta = value.Get<float>();
        }
    }

    public void OnCinematicRaise(InputValue value)
    {
        bool editorOverride = false;
#if UNITY_EDITOR
        editorOverride = true;
#endif
        if (Devmode.active || editorOverride)
        {
            myCamera.cinematicRaiseDelta = value.Get<float>();
        }
    }

    public void OnCinematicMode()
    {
        bool editorOverride = false;
#if UNITY_EDITOR
        editorOverride = true;
#endif
        if (Devmode.active || editorOverride)
        {
            myCamera.cinematicMode = !myCamera.cinematicMode;
        }
    }

    public void OnCinematicPause()
    {
        bool editorOverride = false;
#if UNITY_EDITOR
        editorOverride = true;
#endif
        if (Devmode.active || editorOverride)
        {
            Time.timeScale = (Time.timeScale != 1 ? 1 : 0);
        }
    }

    public void OnLook(InputValue value)
    {
        bool editorOverride = false;
#if UNITY_EDITOR
        editorOverride = true;
#endif
        if (Devmode.active || editorOverride)
        {
            Vector2 mouseDelta = value.Get<Vector2>();
            myCamera.mouseDelta = mouseDelta;
        }
    }
    //cinematic camera controls end

    //intended for use with dynamic music
    private void OnChaseStarted()
    {
        //Debug.Log("player chase begins");
    }

    private void OnChaseEnded()
    {
        //Debug.Log("player chase ends");
    }

    //changes current tab in the pause menu
    public void OnChangeTab(InputValue val)
    {
        if (UIPauseMenu.instance)
        {
            float value = val.Get<float>();
            if (value > 0)
            {
                UIPauseMenu.instance.NextTab();
            } else
            {
                UIPauseMenu.instance.PrevTab();
            }
        }
    }
}
