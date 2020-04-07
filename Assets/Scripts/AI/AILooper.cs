using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILooper : MonoBehaviour
{
    public Vector3 loopPosition;
    public AIInterest loopType;
    public float variation = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        AIAgent AI = other.GetComponentInParent<AIAgent>();
        if (AI && System.Array.Find(AI.interests, (AIInterest interest) => interest == loopType) != AIInterest.MAID)
        {
            Vector3 offset = AI.transform.position - transform.position;
            Vector3 newVect = transform.position + loopPosition + new Vector3(Random.Range(-variation, variation), 0, Random.Range(-variation, variation));
            AI.ChangePatrolPoint(0);
            AI.Idle();
            AI.pathfinder.Warp(newVect + offset);
            
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, .5f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + loopPosition, .5f);
    }
}
