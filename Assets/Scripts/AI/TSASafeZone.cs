using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSASafeZone : MonoBehaviour
{
    public float radius = 1f;
    public SpriteRenderer indicator;
    public CapsuleCollider hitbox;
    private void OnTriggerEnter(Collider other)
    {
        TSAAlert alert = other.GetComponent<TSAAlert>();
        if (alert)
        {
            if (alert.player)
                indicator.color = Color.green;
            foreach (AIAgent AI in FindObjectsOfType<AIAgent>())
            {
                if (AI.currState.state == AIState.INVESTIGATE && AI.currState.location == alert.transform)
                {
                    AI.Investigate(alert.transform.position, 1.5f);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        TSAAlert alert = other.GetComponent<TSAAlert>();
        if (alert && alert.player)
        {
            indicator.color = Color.white;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        TSAAlert alert = other.GetComponent<TSAAlert>();
        if (alert)
        {
            alert.alertTimeRemaining = alert.alertTime;
        }
    }

    private void Start()
    {
        AdjustIndicator();
    }

    private void OnDrawGizmosSelected()
    {
        AdjustIndicator();
    }

    private void AdjustIndicator()
    {
        indicator.transform.localScale = new Vector3(radius / 5f, radius / 5f, 1);
        hitbox.radius = 5;// radius;
        hitbox.height = 10;
    }
}
