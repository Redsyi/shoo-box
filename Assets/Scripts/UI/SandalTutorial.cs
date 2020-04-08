using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandalTutorial : MonoBehaviour
{
    public UIPopup flingPopup;
    public UIPopup swapPopup;
    public GameObject swapControls;
    public GameObject swapTeacher;

    public void TeachFling()
    {
        flingPopup.Activate();
        FindObjectOfType<CheckpointManager>().SetCheckpoint(1);
    }

    public void TeachSwap()
    {
        swapPopup.Activate();
    }

    public void ShowSwapControls()
    {
        swapControls.SetActive(true);
    }

    public void HideSwapControls()
    {
        swapControls.SetActive(false);
    }

    public void SwapPopupDismissed()
    {
        Destroy(swapTeacher);
        FindObjectOfType<CheckpointManager>().SetCheckpoint(2);
    }
}
