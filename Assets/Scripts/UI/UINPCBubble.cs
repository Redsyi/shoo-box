using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Prevent investigation bubble from showing off-screen and rotate arrow to face NPC when this happens
//TODO: "speech" bubbles

public class UINPCBubble : MonoBehaviour
{
    public Transform worldAnchor;
    public GameObject spottedBubble;
    public GameObject investigateBubble;
    public RectTransform investigateBG;
    public Vector2 offset;
    private bool spotting;
    private bool investigating;

    public void Spotted()
    {
        if (!spotting)
            StartCoroutine(Spot());
    }

    IEnumerator Spot()
    {
        spotting = true;
        investigateBubble.SetActive(false);
        spottedBubble.SetActive(true);
        yield return new WaitForSeconds(2);         //magic number
        spottedBubble.SetActive(false);
        spotting = false;
    }

    public void Investigating()
    {
        if (!investigating && !spotting)
        {
            investigating = true;
            investigateBubble.SetActive(true);
        }
    }

    public void StopInvestigating()
    {
        if (investigating)
        {
            investigating = false;
            investigateBubble.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (worldAnchor != null) {
            if (spotting || investigating)
            {
                Vector2 canvasPoint = (Vector2)CameraScript.current.camera.WorldToScreenPoint(worldAnchor.position) + offset;
                transform.position = canvasPoint;
            }
        } else
        {
            Destroy(gameObject);
        }
    }
}
