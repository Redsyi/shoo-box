using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that opens a door for NPCs that enter
/// </summary>
public class AIDoor : MonoBehaviour
{
    private bool open;
    private bool animating;
    public float openRate;
    public float closedAngle;
    public float openAngle;
    [Tooltip("Time the AI is waiting at the door")]
    public float waitTime;
    private float currAngle;
    private int openDir;
    public Transform hinge;
    public AK.Wwise.Event doorOpenClip;
    [Tooltip("Will only open for AIs in the following states (leave empty for any)")]
    public AIState[] openRequiredStates;
    [Tooltip("Will only close for AIs in the following states (leave empty for any)")]
    public AIState[] closeRequiredStates;
    [Tooltip("Will only open for AIs with the following interests (leave empty for any)")]
    public AIInterest[] openFor;
    [Tooltip("Will only close for AIs with the following interests (leave empty for any)")]
    public AIInterest[] closeFor;
    private AIAgent currAI;
    private bool checkAIExists;
    public GameObject[] lockedIcons;
    public GameObject[] unlockedIcons;

    private void Start()
    {
        openDir = (openAngle - closedAngle > 0 ? 1 : -1);
    }

    public void OnTriggerEnter(Collider other)
    {
        AIAgent ai = other.gameObject.GetComponentInParent<AIAgent>();
        if (ai && (openRequiredStates.Length == 0 || System.Array.Exists(openRequiredStates, item => item == ai.state)))
        {
            if (openFor == null || openFor.Length == 0 || System.Array.Exists(openFor, interest => System.Array.Exists(ai.interests, aiinterest => aiinterest == interest)))
            {
                currAI = ai;
                if (!open && ai.doorOpenAnimTrigger != "") // Only play AI door anim if the door is closed and it has an anim
                {
                    currAI.animator.SetTrigger(ai.doorOpenAnimTrigger);
                    ToggleStun();
                }
                checkAIExists = true;
                Open();
                if(currAI.stunned) // Only toggle if AI is currently stunned
                    Invoke("ToggleStun", waitTime);
            }
        }
    }

    private void ToggleStun()
    {
        currAI.stunned = !currAI.stunned;
    }

    public void OnTriggerExit(Collider other)
    {
        AIAgent ai = other.gameObject.GetComponentInParent<AIAgent>();
        if (ai)
        {
            currAI = null;
            checkAIExists = false;
        }
        if (ai && (closeRequiredStates.Length == 0 || System.Array.Exists(closeRequiredStates, item => item == ai.state)))
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

    private void EnableLocked()
    {
        foreach (GameObject obj in lockedIcons)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in unlockedIcons)
        {
            obj.SetActive(false);
        }
    }

    private void EnableUnlocked()
    {
        foreach (GameObject obj in lockedIcons)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in unlockedIcons)
        {
            obj.SetActive(true);
        }
    }

    /// <summary>
    /// honestly don't know why i didn't just make these animations. oh well
    /// </summary>
    /// <returns></returns>
    IEnumerator OpenDoor()
    {
        if(currAI.doorOpenAnimTrigger != "")
            yield return new WaitForSecondsRealtime(1.5f);
        while (animating)
            yield return null;
        doorOpenClip.Post(gameObject);
        EnableUnlocked();
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
        EnableLocked();
    }
}
