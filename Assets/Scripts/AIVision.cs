using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVision : MonoBehaviour
{
    public Maid ai;
    private HashSet<Collider> playerTouchers;
    int collidersTouchingPlayer;

    private void Start()
    {
        playerTouchers = new HashSet<Collider>();
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
                playerTouchers.Add(other);
            }
            if (player != null && (player.legForm || player.currentMovement != Vector3.zero))
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
            playerTouchers.Remove(other);
            collidersTouchingPlayer--;
            if (collidersTouchingPlayer == 0 && ai.currState.state == AIState.CHASE)
            {
                ai.LosePlayer(FindObjectOfType<Player>());
            }
        }
    }
}
