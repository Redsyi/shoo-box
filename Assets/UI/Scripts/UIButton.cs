using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public Text text;
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
            if (!_selected && text)
            {
                if (_hovering)
                {
                    text.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                    text.transform.localEulerAngles = new Vector3(0, 0, -2);
                }
                else
                {
                    text.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                    text.transform.localEulerAngles = new Vector3(0, 0, 0);
                }
            }
        }
    }
    bool _selected;
    bool selected
    {
        get
        {
            return _selected;
        }
        set
        {
            _selected = value;
            if (!_hovering && text)
            {
                if (_selected)
                {
                    text.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                    text.transform.localEulerAngles = new Vector3(0, 0, -2);
                }
                else
                {
                    text.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                    text.transform.localEulerAngles = new Vector3(0, 0, 0);
                }
            }
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        selected = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
        selected = true;
    }
}
