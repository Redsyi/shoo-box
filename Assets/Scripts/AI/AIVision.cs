using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIVision : MonoBehaviour
{
    public AIAgent ai;
    private Player player;
    int collidersTouchingPlayer;
    [Range(0, 15)]
    public float radius = 5;
    [Range(0, 360)]
    public float arc = 100;
    public Image visibleCone;
    public CapsuleCollider collider;
    public float viewFloor = 5f;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        if (ai == null)
            ai = GetComponentInParent<AIAgent>();
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
        if (transform.position.y - other.transform.position.y > viewFloor)
        {
            return;
        }
        Vector2 vectToItem = new Vector2(other.transform.position.x - transform.position.x, other.transform.position.z - transform.position.z).normalized;
        Vector2 forwardVect = new Vector2(transform.forward.x, transform.forward.z);
        float angleDiff = Mathf.Acos(Vector2.Dot(forwardVect, vectToItem))*Mathf.Rad2Deg;
        if (angleDiff > arc / 2)
        {
            return;
        }

        
        IAIInteractable interactable = other.gameObject.GetComponent<IAIInteractable>();
        if (interactable != null && interactable.NeedsInteraction())
        {
            foreach (AIInterest interest in interactable.InterestingToWhatAI())
            {
                if (System.Array.Exists<AIInterest>(ai.interests, element => element == interest))
                {
                    bool canSeeObj = !Physics.Raycast(transform.position, other.gameObject.transform.position - transform.position, (other.gameObject.transform.position - transform.position).magnitude, LayerMask.GetMask("Obstructions", "AI Blinders"));
                    if (canSeeObj)
                    {
                       
                        ai.Interact(interactable);
                    }
                    else
                    {
                       
                    }
                    break;
                }
            }
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
                Vector3 vectToPlayer = player.AISpotPoint.position - transform.position;
                bool canSeeObj = !AIAgent.blindAll && !Physics.Raycast(transform.position, vectToPlayer, vectToPlayer.magnitude, LayerMask.GetMask("Obstructions", "AI Blinders"));
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

    private void OnDrawGizmos()
    {
        (visibleCone.transform as RectTransform).sizeDelta = new Vector2(radius * 2, radius * 2);
        collider.radius = radius;
        visibleCone.transform.localEulerAngles = new Vector3(0, 0, -(360-arc) / 2f);
        visibleCone.fillAmount = arc / 360f;
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -viewFloor));
    }

    private void Update()
    {
        visibleCone.enabled = !AIAgent.blindAll;
    }
}
