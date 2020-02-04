using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVision : MonoBehaviour
{
    public Maid ai;

    private void OnTriggerEnter(Collider other)
    {
        ProcessItemInVision(other);
    }

    private void OnTriggerStay(Collider other)
    {
        ProcessItemInVision(other);
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player != null && ai.currState.state == AIState.CHASE)
        {
            ai.LosePlayer(player);
        }
    }

    private void ProcessItemInVision(Collider other)
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
            if (player != null && player.legForm || player.currentMovement != Vector3.zero)
            {
                bool canSeeObj = !Physics.Raycast(transform.position, other.gameObject.transform.position - transform.position, (other.gameObject.transform.position - transform.position).magnitude, LayerMask.GetMask("VisionObstruction"));
                if (canSeeObj)
                    ai.Chase(player);
                else if (ai.currState.state == AIState.CHASE)
                    ai.LosePlayer(player);
            }
        }
    }
}
