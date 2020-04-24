using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIPaneManager : MonoBehaviour
{
    [System.Serializable]
    public class UIPane
    {
        public GameObject pane;
        public GameObject defaultSelected;
        public UnityEvent onAppear;
        public UnityEvent onDisappear;
    }

    public UIButtonPane buttonPane;
    public UIPane[] panes;
    public int currPaneIdx;
    public float paneSwipeSpeed = 0.3f;
    public AK.Wwise.Event swipeSound;
    int animatorsActive;
    bool currentlySwitchingPanes => animatorsActive > 0;

    UIPane currPane => panes[currPaneIdx];

    private void Awake()
    {
        buttonPane.panes = this;
    }

    public void NextPane()
    {
        SwitchToPane(currPaneIdx + 1);
    }

    public void PreviousPane()
    {
        SwitchToPane(currPaneIdx - 1);
    }

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

    public void Appear()
    {
        buttonPane.currTabIdx = currPaneIdx;
        buttonPane.Activate();
        currPane.onAppear.Invoke();
        AnimateIn(currPane, UIDirection.DOWN);
        EventSystem.current.SetSelectedGameObject(currPane.defaultSelected);
    }

    public void Disappear()
    {
        currPane.onDisappear.Invoke();
        buttonPane.Deactivate();
        AnimateOut(currPane, UIDirection.DOWN);
        EventSystem.current.SetSelectedGameObject(null);
    }

    void AnimateIn(UIPane pane, UIDirection direction)
    {
        StartCoroutine(DoAnimateIn(pane, direction));
    }

    void AnimateOut(UIPane pane, UIDirection direction)
    {
        StartCoroutine(DoAnimateOut(pane, direction));
    }

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
