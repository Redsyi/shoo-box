using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Event = AK.Wwise.Event;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Class that plays Wwise events for UI elemetns
/// </summary>
[RequireComponent(typeof(AkGameObj))]
[RequireComponent(typeof(Button))]
public class AKEventUI : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    public Event hoverSound;
    public Event clickSound;
    private AkGameObj akObj;
    private Button button;

    private void Awake()
    {
        akObj = GetComponent<AkGameObj>();
    }
    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => clickSound?.Post(akObj.gameObject));
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverSound?.Post(akObj.gameObject);
    }

    public void OnSelect(BaseEventData eventData)
    {
        hoverSound?.Post(akObj.gameObject);
    }
}
