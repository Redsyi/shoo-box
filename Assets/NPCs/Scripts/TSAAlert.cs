using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// class that represents an object that the TSA will go after if it isn't in a safe zone for a certain period of time
/// </summary>
public class TSAAlert : MonoBehaviour, IAIInteractable
{
    public float alertTime;
    public float alertTimeRemaining;
    public float alertProgress => (alertTime - alertTimeRemaining) / alertTime;
    public bool player;
    public AIInterest[] interestMask = { AIInterest.TSA };
    [Header("UI")]
    public Transform alertBarCanvas;
    public Image alertProgressBar;
    public RectTransform alertBarTSA;

    public void AIFinishInteract()
    {
        if (player)
        {
            LevelBridge.Reload("Caught by the TSA");
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
        return 2;
    }

    public AIInterest[] InterestingToWhatAI()
    {
        return interestMask;
    }

    public bool NeedsInteraction()
    {
        return alertTimeRemaining <= 0;
    }


    void LateUpdate()
    {
        //manages the UI elements
        if (alertBarCanvas)
        {
            alertBarCanvas.eulerAngles = new Vector3(0, CameraScript.current.transform.eulerAngles.y);
            alertBarCanvas.gameObject.SetActive(alertProgress > 0.025f);
            alertProgressBar.fillAmount = alertProgress;
            alertBarTSA.anchorMin = new Vector2(alertBarTSA.anchorMin.x, 1 - alertProgress);
            alertBarTSA.anchorMax = new Vector2(alertBarTSA.anchorMax.x, 1 - alertProgress);
            alertBarTSA.anchoredPosition = new Vector2(alertBarTSA.anchoredPosition.x, 0);
        }

        if (alertTimeRemaining > 0)
        {
            alertTimeRemaining -= Time.deltaTime;
            if (alertTimeRemaining <= 0)
            {
                AIAgent.SummonAI(gameObject, 2, true, interestMask);
            }
        }
    }
}
