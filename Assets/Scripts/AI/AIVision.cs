using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVision : MonoBehaviour
{
    public AIAgent ai;
    private Player player;
    int collidersTouchingPlayer;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        ProcessItemInVision(other, true);
    }

    private void OnTriggerStay(Collider other)
    {
        ProcessItemInVision(other);
    }

    private void ProcessItemInVision(Collider other, bool justEntered = false)
    {
        IAIInteractable interactable = other.GetComponent<IAIInteractable>();
        if (interactable != null && interactable.NeedsInteraction())
        {
            bool canSeeObj = !Physics.Raycast(transform.position, other.gameObject.transform.position - transform.position, (other.gameObject.transform.position - transform.position).magnitude, LayerMask.GetMask("VisionObstruction"));
            if (canSeeObj)
                ai.Interact(interactable);
        }
        else if (interactable == null)
        {
            Player player = other.GetComponentInParent<Player>();
            if (player != null)
            {
                if (justEntered)
                    collidersTouchingPlayer++;
            }
            if (player != null && (player.legForm || player.moving))
            {
                bool canSeeObj = !Physics.Raycast(transform.position, other.gameObject.transform.position - transform.position, (other.gameObject.transform.position - transform.position).magnitude, LayerMask.GetMask("VisionObstruction"));
                if (canSeeObj)
                {
                    ai.Chase(player);
                }
                else if (ai.currState.state == AIState.CHASE)
                    ai.LosePlayer(player);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player != null)
        {
            collidersTouchingPlayer--;
            if (collidersTouchingPlayer == 0 && ai.currState.state == AIState.CHASE)
            {
                ai.LosePlayer(player);
            }
        }
    }
}
