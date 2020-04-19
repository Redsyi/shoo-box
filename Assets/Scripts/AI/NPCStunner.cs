using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStunner : MonoBehaviour, ISandalable
{
    public ParticleSystem stars;
    public TweetyBirds birds;
    public float stunDuration;

    float stunRemaining;
    AIAgent ai;

    private void Awake()
    {
        ai = GetComponentInParent<AIAgent>();
    }

    void Update()
    {
        if (stunRemaining > 0)
        {
            stunRemaining -= Time.deltaTime;
            if (stunRemaining <= 0)
            {
                stars.Stop();
                birds.Disable();
                ai.stunned = false;
                ai.Investigate(Player.current.transform.position, forceOverrideInteract: true);
            }
        }
    }

    public void HitBySandal()
    {
        if (stunRemaining <= 0)
        {
            stars.Play();
            birds.Enable();
            ai.stunned = true;
        }
        stunRemaining = stunDuration;
    }
}
