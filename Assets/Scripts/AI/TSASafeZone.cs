﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSASafeZone : MonoBehaviour
{
    public float radius = 1f;
    public SpriteRenderer indicator;
    public SpriteRenderer filledIndicator;
    public CapsuleCollider hitbox;
    public Color normalColor;
    public Color normalFilledColor;
    public Color safeColor;
    public Color safeFilledColor;

    AIAgent[] activeAgents;

    private void OnTriggerEnter(Collider other)
    {
        TSAAlert alert = other.GetComponent<TSAAlert>();
        if (alert)
        {
            if (alert.player)
            {
                indicator.color = safeColor;
                filledIndicator.color = safeFilledColor;
            }
            foreach (AIAgent AI in activeAgents)
            {
                if (AI && AI.currState.state == AIState.INVESTIGATE && AI.currState.location == alert.transform)
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
            indicator.color = normalColor;
            filledIndicator.color = normalFilledColor;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        TSAAlert alert = other.GetComponent<TSAAlert>();
        if (alert)
        {
            foreach (AIAgent AI in activeAgents)
            {
                if (AI && AI.currState.state == AIState.INTERACT && AI.currState.location == alert.transform)
                {
                    return;
                }
            }
            alert.alertTimeRemaining = alert.alertTime;
        }
    }

    private void Start()
    {
        AdjustIndicator();
        indicator.color = normalColor;
        filledIndicator.color = normalFilledColor;
        activeAgents = FindObjectsOfType<AIAgent>();
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
