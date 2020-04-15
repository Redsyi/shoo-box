using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotdogSuitcase : MonoBehaviour
{
    Animator animator;
    public GameObject hotdog;
    public GameObject sandals;
    public Vector3 sandalPlaceOffset;
    public Vector3 placeOffset;
    public StealFocusWhenSeen cutscene;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ChangeCutsceneParent(Transform newParent)
    {
        cutscene.transform.SetParent(newParent);
        cutscene.transform.localPosition = Vector3.zero;
    }

    public void Show(Transform position)
    {
        transform.position = position.position + placeOffset;
        gameObject.SetActive(true);
    }

    public void Open()
    {
        animator.SetTrigger("Open");
    }

    public void Ransack()
    {
        hotdog.SetActive(false);
        sandals.transform.position += sandalPlaceOffset;
    }
}
