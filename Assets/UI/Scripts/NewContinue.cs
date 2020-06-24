using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewContinue : MonoBehaviour
{
    public GameObject newGameButton;
    public GameObject continueButton;
    public GameObject levelsButton;
    bool selectContinue;
    bool selectNew;
    float selectDelay;

    public void PaneLoaded()
    {
        if (PlayerData.currCheckpoint != 0 || PlayerData.currLevel != PlayerData.defaultLevel.saveID)
        {
            continueButton.SetActive(true);
            selectContinue = true;
            newGameButton.SetActive(false);
        } else
        {
            selectNew = true;
        }
        selectDelay = 0;

        int unlockedLevels = 0;
        foreach (bool levelUnlocked in PlayerData.unlockedLevels.Values)
        {
            if (levelUnlocked)
                unlockedLevels++;
        }

        if (unlockedLevels > 1)
            levelsButton.SetActive(true);
    }

    //kind of a janky way to make sure the correct button is selected. would use coroutines but those don't always work here because of how object activation works...
    private void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            if (selectContinue && selectDelay > 0.1f)
            {
                selectContinue = false;
                EventSystem.current.SetSelectedGameObject(continueButton);
            }
            if (selectNew && selectDelay > 0.1f)
            {
                selectNew = false;
                EventSystem.current.SetSelectedGameObject(newGameButton);
            }
            if (selectDelay <= 0.1f)
                selectDelay += Time.deltaTime;
        }
    }
}
