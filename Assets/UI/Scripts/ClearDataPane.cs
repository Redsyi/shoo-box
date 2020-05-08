using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClearDataPane : MonoBehaviour
{
    public GameObject confirmation;
    public GameObject defaultSelected;
    public GameObject noButton;

    public void HideConfirmation()
    {
        confirmation.SetActive(false);
        EventSystem.current.SetSelectedGameObject(defaultSelected);
    }

    public void ShowConfirmation()
    {
        confirmation.SetActive(true);
        EventSystem.current.SetSelectedGameObject(noButton);
    }
}
