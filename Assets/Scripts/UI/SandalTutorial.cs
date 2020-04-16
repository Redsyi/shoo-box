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
    }

    public void TeachSwap()
    {
        if (FindObjectOfType<PlayerShoeManager>().Has(ShoeType.FLIPFLOPS))
            swapPopup.Activate();
    }

    public void ShowSwapControls()
    {
        if (FindObjectOfType<PlayerShoeManager>().Has(ShoeType.FLIPFLOPS))
            swapControls.SetActive(true);
    }

    public void HideSwapControls()
    {
        swapControls.SetActive(false);
    }

    public void SwapPopupDismissed()
    {
        Destroy(swapTeacher);
    }
}
