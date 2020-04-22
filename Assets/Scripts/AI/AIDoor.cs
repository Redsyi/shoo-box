using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDoor : MonoBehaviour
{
    private bool open;
    private bool animating;
    public float openRate;
    public float closedAngle;
    public float openAngle;
    private float currAngle;
    private int openDir;
    public Transform hinge;
    public AK.Wwise.Event doorOpenClip;
    public AIState[] openRequiredStates;
    public AIState[] closeRequiredStates;
    public AIInterest[] openFor;
    public AIInterest[] closeFor;
    private AIAgent currAI;
    private bool checkAIExists;

    private void Start()
    {
        openDir = (openAngle - closedAngle > 0 ? 1 : -1);
    }

    public void OnTriggerEnter(Collider other)
    {
        AIAgent ai = other.gameObject.GetComponentInParent<AIAgent>();
        if (ai && (openRequiredStates.Length == 0 || System.Array.Exists(openRequiredStates, item => item == ai.currState.state)))
        {
            if (openFor == null || openFor.Length == 0 || System.Array.Exists(openFor, interest => System.Array.Exists(ai.interests, aiinterest => aiinterest == interest)))
            {
                currAI = ai;
                checkAIExists = true;
                Open();
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        AIAgent ai = other.gameObject.GetComponentInParent<AIAgent>();
        if (ai)
        {
            currAI = null;
            checkAIExists = false;
        }
        if (ai && (closeRequiredStates.Length == 0 || System.Array.Exists(closeRequiredStates, item => item == ai.currState.state)))
        {
            if (closeFor == null || closeFor.Length == 0 || System.Array.Exists(closeFor, interest => System.Array.Exists(ai.interests, aiinterest => aiinterest == interest)))
                Close();
        }
    }

    private void Update()
    {
        if (open && checkAIExists && !currAI)
        {
            Close();
            checkAIExists = false;
        }
    }

    public void Open()
    {
        if (!open)
        {
            StartCoroutine(OpenDoor());
        }
    }

    public void Close()
    {
        if (open)
        {
            StartCoroutine(CloseDoor());
        }
    }

    IEnumerator OpenDoor()
    {
        while (animating)
            yield return null;
        doorOpenClip.Post(gameObject);
        animating = true;
        while (openDir > 0 ? currAngle < openAngle : currAngle > openAngle)
        {
            currAngle += Time.deltaTime * openRate * openDir;
            hinge.localEulerAngles = new Vector3(0, currAngle, 0);
            yield return null;
        }
        currAngle = openAngle;
        hinge.localEulerAngles = new Vector3(0, currAngle, 0);
        open = true;
        animating = false;
    }

    IEnumerator CloseDoor()
    {
        while (animating)
            yield return null;
        doorOpenClip.Post(gameObject);
        animating = true;
        while (openDir < 0 ? currAngle < closedAngle : currAngle > closedAngle)
        {
            currAngle -= Time.deltaTime * openRate * openDir;
            hinge.localEulerAngles = new Vector3(0, currAngle, 0);
            yield return null;
        }
        currAngle = closedAngle;
        hinge.localEulerAngles = new Vector3(0, currAngle, 0);
        open = false;
        animating = false;
    }
}
