using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

/// <summary>
/// manages npc behavior
/// </summary>
public class AIAgent : MonoBehaviour
{
    [Header("Component refs")]
    public Animator animator;
    public NavMeshAgent pathfinder;
    public ThrowItem thrower;
    public Transform leftHand;
    public Transform rightHand;

    [Header("Behavior")]
    public Transform[] patrolPoints;
    public AIInterest[] interests;
    public bool deaf;
    public float stopDist = 1f;
    public string doorOpenAnimTrigger = "OpenDoor";

    [Header("Stats")]
    public float walkSpeed;
    public float runSpeed;
    public float throwWaitTime;
    public float interactSpeedMultiplier = 1;

    [Header("Prefab refs")]
    public GameObject targetPrefab;
    public UINPCBubble bubblePrefab;
    public Transform bubbleAnchor;

    [Header("Audio")]
    public AK.Wwise.Event onSpot;
    public AK.Wwise.Event onInvestigate;

    [Header("Misc")]
    [Tooltip("The name displayed in \"you were caught by _____\" when you get caught")]
    public string gameOverName;
    public bool debug;

    public ChaseBehaviors chaseBehavior;
    public AIInterest[] chaseBroadcastInterests;
    [HideInInspector] public ScriptedSequence chaseOverrideSequence;


    private AIState _state;
    public AIState state
    {
        get
        {
            return _state;
        }
        set
        {
            if (_state != value || !initializedState)
            {
                if (initializedState)
                    OnStateExit(_state);
                OnStateEnter(value);
                _state = value;
                initializedState = true;
            }
        }
    }
    [HideInInspector]
    public Transform destination;
    bool initializedState;

    private UINPCBubble myBubble;           //reference to instantiated ?/! bubble
    private GameObject instantiatedTarget;  //reference to a reusable "target point" gameobject
    private bool stopped;                   //is the agent standing still
    private Vector3 prevPos;                //used for calculating "stopped"
    private float stoppedTime = 0f;         //how long agent has been standing still
    private const float giveUpTime = 1f;    //how long the agent investigates a lost player's position before giving up
    public static bool blindAll;            //static bool, if toggled on, all AI lose sight
    private bool reachedInteractable;       //if true, agent goes from navigating to interactable to performing interact animation
    private AKEventNPC wwiseComponent;      //deprecated
    private int currPatrolPoint;            //which patrol point in patrolPoints the agent is on
    private CustomShoeSight shoeSightColoring;      //reference to the shoe sight colorer, we need this ref so we can change from red to yellow
    private ShoeSightType originalSightColoring;    //original sight coloring, helps us know not to change to red if we weren't originally red
    private bool investigateSoundPlayed;
    private float _spotProgress;
    AIVision vision;

    /// <summary>
    /// clamped value from 0..1 representing how "spotted" the player is by this agent
    /// </summary>
    public float spotProgress
    {
        get
        {
            return _spotProgress;
        }
        set
        {
            float clampedProgress = Mathf.Clamp01(value);
            if (_spotProgress != clampedProgress)
            {
                _spotProgress = clampedProgress;
                myBubble.stealthProgress = _spotProgress;
            }
        }
    }
    [HideInInspector]
    public bool stunned;    //if this agent is stunned
    ScriptedSequence[] sequences;   //refs to all possible sequences on this agent
    float timeSinceLastOnSpot;          //this and below help prevent sound spam with rapid changing of states
    float timeSinceLastOnInvestigate;
    public Queue<IAIInteractable> thingsToInteractWith; //current list of items the agent has detected need interaction
    float timer;        //general timer variable used across all states
    [HideInInspector] public bool investigatingPlayer;
    const float updateInterval = 0.05f;
    static List<AIAgent> activeAgents;

    void Start()
    {
        blindAll = false;
        thingsToInteractWith = new Queue<IAIInteractable>();
        Idle();
        
        GameObject bubbleCanvas = GameObject.FindGameObjectWithTag("NPC Bubble Canvas");
        if (bubbleCanvas == null)
            Debug.LogError("AIAgent couldn't find bubble canvas");
        myBubble = Instantiate(bubblePrefab, bubbleCanvas.transform);
        myBubble.worldAnchor = bubbleAnchor;
        myBubble.useRadioBubbles = chaseBehavior == ChaseBehaviors.SUMMON;

        StartCoroutine(CheckPos());
        wwiseComponent = GetComponent<AKEventNPC>();
        shoeSightColoring = GetComponent<CustomShoeSight>();
        if (shoeSightColoring)
            originalSightColoring = shoeSightColoring.type;
        sequences = GetComponents<ScriptedSequence>();

        if (activeAgents == null)
            activeAgents = new List<AIAgent>();
        activeAgents.Add(this);
        AudioManager.RegisterAgent(this);
        StartCoroutine(UpdateState());
        vision = GetComponentInChildren<AIVision>();
    }

