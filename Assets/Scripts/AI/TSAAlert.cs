using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TSAAlert : MonoBehaviour, IAIInteractable
{
    public float alertTime;
    public float alertTimeRemaining;
    public bool player;
    public AIInterest[] interestMask = { AIInterest.TSA };

    public void AIFinishInteract()
    {
        if (player)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        } else
        {
            Destroy(gameObject);
        }
    }

    public void AIInteracting(float interactProgress)
    {
    }

    public float AIInteractTime()
    {
        return 1;
    }

    public AIInterest[] InterestingToWhatAI()
    {
        return interestMask;
    }

    public bool NeedsInteraction()
    {
        return alertTimeRemaining <= 0;
    }

    void Update()
    {
        if (alertTimeRemaining > 0)
        {
            alertTimeRemaining -= Time.deltaTime;
            if (alertTimeRemaining <= 0)
            {
                AIAgent.SummonAI(gameObject, 1, interestMask);
            }
        }
    }
}
