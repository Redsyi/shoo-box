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
    public Transform handle;
    [Header("UI")]
    public Transform alertBarCanvas;
    public Image alertProgressBar;
    public RectTransform alertBarTSA;
    public Animator alertAnimator;
    public bool active = true;

    Vector3 alertCanvasOffset;
    Vector3 originalOffset;
    Quaternion originalRotation;
    bool caught;

    private void Start()
    {
        alertCanvasOffset = alertBarCanvas.transform.localPosition;
        originalOffset = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    public void AIFinishInteract(AIAgent ai)
    {
        if (handle && !caught)
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            if (rigidbody)
            {
                rigidbody.isKinematic = true;
            }

            transform.localRotation = originalRotation;
            transform.localPosition = originalOffset;
            handle.SetParent(ai.rightHand);
            handle.localPosition = Vector3.zero;
            caught = true;
            StartCoroutine(GetCarriedOut(ai));
        } else if (!handle && !player)
        {
            Destroy(gameObject);
        }

        if (player)
        {
            LevelBridge.Reload("Caught by the TSA");
        }
    }

    public void Activate()
    {
        active = true;
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
        return alertTimeRemaining <= 0 && !caught;
    }

    IEnumerator GetCarriedOut(AIAgent by)
    {
        ScriptedSequence carrySequence = by.GetComponent<ScriptedSequence>();
        if (carrySequence)
        {
            carrySequence.Trigger();
        }
        while (by.InSequence())
        {
            yield return null;
        }
        Destroy(gameObject);
    }

    void LateUpdate()
    {
        //manages the UI elements
        if (alertBarCanvas)
        {
            alertBarCanvas.transform.LookAt(CameraScript.current.camera.transform);
            alertBarCanvas.transform.position = transform.position + alertCanvasOffset;
            alertBarCanvas.gameObject.SetActive(alertProgress > 0.025f && !caught);
            if (alertProgressBar)
                alertProgressBar.fillAmount = alertProgress;
            if (alertBarTSA)
            {
                alertBarTSA.anchorMin = new Vector2(alertBarTSA.anchorMin.x, 1 - alertProgress);
                alertBarTSA.anchorMax = new Vector2(alertBarTSA.anchorMax.x, 1 - alertProgress);
                alertBarTSA.anchoredPosition = new Vector2(alertBarTSA.anchoredPosition.x, 0);
            }
        }

        if (alertTimeRemaining > 0 && active)
        {
            alertTimeRemaining -= Time.deltaTime;
            if (alertTimeRemaining <= 0 && !caught)
            {
                AIAgent.SummonAI(gameObject, 2, true, interestMask);
            }
        }

        alertAnimator.SetBool("Active", alertTimeRemaining <= 0);
    }
}
