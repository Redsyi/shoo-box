using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that manages the suitcase victim for the conveyor mechanics cutscene
/// </summary>
public class HotdogSuitcase : MonoBehaviour
{
    Animator animator;
    public GameObject hotdog;
    public GameObject sandals;
    public Vector3 sandalPlaceOffset;
    public Vector3 placeOffset;
    public StealFocusWhenSeen cutscene;
    public Transform agentHand;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// called when tsa agent takes suitcase from belt
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// called when the cutscene needs to focus on the tsa agent
    /// </summary>
    public void ChangeCutsceneParent(Transform newParent)
    {
        cutscene.transform.SetParent(newParent);
        cutscene.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// called when the tsa agent puts the suitcase down
    /// </summary>
    public void Show(Transform position)
    {
        transform.position = position.position + placeOffset;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// called when the tsa agent opens the suitcase
    /// </summary>
    public void Open()
    {
        animator.SetTrigger("Open");
    }

    /// <summary>
    /// called when the tsa agent takes the hotdog and sandals out of the suitcase
    /// </summary>
    public void Ransack()
    {
        if (agentHand) {
            hotdog.transform.SetParent(agentHand);
            hotdog.transform.localPosition = new Vector3(-0.0686f, 0.0003f, -0.0073f);
            hotdog.transform.localEulerAngles = new Vector3(100.006f, -89.979f, -180f);
        } else
        {
            hotdog.SetActive(false);
        }
        sandals.transform.position += sandalPlaceOffset;
    }
}
