using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// class to manage the button pane/tabs for a UIPaneManager
/// </summary>
public class UIButtonPane : MonoBehaviour
{
    List<UITabButton> tabs;
    [HideInInspector]
    public int currTabIdx;

    [Tooltip("Sprite the active button should use")]
    public Sprite activeSprite;
    [Tooltip("Sprite the inactive buttons should use")]
    public Sprite inactiveSprite;
    [Tooltip("Time (in seconds) for the bar to be brought up from the bottom of the screen")]
    public float animateInTime = 0.3f;

    bool active;
    RectTransform rect;
    [HideInInspector] public UIPaneManager panes;
    UITabButton currTab => tabs[currTabIdx];

    private void Awake()
    {
        //automatically populate the list of buttons
        tabs = new List<UITabButton>();
        int i = 0;
        foreach (Transform child in transform)
        {
            UITabButton button = child.GetComponent<UITabButton>();
            if (button)
            {
                tabs.Add(button);
                button.index = i;
                button.selectedImage = activeSprite;
                button.unselectedImage = inactiveSprite;
                ++i;
            }
        }

        //set position to hidden
        rect = GetComponent<RectTransform>();
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = Vector2.zero;
    }

    //swap to the given button
    public void Select(int tabIdx)
    {
        if (tabIdx >= 0 && tabIdx < tabs.Count)
        {
            currTab.selected = false;
            currTabIdx = tabIdx;
            currTab.selected = true;
        }
    }

    //kicks off the animation in
    public void Activate()
    {
        Select(currTabIdx);
        if (gameObject.activeInHierarchy)
            StartCoroutine(DoAnimateIn());
    }

    //kicks off the animation out
    public void Deactivate()
    {
        StartCoroutine(DoAnimateOut());
    }

    /// <summary>
    /// Invoked when the user clicks on a button, tells the pane controller to switch to that pane
    /// </summary>
    public void ButtonSelected(int index)
    {
        panes.SwitchToPane(index);
    }
    
    //animates the button pane up from the bottom of the screen
    IEnumerator DoAnimateIn()
    {
        float timePassed = 0f;
        float progress = 0f;
        Vector2 originalPivot = new Vector2(0.5f, 1f);
        Vector2 destPivot = new Vector2(0.5f, 0f);
        while (progress < 1)
        {
            yield return null;
            timePassed += Time.unscaledDeltaTime;
            progress = Mathf.Clamp01(timePassed / animateInTime);
            rect.pivot = Vector2.Lerp(originalPivot, destPivot, progress);
            rect.anchoredPosition = Vector2.zero;
        }
    }

    //animates the button pane down under the bottom of the screen
    IEnumerator DoAnimateOut()
    {
        float timePassed = 0f;
        float progress = 0f;
        Vector2 originalPivot = new Vector2(0.5f, 0f);
        Vector2 destPivot = new Vector2(0.5f, 1f);
        while (progress < 1)
        {
            yield return null;
            timePassed += Time.unscaledDeltaTime;
            progress = Mathf.Clamp01(timePassed / animateInTime);
            rect.pivot = Vector2.Lerp(originalPivot, destPivot, progress);
            rect.anchoredPosition = Vector2.zero;
        }
    }
}
