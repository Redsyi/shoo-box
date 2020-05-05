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

    [Header("Behavior")]
    public Transform[] patrolPoints;
    public AIInterest[] interests;
    public bool deaf;
    [Tooltip("If there are any events specified here, they will be performed INSTEAD of the npc chasing the player")]
    public UnityEngine.Events.UnityEvent chaseOverride;
    public float stopDist = 1f;

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
    public AIStateNode currState;       //the current ai state
    public Queue<IAIInteractable> thingsToInteractWith; //current list of items the agent has detected need interaction
    float timer;        //general timer variable used across all states
    [HideInInspector] public bool investigatingPlayer;

    void Start()
    {
        blindAll = false;
        currState = new AIStateNode();
        thingsToInteractWith = new Queue<IAIInteractable>();
        Idle();
        
        GameObject bubbleCanvas = GameObject.FindGameObjectWithTag("NPC Bubble Canvas");
        if (bubbleCanvas == null)
            Debug.LogError("AIAgent couldn't find bubble canvas");
        myBubble = Instantiate(bubblePrefab, bubbleCanvas.transform);
        myBubble.worldAnchor = bubbleAnchor;

        StartCoroutine(CheckPos());
        wwiseComponent = GetComponent<AKEventNPC>();
        shoeSightColoring = GetComponent<CustomShoeSight>();
        if (shoeSightColoring)
        {
            originalSightColoring = shoeSightColoring.type;
        }
        sequences = GetComponents<ScriptedSequence>();

        AudioManager.RegisterAgent(this);
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
                if (stopped && currState.state != AIState.IDLE)
                {
                    Face(currState.location.position);
                }
            }
            prevPos = transform.position;
            yield return new WaitForSeconds(.2f);
        }
     
    }

    /// <summary>
    /// returns if this npc is currently executing a sequence
    /// </summary>
    bool InSequence()
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
        if (currState.state != AIState.IDLE || patrolPoints.Length > 1 || !currState.location)
        { 
            stoppedTime = 0f;
            currState.state = AIState.IDLE;
            currState.location = patrolPoints[currPatrolPoint];
            investigatingPlayer = false;
        }
    }

    /// <summary>
    /// Tell the AI to investigate an object
    /// </summary>
    /// <param name="location">Transform to investigate</param>
    /// <param name="forceOverrideChase">Whether this trigger can interrupt a chase state</param>
    public void Investigate(GameObject location, float investigateTime = 3f, bool forceOverrideChase = false, bool forceOverrideInteract = true)
    {
        if ((currState.state != AIState.CHASE || forceOverrideChase) && (currState.state != AIState.INTERACT || forceOverrideInteract))
        {
            stoppedTime = 0f;
            currState.state = AIState.INVESTIGATE;
            currState.location = location.transform;
            if (currState.state != AIState.INVESTIGATE)
                wwiseComponent?.StartedInvestigation();
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

            if (currState.state != AIState.CHASE && currState.state != AIState.INTERACT && thingsToInteractWith.Count == 1)
            {
                wwiseComponent?.SomethingWrong();
                currState.state = AIState.INTERACT;
                currState.location = (interactable as MonoBehaviour).transform;
                timer = interactable.AIInteractTime() * interactSpeedMultiplier;
                stoppedTime = 0f;
                reachedInteractable = false;
                investigatingPlayer = false;
            }
        }
    }

    /// <summary>
    /// Chase the player
    /// </summary>
    /// <param name="player"></param>
    public void Chase(Player player)
    {
        if (currState.state != AIState.CHASE)
        {
            wwiseComponent?.PlayerSpotted();
            myBubble.Spotted();
            if (timeSinceLastOnSpot > 2)
            {
                onSpot.Post(gameObject);
                timeSinceLastOnSpot = 0;
            }
            player.npcsChasing++;
            timer = 0f;
            chaseOverride.Invoke();
        }
        if (chaseOverride.GetPersistentEventCount() == 0 && InSequence())
        {
            foreach (ScriptedSequence sequence in sequences)
            {
                if (sequence.running)
                {
                    sequence.Interrupt();
                }
            }
        }
        stoppedTime = 0f;
        currState.state = AIState.CHASE;
        currState.location = player.transform;
        investigatingPlayer = false;
    }

    /// <summary>
    /// Notify the AI that is has lost sight of the player
    /// </summary>
    public void LosePlayer(Player player)
    {
        if (currState.state == AIState.CHASE)
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
        if (chaseOverride.GetPersistentEventCount() == 0)
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


    //this is a big boi function and should probably be a coroutine instead but whatever
    void Update()
    {
        timeSinceLastOnSpot += Time.deltaTime;
        timeSinceLastOnInvestigate += Time.deltaTime;

        if (stopped)
            stoppedTime += Time.deltaTime;
        else
            stoppedTime = 0f;

        //being in a sequence will temporarily stop this behavior
        if (!InSequence())
        {
            //handles our current target being unexpectedly destroyed
            if (currState.state != AIState.IDLE && !currState.location)
            {
                if (currState.state == AIState.INTERACT)
                {
                    IAIInteractable invalid = thingsToInteractWith.Dequeue();
                    while (thingsToInteractWith.Count > 0)
                    {
                        IAIInteractable newInteractable = thingsToInteractWith.Peek();
                        if (newInteractable as MonoBehaviour)
                        {
                            timer = newInteractable.AIInteractTime() * interactSpeedMultiplier;
                            currState.state = AIState.INTERACT;
                            currState.location = (newInteractable as MonoBehaviour).transform;
                            stoppedTime = 0f;
                            reachedInteractable = false;
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

            //detects whether we are close to our target; or if we tried to reach the target but ended up stopped (which is enough for some states)
            bool closeToTarget = (transform.position - currState.location.position).sqrMagnitude < (stopDist * stopDist);
            bool closeEnough = (reachedInteractable && currState.state == AIState.INTERACT) || (stoppedTime >= giveUpTime) || closeToTarget;

            //manages npc speed depending on state
            if (pathfinder)
                pathfinder.speed = (stunned ? 0f : (currState.state == AIState.CHASE || currState.state == AIState.INTERACT ? runSpeed : walkSpeed));

            //manages npc animations
            animator.SetBool("Moving", stoppedTime < 0.1f && !stunned);
            animator.SetBool("Running", currState.state == AIState.CHASE || currState.state == AIState.INTERACT);
            animator.SetBool("Interacting", currState.state == AIState.INTERACT && closeEnough && !stunned);

            if (debug)
                print($"{gameObject.name}: {currState.state}, {currState.location}, {thingsToInteractWith.Count} | {stoppedTime}");

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

            //take action depending on the current state
            switch (currState.state)
            {
                case AIState.IDLE:
                    //haven't reached current patrol node: just keep going
                    if (!closeToTarget && pathfinder && pathfinder.enabled)
                    {
                        pathfinder.destination = currState.location.position;
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
                        timer -= Time.deltaTime;
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
                    break;

                case AIState.INVESTIGATE:
                    //haven't gotten to the point we want to investigate: just keep going
                    if (!closeEnough && pathfinder && pathfinder.enabled)
                    {
                        pathfinder.destination = currState.location.position;
                    }
                    //reached point we want to investigate: switch states to INVESTIGATING
                    else
                    {
                        if (pathfinder && pathfinder.enabled)
                            pathfinder.destination = transform.position;
                        currState.state = AIState.INVESTIGATING;
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
                    break;

                case AIState.INVESTIGATING:
                    timer -= Time.deltaTime;
                    //finshed investigating the point: switch to INTERACT if we have something in queue, otherwise IDLE
                    if (timer <= 0f)
                    {
                        if (thingsToInteractWith.Count > 0)
                        {
                            IAIInteractable newInteractable = thingsToInteractWith.Peek();
                            timer = newInteractable.AIInteractTime() * interactSpeedMultiplier;
                            currState.state = AIState.INTERACT;
                            currState.location = (newInteractable as MonoBehaviour).transform;
                            stoppedTime = 0f;
                            reachedInteractable = false;
                        }
                        else
                        {
                            wwiseComponent?.GiveUp();
                            Idle();
                        }
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
                    break;

                case AIState.INTERACT:
                    //haven't reached what we want to interact with yet: keep going
                    if (!closeEnough && pathfinder && pathfinder.enabled)
                    {
                        pathfinder.destination = currState.location.position;
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
                            timer -= Time.deltaTime;
                            //TODO: call interactable.AIInteracting(float progress)
                        }
                        //finished interacting with current interactable: fix it, dequeue it, then either move to next interactable in queue or IDLE if nothing left to interact with
                        else
                        {
                            IAIInteractable interactable = thingsToInteractWith.Dequeue();
                            if (interactable.NeedsInteraction())
                                interactable.AIFinishInteract();
                            if (thingsToInteractWith.Count > 0)
                            {
                                IAIInteractable newInteractable = thingsToInteractWith.Peek();
                                timer = newInteractable.AIInteractTime() * interactSpeedMultiplier;
                                currState.location = (newInteractable as MonoBehaviour).transform;
                                stoppedTime = 0f;
                            }
                            else
                            {
                                Idle();
                            }
                            reachedInteractable = false;
                        }
                    }
                    myBubble.StopInvestigating();
                    investigateSoundPlayed = false;
                    break;

                case AIState.CHASE:
                    //haven't reached player: keep going
                    if (!closeToTarget && !closeEnough && pathfinder)
                    {
                        if (chaseOverride.GetPersistentEventCount() == 0)
                            pathfinder.destination = currState.location.position;
                    }
                    //unable to reach player: give up
                    else if (!closeToTarget && pathfinder)
                    {
                        LosePlayer(Player.current);
                    }
                    //throw shit at player if applicable
                    if (thrower)
                    {
                        //new WaitForSeconds(2.0f);
                        timer -= Time.deltaTime;
                        if (timer <= 0)
                        {
                            timer = throwWaitTime;
                            thrower.Throw(Player.current.AISpotPoint.position);
                        }

                    }
                    investigateSoundPlayed = false;
                    myBubble.StopInvestigating();
                    break;
            }
        }
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
        foreach (AIAgent agent in FindObjectsOfType<AIAgent>())
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

    /// <summary>
    /// summons all npcs with the given interests to a specific location
    /// </summary>
    public static void SummonAI(Vector3 location, float investigateTime, bool interruptInteract = false, params AIInterest[] interests)
    {
        foreach (AIAgent agent in FindObjectsOfType<AIAgent>())
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

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (currState != null && currState.location)
        {
            switch (currState.state)
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
            Gizmos.DrawLine(transform.position, currState.location.position);
        }
    }

#endif
}
