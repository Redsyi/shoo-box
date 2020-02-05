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

    private void Start()
    {
        openDir = (openAngle - closedAngle > 0 ? 1 : -1);
    }

    public void OnTriggerEnter(Collider other)
    {
        Maid ai = other.gameObject.GetComponentInParent<Maid>();
        if (ai)
        {
            if (!open)
            {
                StartCoroutine(OpenDoor());
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Maid ai = other.gameObject.GetComponentInParent<Maid>();
        if (ai)
        {
            if (open && ai.currState.state == AIState.IDLE)
            {
                StartCoroutine(CloseDoor());
            }
        }
    }

    IEnumerator OpenDoor()
    {
        while (animating)
            yield return null;
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