    /// <summary>
    /// periodically checks and sees if the npc has stopped
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckPos()
    {
        while (true)
        {
            stopped = (prevPos - transform.position).sqrMagnitude <= walkSpeed*.05f;

            if (!InSequence() && !stunned) {
                if (stopped && state != AIState.IDLE)
                {
                    Face(destination.position);
                }
            }
            prevPos = transform.position;
            yield return new WaitForSeconds(.2f);
        }
     
    }

    /// <summary>
    /// returns if this npc is currently executing a sequence
    /// </summary>
    public bool InSequence()
    {
        if (sequences == null || sequences.Length == 0)
            return false;

        foreach (ScriptedSequence sequence in sequences)
        {
            if (sequence.running)
                return true;
        }

        return false;
    }

    /// <summary>
    /// makes the npc face a certain position, only rotating on the y axis.
    /// </summary>
    public void Face(Vector3 position)
    {
        Vector2 vectToDest = new Vector2(position.x, position.z) - new Vector2(transform.position.x, transform.position.z);
        if (vectToDest.sqrMagnitude > 0.25f)
        {
            transform.LookAt(position);
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y);
        }
    }

    /// <summary>
    /// Tell the AI to go to idle mode
    /// </summary>
    public void Idle()
    {
        if (state != AIState.IDLE || patrolPoints.Length > 1 || !destination)
            state = AIState.IDLE;
    }

    /// <summary>
    /// Tell the AI to investigate an object
    /// </summary>
    /// <param name="location">Transform to investigate</param>
    /// <param name="forceOverrideChase">Whether this trigger can interrupt a chase state</param>
    public void Investigate(GameObject location, float investigateTime = 3f, bool forceOverrideChase = false, bool forceOverrideInteract = true)
    {
        if ((state != AIState.CHASE || forceOverrideChase) && (state != AIState.INTERACT || forceOverrideInteract))
        {
            if (state != AIState.INVESTIGATE)
                wwiseComponent?.StartedInvestigation();
            state = AIState.INVESTIGATE;
            destination = location.transform;
            timer = investigateTime;
            investigatingPlayer = false;
        }
    }

    /// <summary>
    /// Tell the AI to investigate a location
    /// </summary>
    /// <param name="location">location to investigate</param>
    /// <param name="forceOverrideChase">Whether this trigger can interrupt a chase state</param>
    public void Investigate(Vector3 location, float investigateTime = 3f, bool forceOverrideChase = false, bool forceOverrideInteract = true)
    {
        GameObject target = Instantiate(targetPrefab, location, Quaternion.identity);
        Investigate(target, investigateTime, forceOverrideChase);
    }

    /// <summary>
    /// Tell the AI to interact with an object. Will add to back of interact queue if already interacting with
    /// something else or AI currently busy chasing player
    /// </summary>
    public void Interact(IAIInteractable interactable)
    {
        if (!thingsToInteractWith.Contains(interactable))
        {
            thingsToInteractWith.Enqueue(interactable);

            if (state != AIState.CHASE && state != AIState.INTERACT && thingsToInteractWith.Count == 1)
                state = AIState.INTERACT;
        }
    }

    /// <summary>
    /// Chase the player
    /// </summary>
    /// <param name="player"></param>
    public void Chase(Player player)
    {
        if (state != AIState.CHASE)
            state = AIState.CHASE;
    }

    /// <summary>
    /// Notify the AI that is has lost sight of the player
    /// </summary>
    public void LosePlayer(Player player)
    {
        if (state == AIState.CHASE)
        {
            player.npcsChasing--;
        }
        if (instantiatedTarget == null)
        {
            instantiatedTarget = Instantiate(targetPrefab, player.transform.position, Quaternion.identity);
        } else
        {
            instantiatedTarget.transform.position = player.transform.position;
        }
        myBubble.Lost();
        if (chaseBehavior != ChaseBehaviors.SEQUENCE)
        {
            Investigate(instantiatedTarget, forceOverrideChase: true);
            investigatingPlayer = true;
        }
        else
            Idle();
    }

    public void CatchPlayer(Player player)
    {
        //this is the part where the player fucking dies
        wwiseComponent?.PlayerCaught();
        AudioManager.playerWasCaught = true;
        LevelBridge.Reload($"Caught by {gameOverName}");
    }

    public void ChangePatrolPoint(int index)
    {
        currPatrolPoint = index;
    }
    
    void Update()
    {
        timeSinceLastOnSpot += Time.deltaTime;
        timeSinceLastOnInvestigate += Time.deltaTime;

        if (stopped && !stunned)
            stoppedTime += Time.deltaTime;
        else
            stoppedTime = 0f;

        //being in a sequence will temporarily stop this behavior
        if (!InSequence())
        {
            CheckInvalidLocation();
            UpdatePathfinder();
            UpdateAnimationState();

            if (debug)
                print($"{gameObject.name}: {state}, {destination}, {thingsToInteractWith.Count} | {stoppedTime}");

            //manages shoe sight coloring if everyone is blind
            if (shoeSightColoring)
            {
                if (blindAll)
                {
                    shoeSightColoring.SetType(ShoeSightType.BLIND_ENEMY);
                }
                else
                {
                    shoeSightColoring.SetType(originalSightColoring);
                }
            }
        }
    }

    void UpdatePathfinder()
    {
        //manages npc speed depending on state
        bool inGoodSightDistance = (vision && state == AIState.CHASE && chaseBehavior == ChaseBehaviors.SUMMON && Utilities.Flatten(transform.position - Player.current.transform.position).sqrMagnitude < Mathf.Pow(vision.radius * 0.3f, 2));

        if (pathfinder)
            pathfinder.speed = (stunned || inGoodSightDistance ? 0f : (state == AIState.CHASE || state == AIState.INTERACT ? runSpeed : walkSpeed));
    }

    void UpdateAnimationState()
    {
        //manages npc animations
        animator.SetBool("Moving", stoppedTime < 0.1f && !stunned);
        animator.SetBool("Running", state == AIState.CHASE || state == AIState.INTERACT);
        animator.SetBool("Interacting", state == AIState.INTERACT && reachedInteractable && !stunned);
    }

    public void ImConfused()
    {
        myBubble.Investigating();
    }

    public void ImNoLongerConfused()
    {
        myBubble.Investigating();
    }

    /// <summary>
    /// summons all npcs with the given interests to a specific gameobject
    /// </summary>
    public static void SummonAI(GameObject to, float investigateTime, bool interruptInteract = false, params AIInterest[] interests)
    {
        foreach (AIAgent agent in activeAgents)
        {
            if (agent)
            {
                foreach (AIInterest interest in interests)
                {
                    if (System.Array.Exists<AIInterest>(agent.interests, element => element == interest))
                    {
                        agent.Investigate(to, investigateTime, forceOverrideInteract: interruptInteract);
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// summons all npcs with the given interests to a specific location
    /// </summary>
    public static void SummonAI(Vector3 location, float investigateTime, bool interruptInteract = false, params AIInterest[] interests)
    {
        foreach (AIAgent agent in activeAgents)
        {
            if (agent)
            {
                foreach (AIInterest interest in interests)
                {
                    if (System.Array.Exists<AIInterest>(agent.interests, element => element == interest))
                    {
                        agent.Investigate(location, investigateTime, forceOverrideInteract: interruptInteract);
                        break;
                    }
                }
            }
        }
    }

    void CheckInvalidLocation()
    {
        //handles our current target being unexpectedly destroyed
        if (state != AIState.IDLE && !destination)
        {
            if (state == AIState.INTERACT)
            {
                IAIInteractable invalid = thingsToInteractWith.Dequeue();
                while (thingsToInteractWith.Count > 0)
                {
                    IAIInteractable newInteractable = thingsToInteractWith.Peek();
                    if (newInteractable as MonoBehaviour)
                    {
                        InteractWithNextInQueue();
                        break;
                    }
                    else
                    {
                        thingsToInteractWith.Dequeue();
                    }
                }
                if (thingsToInteractWith.Count == 0)
                {
                    Idle();
                }
            }
        }
    }

    /// <summary>
    /// code that runs when a state is switched to from a different state
    /// </summary>
    void OnStateEnter(AIState state)
    {
        switch(state)
        {
            case AIState.IDLE:
                stoppedTime = 0f;
                destination = patrolPoints[currPatrolPoint];
                investigatingPlayer = false;
                break;
            case AIState.INVESTIGATE:
                stoppedTime = 0f;
                break;
            case AIState.INTERACT:
                IAIInteractable interactable = thingsToInteractWith.Peek();
                destination = (interactable as MonoBehaviour).transform;
                wwiseComponent?.SomethingWrong();
                timer = interactable.AIInteractTime() * interactSpeedMultiplier;
                stoppedTime = 0f;
                reachedInteractable = false;
                investigatingPlayer = false;
                break;
            case AIState.CHASE:
                wwiseComponent?.PlayerSpotted();
                myBubble.Spotted();
                if (timeSinceLastOnSpot > 2)
                {
                    onSpot.Post(gameObject);
                    timeSinceLastOnSpot = 0;
                }
                Player.current.npcsChasing++;
                destination = Player.current.transform;
                investigatingPlayer = false;
                stoppedTime = 0f;
                timer = 0f;
                
                if (chaseBehavior == ChaseBehaviors.SEQUENCE)
                    chaseOverrideSequence.Trigger();

                if (chaseBehavior != ChaseBehaviors.SEQUENCE && InSequence())
                {
                    foreach (ScriptedSequence sequence in sequences)
                    {
                        if (sequence.running)
                        {
                            sequence.Interrupt();
                        }
                    }
                }
                break;
        }
    }

    /// <summary>
    /// runs state update code on an interval
    /// </summary>
    IEnumerator UpdateState()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval);
            while (InSequence())
                yield return null;
            OnStateUpdate(state);
        }
    }

    /// <summary>
    /// updates ourself based on state
    /// </summary>
    void OnStateUpdate(AIState state)
    {
        //detects whether we are close to our target; or if we tried to reach the target but ended up stopped (which is enough for some states)
        bool closeToTarget = (transform.position - destination.position).sqrMagnitude < (stopDist * stopDist);
        bool closeEnough = (reachedInteractable && state == AIState.INTERACT) || (stoppedTime >= giveUpTime) || closeToTarget;

        //take action depending on the current state
        switch (state)
        {
            case AIState.IDLE:
                OnIdleUpdate(closeToTarget);
                break;

            case AIState.INVESTIGATE:
                OnInvestigateUpdate(closeEnough);
                break;

            case AIState.INVESTIGATING:
                OnInvestigatingUpdate();
                break;

            case AIState.INTERACT:
                OnInteractUpdate(closeEnough);
                break;

            case AIState.CHASE:
                OnChaseUpdate(closeEnough, closeToTarget);
                break;
        }
    }

    /// <summary>
    /// updates based on idle state
    /// </summary>
    void OnIdleUpdate(bool closeToTarget)
    {
        //haven't reached current patrol node: just keep going
        if (!closeToTarget && pathfinder && pathfinder.enabled)
        {
            pathfinder.destination = destination.position;
            timer = -1;
        }
        //reached current patrol node: take a break there
        else if (timer == -1)
        {
            AIPatrolPoint patrolPoint = patrolPoints[currPatrolPoint].GetComponent<AIPatrolPoint>();
            timer = patrolPoint.stopTime;
        }
        else
        {
            //at current patrol node: keep waiting
            timer -= updateInterval;
            //enought time passed: proceed to next patrol point
            if (timer <= 0)
            {
                currPatrolPoint = (currPatrolPoint + 1) % patrolPoints.Length;
                Idle();
            }
            //face patrol point direction if needed
            AIPatrolPoint patrolPoint = patrolPoints[currPatrolPoint].GetComponent<AIPatrolPoint>();
            if (patrolPoint && patrolPoint.faceDirection)
            {
                transform.rotation = patrolPoint.transform.rotation;
            }
        }
        investigateSoundPlayed = false;
        myBubble.StopInvestigating();
    }

    /// <summary>
    /// updates based on investigate state
    /// </summary>
    void OnInvestigateUpdate(bool closeEnough)
    {
        //haven't gotten to the point we want to investigate: just keep going
        if (!closeEnough && pathfinder && pathfinder.enabled)
        {
            pathfinder.destination = destination.position;
        }
        //reached point we want to investigate: switch states to INVESTIGATING
        else
        {
            if (pathfinder && pathfinder.enabled)
                pathfinder.destination = transform.position;
            state = AIState.INVESTIGATING;
            animator.SetTrigger("Investigate");
        }

        //manages playing the "hmm?" sound
        myBubble.Investigating();
        if (!investigateSoundPlayed)
        {
            investigateSoundPlayed = true;
            if (timeSinceLastOnInvestigate > 2)
            {
                onInvestigate.Post(gameObject);
                timeSinceLastOnInvestigate = 0;
            }
        }
    }

    /// <summary>
    /// updates based on investigating state
    /// </summary>
    void OnInvestigatingUpdate()
    {
        timer -= updateInterval;
        //finshed investigating the point: switch to INTERACT if we have something in queue, otherwise IDLE
        if (timer <= 0f)
        {
            CheckInteractQueue();
            wwiseComponent?.GiveUp();
        }

        //haven't finished investigating the point: manage playing the "hmm?" sound
        else
        {
            myBubble.Investigating();
            if (!investigateSoundPlayed)
            {
                investigateSoundPlayed = true;
                if (timeSinceLastOnInvestigate > 2)
                {
                    onInvestigate.Post(gameObject);
                    timeSinceLastOnInvestigate = 0;
                }
            }
        }
    }

    /// <summary>
    /// updates based on interact state
    /// </summary>
    void OnInteractUpdate(bool closeEnough)
    {
        //haven't reached what we want to interact with yet: keep going
        if (!closeEnough && pathfinder && pathfinder.enabled)
        {
            pathfinder.destination = destination.position;
        }

        //reached point:
        else
        {
            if (pathfinder && pathfinder.enabled)
                pathfinder.destination = transform.position;
            if (!reachedInteractable)
                wwiseComponent?.Fixing();
            reachedInteractable = true;
            //still haven't interacted with it for long enough: keep interacting
            if (timer >= 0 && thingsToInteractWith.Peek().NeedsInteraction())
            {
                timer -= updateInterval;
                //TODO: call interactable.AIInteracting(float progress)
            }
            //finished interacting with current interactable: fix it, dequeue it, then either move to next interactable in queue or IDLE if nothing left to interact with
            else
            {
                IAIInteractable interactable = thingsToInteractWith.Dequeue();
                if (interactable.NeedsInteraction())
                    interactable.AIFinishInteract(this);
                CheckInteractQueue();
            }
        }
        myBubble.StopInvestigating();
        investigateSoundPlayed = false;
    }

    /// <summary>
    /// updates based on chase state
    /// </summary>
    void OnChaseUpdate(bool closeEnough, bool closeToTarget)
    {
        //haven't reached player: keep going
        if (!closeToTarget && !closeEnough && pathfinder)
        {
            if (chaseBehavior != ChaseBehaviors.SEQUENCE)
                pathfinder.destination = destination.position;
        }
        //unable to reach player: give up
        else if (!closeToTarget && pathfinder)
        {
            LosePlayer(Player.current);
        }

        if (chaseBehavior == ChaseBehaviors.SUMMON)
        {
            SummonAI(Player.current.transform.position, 2.5f, true, chaseBroadcastInterests);
        }

        //throw shit at player if applicable
        if (thrower)
        {
            //new WaitForSeconds(2.0f);
            timer -= updateInterval;
            if (timer <= 0)
            {
                timer = throwWaitTime;
                thrower.Throw(Player.current.AISpotPoint.position);
            }

        }
        investigateSoundPlayed = false;
        myBubble.StopInvestigating();
    }

    /// <summary>
    /// runs when a state is switched off to a different state
    /// </summary>
    void OnStateExit(AIState state)
    {

    }

    /// <summary>
    /// interacts with the next object in the interact queue if there's something there, else idles
    /// </summary>
    void CheckInteractQueue()
    {
        if (thingsToInteractWith.Count > 0)
        {
            InteractWithNextInQueue();
        }
        else
        {
            Idle();
        }
        reachedInteractable = false;
    }

    /// <summary>
    /// interacts with the next object in the interact queue
    /// </summary>
    void InteractWithNextInQueue()
    {
        IAIInteractable newInteractable = thingsToInteractWith.Peek();
        timer = newInteractable.AIInteractTime() * interactSpeedMultiplier;
        destination = (newInteractable as MonoBehaviour).transform;
        stoppedTime = 0f;
        state = AIState.INTERACT;
    }

    private void OnDestroy()
    {
        activeAgents.Remove(this);
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (destination)
        {
            switch (state)
            {
                case AIState.IDLE:
                    Gizmos.color = Color.white;
                    break;
                case AIState.CHASE:
                    Gizmos.color = Color.red;
                    break;
                case AIState.INTERACT:
                    Gizmos.color = Color.green;
                    break;
                case AIState.INVESTIGATE:
                    Gizmos.color = Color.blue;
                    break;
                case AIState.INVESTIGATING:
                    Gizmos.color = Color.cyan;
                    break;
            }
            Gizmos.DrawLine(transform.position, destination.position);
        }
    }

#endif
}
