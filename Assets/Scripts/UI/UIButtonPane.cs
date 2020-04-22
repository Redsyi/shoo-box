using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonPane : MonoBehaviour
{
    public List<Image> tabs;
    public int currTab;
    public Sprite activeSprite;
    public Sprite inactiveSprite;

    CanvasGroup group;

    Image currTabImage => tabs[currTab];

    private void Start()
    {
        group = GetComponent<CanvasGroup>();
        Select(currTab);
        Deactivate();
    }

    public void Select(int tabIdx)
    {
        currTabImage.sprite = inactiveSprite;
        currTab = tabIdx;
        currTabImage.sprite = activeSprite;
    }

    public void Activate()
    {
        group.alpha = 1f;
    }

    public void Deactivate()
    {
        group.alpha = 0f;
    }
}
