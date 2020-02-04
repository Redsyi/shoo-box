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

    void Start()
    {
        currState = new AIStateNode();
        thingsToInteractWith = new Queue<IAIInteractable>();
        Idle();
    }

    public void Idle()
    {   currState.state = AIState.IDLE;
        currState.location = patrolPoint;
    }

    public void Investigate(GameObject location)
    {
        currState.state = AIState.INVESTIGATE;
        currState.location = location.transform;
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
            }
        }
    }

    public void Chase(Player player)
    {
        currState.state = AIState.CHASE;
        currState.location = player.transform;
    }

    public void LosePlayer(Player player)
    {
        GameObject target = Instantiate(targetPrefab, player.transform.position, Quaternion.identity);
        Investigate(target);
    }
    
    void Update()
    {
        bool closeToTarget = (transform.position - currState.location.position).sqrMagnitude < 1f;
        if (!closeToTarget)
        {
            transform.LookAt(currState.location);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }

        print(currState.state + ", " + currState.location + ", " + thingsToInteractWith.Count);

        switch(currState.state)
        {
            case AIState.IDLE:
                if (!closeToTarget)
                {
                    pathfinder.destination = currState.location.position;
                }
                break;
            case AIState.INVESTIGATE:
                if (!closeToTarget)
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
                    }
                    else
                    {
                        Idle();
                    }
                }
                break;
            case AIState.INTERACT:
                if (!closeToTarget)
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
