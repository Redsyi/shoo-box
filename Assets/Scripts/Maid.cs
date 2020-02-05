using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Maid : MonoBehaviour
{
    public Transform patrolPoint;
    public AIStateNode currState;
    public NavMeshAgent pathfinder;
    public Queue<IAIInteractable> thingsToInteractWith;
    float timer;
    public GameObject targetPrefab;
    private bool stopped;
    private Vector3 prevPos;
    private float stoppedTime = 0f;
    private const float giveUpTime = 1f;
    public bool debug;

    void Start()
    {
        currState = new AIStateNode();
        thingsToInteractWith = new Queue<IAIInteractable>();
        Idle();
    }

    private void FixedUpdate()
    {
        stopped = prevPos == transform.position;
        prevPos = transform.position;
    }

    public void Idle()
    {
        stoppedTime = 0f;
        currState.state = AIState.IDLE;
        currState.location = patrolPoint;
    }

    public void Investigate(GameObject location, bool forceOverrideChase = false)
    {
        if (currState.state != AIState.CHASE || forceOverrideChase)
        {
            stoppedTime = 0f;
            currState.state = AIState.INVESTIGATE;
            currState.location = location.transform;
        }
    }

    public void Interact(IAIInteractable interactable)
    {
        if (!thingsToInteractWith.Contains(interactable))
        {
            thingsToInteractWith.Enqueue(interactable);
            if (currState.state != AIState.CHASE)
            {
                currState.state = AIState.INTERACT;
                currState.location = (interactable as MonoBehaviour).transform;
                timer = interactable.AIInteractTime();
                stoppedTime = 0f;
            }
        }
    }

    public void Chase(Player player)
    {
        stoppedTime = 0f;
        currState.state = AIState.CHASE;
        currState.location = player.transform;
    }

    public void LosePlayer(Player player)
    {
        GameObject target = Instantiate(targetPrefab, player.transform.position, Quaternion.identity);
        Investigate(target, true);
    }
    
    void Update()
    {
        if (stopped)
            stoppedTime += Time.deltaTime;
        else
            stoppedTime = 0f;

        bool closeToTarget = (transform.position - currState.location.position).sqrMagnitude < 0.4f;
        bool closeEnough = (stoppedTime >= giveUpTime) || closeToTarget;
        if (!closeToTarget)
        {
            transform.LookAt(currState.location);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }

        if (debug)
            print(currState.state + ", " + currState.location + ", " + thingsToInteractWith.Count + " | " + stoppedTime);

        switch(currState.state)
        {
            case AIState.IDLE:
                if (!closeEnough)
                {
                    pathfinder.destination = currState.location.position;
                }
                break;
            case AIState.INVESTIGATE:
                if (!closeEnough)
                {
                    pathfinder.destination = currState.location.position;
                } else
                {
                    currState.state = AIState.INVESTIGATING;
                    timer = 3;
                }
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
                    }
                    else
                    {
                        Idle();
                    }
                }
                break;
            case AIState.INTERACT:
                if (!closeEnough)
                {
                    pathfinder.destination = currState.location.position;
                } else
                {
                    if (timer >= 0)
                    {
                        timer -= Time.deltaTime;
                    } else
                    {
                        IAIInteractable interactable = thingsToInteractWith.Dequeue();
                        interactable.AIInteract();
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
                    }
                }
                break;
            case AIState.CHASE:
                if (!closeToTarget)
                {
                    pathfinder.destination = currState.location.position;
                } else
                {
                    SceneManager.LoadScene(0);
                }
                break;
        }
    }
}
