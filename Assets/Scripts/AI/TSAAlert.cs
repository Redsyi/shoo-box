using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TSAAlert : MonoBehaviour, IAIInteractable
{
    public float alertTime;
    public float alertTimeRemaining;
    public bool player;
    public AIInterest[] interestMask = { AIInterest.TSA };
    [Header("UI")]
    public Transform alertBarCanvas;
    public Image alertProgressBar;

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
        if (alertBarCanvas)
        {
            alertBarCanvas.eulerAngles = new Vector3(0, CameraScript.current.transform.eulerAngles.y + 45);
            float alertProgress = (alertTime - alertTimeRemaining) / alertTime;
            alertBarCanvas.gameObject.SetActive(alertProgress > 0.01f);
            alertProgressBar.fillAmount = alertProgress;
        }

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
