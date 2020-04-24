using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [HideInInspector] public Sprite selectedImage;
    [HideInInspector] public Sprite unselectedImage;
    [HideInInspector] public int index;
    private bool _selected;
    [HideInInspector] public bool selected
    {
        get
        {
            return _selected;
        }
        set
        {
            _selected = value;
            if (value)
                buttonImage.sprite = selectedImage;
            else if (!hovering)
                buttonImage.sprite = unselectedImage;
        }
    }
    bool _hovering;
    bool hovering
    {
        get
        {
            return _hovering;
        }
        set
        {
            _hovering = value;
            if (value)
                buttonImage.sprite = selectedImage;
            else if (!selected)
                buttonImage.sprite = unselectedImage;
        }
    }
    UIButtonPane parent;
    public Image buttonImage;
    public AK.Wwise.Event hoverSound;
    public AK.Wwise.Event clickSound;

    public void OnPointerClick(PointerEventData eventData)
    {
        clickSound.Post(gameObject);
        if (parent)
        {
            parent.ButtonSelected(index);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
        hoverSound.Post(gameObject);
    }
    
    private void Awake()
    {
        parent = GetComponentInParent<UIButtonPane>();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }
}
