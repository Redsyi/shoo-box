using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// class that manages the sight for an NPC
/// </summary>
public class AIVision : MonoBehaviour
{
    [Header("Components")]
    public AIAgent ai;
    public Image visibleCone;
    public CapsuleCollider collider;
    [Header("Stats")]
    [Range(0, 15)]
    [Tooltip("Radius of this NPC's sight")]
    public float radius = 5;
    [Range(0, 360)]
    [Tooltip("View arc (in degrees)")]
    public float arc = 100;
    [Tooltip("NPC won't see further than this below them")]
    public float viewFloor = 5f;
    [Tooltip("How long it takes to spot the player standing up")]
    public float standingSpotTime = 0.2f;
    [Tooltip("How long it takes to spot the player shuffling")]
    public float shuffleSpotTime = 0.7f;
    [Tooltip("How quickly the spot progress decays")]
    public float spotDecay = 1.2f;
    [Tooltip("Where the NPC head is relative to this object - raycasts will originate here")]
    public Vector3 headOffset;

    private bool playerInVision;
    private Player player;
    Vector3 eyePosition => transform.position + headOffset;

    private void Start()
    {
        player = Player.current;
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

    /// <summary>
    /// processes an item that is in the vision collider
    /// </summary>
    private void ProcessItemInVision(Collider other, bool justEntered = false)
    {
        //check if below view floor - if so abort
        if (transform.position.y - other.transform.position.y > viewFloor)
        {
            return;
        }

        //check if inside arc - if not abort
        Vector2 vectToItem = new Vector2(other.transform.position.x - transform.position.x, other.transform.position.z - transform.position.z).normalized;
        Vector2 forwardVect = new Vector2(transform.forward.x, transform.forward.z);
        float angleDiff = Mathf.Acos(Vector2.Dot(forwardVect, vectToItem))*Mathf.Rad2Deg;
        if (angleDiff > arc / 2)
        {
            if (other.GetComponentInParent<Player>())
            {
                playerInVision = false;
            }
            return;
        }

        
        //see if we found an interactable object
        IAIInteractable interactable = other.gameObject.GetComponent<IAIInteractable>();
        //make sure it wants to be interacted with
        if (interactable != null && interactable.NeedsInteraction())
        {
            //make sure it wants us specifically to interact with it
            foreach (AIInterest interest in interactable.InterestingToWhatAI())
            {
                if (System.Array.Exists<AIInterest>(ai.interests, element => element == interest))
                {
                    //make sure nothing is blocking our vision to it
                    bool canSeeObj = !Physics.Raycast(eyePosition, other.gameObject.transform.position - eyePosition, (other.gameObject.transform.position - eyePosition).magnitude, LayerMask.GetMask("Obstructions", "AI Blinders"));
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

        //see if we found the player
        else if (interactable == null)
        {
            Player player = other.GetComponentInParent<Player>();

            //make sure the player isn't completely stealthed
            if (player != null && (player.legForm || player.moving))
            {
                playerInVision = false;

                Vector3 vectToPlayer = player.AISpotPoint.position - eyePosition;

                //make sure we have unblocked line-of-sight
                bool canSeeObj = !AIAgent.blindAll && !Physics.Raycast(eyePosition, vectToPlayer, vectToPlayer.magnitude, LayerMask.GetMask("Obstructions", "AI Blinders"));
                if (canSeeObj)
                {
                    //increase stealth meter, if full, chase the player
                    playerInVision = true;
                    ai.spotProgress += Time.fixedDeltaTime / (player.legForm ? standingSpotTime : shuffleSpotTime);
                    if (ai.spotProgress == 1f)
                        ai.Chase(player);
                }

                //lose the player if line of sight became blocked
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
            playerInVision = false;
            if (ai.currState.state == AIState.CHASE)
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
        
        if(!playerInVision) // Only decay if the player isn't in the vision
            ai.spotProgress -= Time.deltaTime / spotDecay;
    }
}
