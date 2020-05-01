using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//deprecated
public class UILevelSelectButton : MonoBehaviour, ISelectHandler
{
    private ScrollRect scrollRect;

    private void Start()
    {
        scrollRect = GetComponentInParent<ScrollRect>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        StartCoroutine(SetScrollValue(Mathf.Clamp01(transform.GetSiblingIndex() * 1f / transform.parent.childCount) + 0.05f));
    }

    IEnumerator SetScrollValue(float val)
    {
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = (1 - val);
        yield return null;
    }
}