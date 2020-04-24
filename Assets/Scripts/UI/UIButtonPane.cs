using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonPane : MonoBehaviour
{
    List<UITabButton> tabs;
    [HideInInspector]
    public int currTabIdx;
    public Sprite activeSprite;
    public Sprite inactiveSprite;
    public float animateInTime = 0.3f;
    bool active;
    RectTransform rect;
    [HideInInspector] public UIPaneManager panes;

    UITabButton currTab => tabs[currTabIdx];

    private void Awake()
    {
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

        rect = GetComponent<RectTransform>();
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = Vector2.zero;
    }

    private void Start()
    {
        Deactivate();
    }

    public void Select(int tabIdx)
    {
        currTab.selected = false;
        currTabIdx = tabIdx;
        currTab.selected = true;
    }

    public void Activate()
    {
        Select(currTabIdx);
        StartCoroutine(DoAnimateIn());
    }

    public void Deactivate()
    {
        StartCoroutine(DoAnimateOut());
    }

    public void ButtonSelected(int index)
    {
        panes.SwitchToPane(index);
    }
    
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
