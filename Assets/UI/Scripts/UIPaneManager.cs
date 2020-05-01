using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// class that manages several panes in the UI, allowing the ability to swap between them
/// </summary>
public class UIPaneManager : MonoBehaviour
{
    [System.Serializable]
    public class UIPane
    {
        [Tooltip("The pane itself")]
        public GameObject pane;
        [Tooltip("The object in the pane that should be selected by default. Can be left empty.")]
        public GameObject defaultSelected;
        [Tooltip("Action to perform when this pane is switched to")]
        public UnityEvent onAppear;
        [Tooltip("Action to perform when this pane is switched from")]
        public UnityEvent onDisappear;
    }

    [Tooltip("The button pane (i.e. tabs) connected to these panes")]
    public UIButtonPane buttonPane;
    public UIPane[] panes;
    public int currPaneIdx;
    [Tooltip("Time (in seconds) to swap panes")]
    public float paneSwipeSpeed = 0.3f;
    [Tooltip("Sound played when panes are swapped")]
    public AK.Wwise.Event swipeSound;

    int animatorsActive;
    bool currentlySwitchingPanes => animatorsActive > 0;
    UIPane currPane => panes[currPaneIdx];

    private void Awake()
    {
        buttonPane.panes = this;
    }

    /// <summary>
    /// Advances to the next pane if able
    /// </summary>
    public void NextPane()
    {
        SwitchToPane(currPaneIdx + 1);
    }

    /// <summary>
    /// Advances to the previous pane if able
    /// </summary>
    public void PreviousPane()
    {
        SwitchToPane(currPaneIdx - 1);
    }

    /// <summary>
    /// Switches to the given pane index if able and different than the current one
    /// </summary>
    public void SwitchToPane(int paneIdx)
    {
        if (paneIdx >= 0 && paneIdx < panes.Length && paneIdx != currPaneIdx)
        {
            if (paneIdx > currPaneIdx)
                Shift(UIDirection.RIGHT, UIDirection.LEFT, paneIdx);
            else
                Shift(UIDirection.LEFT, UIDirection.RIGHT, paneIdx);
        }
    }

    //General code to switch from the current pane to a new one
    void Shift(UIDirection newFrom, UIDirection oldTo, int newIndex)
    {
        if (!currentlySwitchingPanes)
        {
            swipeSound.Post(gameObject);
            currPane.onDisappear.Invoke();
            AnimateOut(currPane, oldTo);
            currPaneIdx = newIndex;
            AnimateIn(currPane, newFrom);
            buttonPane.Select(currPaneIdx);
            EventSystem.current.SetSelectedGameObject(currPane.defaultSelected);
            currPane.onAppear.Invoke();
        }
    }

    /// <summary>
    /// Bring up the pane manager
    /// </summary>
    public void Appear()
    {
        buttonPane.currTabIdx = currPaneIdx;
        buttonPane.Activate();
        currPane.onAppear.Invoke();
        AnimateIn(currPane, UIDirection.DOWN);
        EventSystem.current.SetSelectedGameObject(currPane.defaultSelected);
    }

    /// <summary>
    /// Dismiss the pane manager
    /// </summary>
    public void Disappear()
    {
        currPane.onDisappear.Invoke();
        buttonPane.Deactivate();
        AnimateOut(currPane, UIDirection.DOWN);
        EventSystem.current.SetSelectedGameObject(null);
    }

    //kicks off a pane animation in
    void AnimateIn(UIPane pane, UIDirection direction)
    {
        StartCoroutine(DoAnimateIn(pane, direction));
    }

    //kicks off a pane animation out
    void AnimateOut(UIPane pane, UIDirection direction)
    {
        StartCoroutine(DoAnimateOut(pane, direction));
    }

    //performs an animation. I probably would've done this using a unity animation if I had known at the time
    //that you can set an animation to use unscaled time, but this works fine
    IEnumerator DoAnimateIn(UIPane pane, UIDirection direction)
    {
        animatorsActive++;
        float timePassed = 0f;
        float progress = 0f;
        RectTransform rect = pane.pane.GetComponent<RectTransform>();
        pane.pane.SetActive(true);
        Vector2 originalPivot = getPivot(direction);
        Vector2 originalAnchor = getAnchor(direction);
        Vector2 destPivot = new Vector2(0.5f, 0.5f);
        while (progress < 1)
        {
            yield return null;
            pane.pane.SetActive(true);
            timePassed += Time.unscaledDeltaTime;
            progress = Mathf.Clamp01(timePassed / paneSwipeSpeed);
            rect.pivot = Vector2.Lerp(originalPivot, destPivot, progress);
            rect.anchorMin = Vector2.Lerp(originalAnchor, destPivot, progress);
            rect.anchorMax = Vector2.Lerp(originalAnchor, destPivot, progress);
            rect.anchoredPosition = Vector2.zero;
        }
        animatorsActive--;
    }

    IEnumerator DoAnimateOut(UIPane pane, UIDirection direction)
    {
        animatorsActive++;
        float timePassed = 0f;
        float progress = 0f;
        RectTransform rect = pane.pane.GetComponent<RectTransform>();
        Vector2 destPivot = getPivot(direction);
        Vector2 destAnchor = getAnchor(direction);
        Vector2 originalPivot = new Vector2(0.5f, 0.5f);
        while (progress < 1)
        {
            yield return null;
            timePassed += Time.unscaledDeltaTime;
            progress = Mathf.Clamp01(timePassed / paneSwipeSpeed);
            rect.pivot = Vector2.Lerp(originalPivot, destPivot, progress);
            rect.anchorMin = Vector2.Lerp(originalPivot, destAnchor, progress);
            rect.anchorMax = Vector2.Lerp(originalPivot, destAnchor, progress);
            rect.anchoredPosition = Vector2.zero;
        }
        pane.pane.SetActive(false);
        animatorsActive--;
    }

    //get the pivot needed to align a pane in the given position
    Vector2 getPivot(UIDirection forDirection)
    {
        if (forDirection == UIDirection.LEFT)
            return new Vector2(1, 0.5f);
        else if (forDirection == UIDirection.RIGHT)
            return new Vector2(0, 0.5f);
        else if (forDirection == UIDirection.UP)
            return new Vector2(0.5f, 0);
        else
            return new Vector2(0.5f, 1);
    }

    //get the anchor needed to align a pane in the given position
    Vector2 getAnchor(UIDirection forDirection)
    {
        if (forDirection == UIDirection.LEFT)
            return new Vector2(0, 0.5f);
        else if (forDirection == UIDirection.RIGHT)
            return new Vector2(1, 0.5f);
        else if (forDirection == UIDirection.UP)
            return new Vector2(0.5f, 1);
        else
            return new Vector2(0.5f, 0);
    }
}
