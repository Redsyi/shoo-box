using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class AIAgent : MonoBehaviour
{
    public Transform[] patrolPoints;
    public Animator animator;
    public AIStateNode currState;
    public NavMeshAgent pathfinder;
    public Queue<IAIInteractable> thingsToInteractWith;
    float timer;
    public GameObject targetPrefab;
    public UINPCBubble bubblePrefab;
    public Transform bubbleAnchor;
    private UINPCBubble myBubble;
    private GameObject instantiatedTarget;
    private bool stopped;
    private Vector3 prevPos;
    private float stoppedTime = 0f;
    private const float giveUpTime = 1f;
    public bool debug;
    public float walkSpeed;
    public float runSpeed;
    public AIInterest[] interests;
    public bool beingRepelled;
    public static bool blindAll;
    public bool deaf;
    private bool reachedInteractable;
    private AKEventNPC wwiseComponent;
    private int currPatrolPoint;

    void Start()
    {
        blindAll = false;
        currState = new AIStateNode();
        thingsToInteractWith = new Queue<IAIInteractable>();
        Idle();

        if (pathfinder == null)
            Debug.LogError("AIAgent has null NavMeshAgent!");

        GameObject bubbleCanvas = GameObject.FindGameObjectWithTag("NPC Bubble Canvas");
        if (bubbleCanvas == null)
            Debug.LogError("AIAgent couldn't find bubble canvas");
        myBubble = Instantiate(bubblePrefab, bubbleCanvas.transform);
        myBubble.worldAnchor = bubbleAnchor;

        StartCoroutine(CheckPos());
        wwiseComponent = GetComponent<AKEventNPC>();
    }

    IEnumerator CheckPos()
    {
        while (true)
        {
            stopped = (prevPos - transform.position).sqrMagnitude <= walkSpeed*.05f;
            /*if (!stopped)
            {
                transform.LookAt(transform.position + (transform.position - prevPos));
            }
            else */if (stopped && currState.state != AIState.IDLE)
            {
                transform.LookAt(currState.location);
                transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y);
            }
            prevPos = transform.position;
            yield return new WaitForSeconds(.2f);
        }
     
    }

    /// <summary>
    /// Tell the AI to go to idle mode
    /// </summary>
    public void Idle()
    {
        if (currState.state != AIState.IDLE || patrolPoints.Length > 1 || !currState.location)
        {
            pathfinder.speed = walkSpeed;
            stoppedTime = 0f;
            currState.state = AIState.IDLE;
            currState.location = patrolPoints[currPatrolPoint];
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
            pathfinder.speed = walkSpeed;
            stoppedTime = 0f;
            currState.state = AIState.INVESTIGATE;
            currState.location = location.transform;
            wwiseComponent?.StartedInvestigation();
            timer = investigateTime;
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
                pathfinder.speed = runSpeed;
                currState.state = AIState.INTERACT;
                currState.location = (interactable as MonoBehaviour).transform;
                timer = interactable.AIInteractTime();
                stoppedTime = 0f;
                reachedInteractable = false;
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
            player.npcsChasing++;
        }
        pathfinder.speed = runSpeed;
        stoppedTime = 0f;
        currState.state = AIState.CHASE;
        currState.location = player.transform;
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
        Investigate(instantiatedTarget, forceOverrideChase: true);
    }

    public void CatchPlayer(Player player)
    {
        //this is the part where the player fucking dies
        wwiseComponent?.PlayerCaught();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    void Update()
    {
        if (stopped)
            stoppedTime += Time.deltaTime;
        else
            stoppedTime = 0f;

        if (currState.state != AIState.IDLE && !currState.location)
        {
            if (currState.state == AIState.INTERACT)
            {
                IAIInteractable invalid = thingsToInteractWith.Dequeue();
                while (thingsToInteractWith.Count > 0)
                {
                    IAIInteractable newInteractable = thingsToInteractWith.Peek();
                    if (newInteractable as MonoBehaviour) {
                        timer = newInteractable.AIInteractTime();
                        currState.state = AIState.INTERACT;
                        currState.location = (newInteractable as MonoBehaviour).transform;
                        stoppedTime = 0f;
                        reachedInteractable = false;
                        break;
                    } else
                    {
                        thingsToInteractWith.Dequeue();
                    }
                }
                if (thingsToInteractWith.Count == 0)
                    Idle();
            }
        }
        bool closeToTarget = (transform.position - currState.location.position).sqrMagnitude < 0.4f;
        bool closeEnough = (reachedInteractable && currState.state == AIState.INTERACT) || (stoppedTime >= giveUpTime) || closeToTarget;
        
        animator.SetBool("Moving", stoppedTime < 0.1f);
        animator.SetBool("Running", currState.state == AIState.CHASE || currState.state == AIState.INTERACT);
        animator.SetBool("Interacting", currState.state == AIState.INTERACT && closeEnough);

        if (debug)
            print($"{gameObject.name}: {currState.state}, {currState.location}, {thingsToInteractWith.Count} | {stoppedTime}");

        //take action depending on the current state
        switch(currState.state)
        {
            case AIState.IDLE:
                if (!closeEnough)
                {
                    pathfinder.destination = currState.location.position;
                    timer = -1;
                } else if (timer == -1)
                {
                    AIPatrolPoint patrolPoint = patrolPoints[currPatrolPoint].GetComponent<AIPatrolPoint>();
                    timer = patrolPoint.stopTime;
                } else
                {
                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        currPatrolPoint = (currPatrolPoint + 1) % patrolPoints.Length;
                        Idle();
                    }
                    AIPatrolPoint patrolPoint = patrolPoints[currPatrolPoint].GetComponent<AIPatrolPoint>();
                    if (patrolPoint.faceDirection)
                    {
                        transform.rotation = patrolPoint.transform.rotation;
                    }
                }
                myBubble.StopInvestigating();
                break;
            case AIState.INVESTIGATE:
                if (!closeEnough)
                {
                    pathfinder.destination = currState.location.position;
                } else
                {
                    currState.state = AIState.INVESTIGATING;
                    animator.SetTrigger("Investigate");
                }
                myBubble.Investigating();
                break;
            case AIState.INVESTIGATING:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (thingsToInteractWith.Count > 0)
                    {
                        IAIInteractable newInteractable = thingsToInteractWith.Peek();
                        timer = newInteractable.AIInteractTime();
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
                else
                {
                    myBubble.Investigating();
                }
                break;
            case AIState.INTERACT:
                if (!closeEnough)
                {
                    pathfinder.destination = currState.location.position;
                } else
                {
                    if (!reachedInteractable)
                        wwiseComponent?.Fixing();
                    reachedInteractable = true;
                    if (timer >= 0)
                    {
                        timer -= Time.deltaTime;
                        //TODO: call interactable.AIInteracting(float progress)
                    } else
                    {
                        IAIInteractable interactable = thingsToInteractWith.Dequeue();
                        interactable.AIFinishInteract();
                        if (thingsToInteractWith.Count > 0)
                        {
                            IAIInteractable newInteractable = thingsToInteractWith.Peek();
                            timer = newInteractable.AIInteractTime();
                            currState.location = (newInteractable as MonoBehaviour).transform;
                            stoppedTime = 0f;
                        } else
                        {
                            Idle();
                        }
                        reachedInteractable = false;
                    }
                }
                myBubble.StopInvestigating();
                break;
            case AIState.CHASE:
                if (!closeToTarget && !closeEnough)
                {
                    pathfinder.destination = currState.location.position;
                }
                else if (!closeToTarget)
                {
                    LosePlayer(FindObjectOfType<Player>());
                }
                myBubble.StopInvestigating();
                break;
        }
    }


    public static void SummonAI(GameObject to, float investigateTime, params AIInterest[] interests)
    {
        foreach (AIAgent agent in FindObjectsOfType<AIAgent>())
        {
            foreach (AIInterest interest in interests)
            {
                if (System.Array.Exists<AIInterest>(agent.interests, element => element == interest))
                {
                    agent.Investigate(to, investigateTime);
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
