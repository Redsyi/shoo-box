using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIOptionsTabGroup : MonoBehaviour
{
    public List<UIOptionsTabButton> tabButtons;
    public List<GameObject> objectsToSwap;
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    public UIOptionsTabButton selectedTab;

    public void Subscribe(UIOptionsTabButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<UIOptionsTabButton>();
        }

        tabButtons.Add(button);

        //Make first tab sprite automatically selected
        if (selectedTab == null && tabButtons.Count > 3)
        {
            for (int i = 0; i < tabButtons.Count; i++)
            {
                string buttonName = tabButtons[i].gameObject.name;
                if (buttonName == "Level Select")
                {
                    selectedTab = tabButtons[i];
                }
            }
            selectedTab.background.sprite = tabActive;
            selectedTab.background.color = new Color(0.9f, 0.9f, 0.9f);
        }
    }

    public void OnTabEnter(UIOptionsTabButton button)
    {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
        {
            button.background.sprite = tabHover;
            button.background.color = new Color(0.9f,0.9f,0.9f);
        }
        
    }

    public void OnTabExit(UIOptionsTabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(UIOptionsTabButton button)
    {
        if (selectedTab != null)
        {
            selectedTab.Deselect();
        }

        selectedTab = button;

        selectedTab.Select();

        ResetTabs();
        button.background.sprite = tabActive;

        //Swap tab pages on select
        //index incorrect bc of tab parenting, so do minus 1
        //Keep OptionsMenu set inactive so list is populated correctly
        int index = button.transform.GetSiblingIndex() - 1;
        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                objectsToSwap[i].SetActive(true);
            }
            else
            {
                objectsToSwap[i].SetActive(false);
            }
        }
    }

    public void ResetTabs()
    {
        foreach (UIOptionsTabButton button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab)
            {
                continue;
            }
            button.background.sprite = tabIdle;
            button.background.color = Color.white;
        }
    }
}
