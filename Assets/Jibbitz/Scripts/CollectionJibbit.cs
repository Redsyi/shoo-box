using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// class that manages a jibbit shown in the collection page of the main menu
/// </summary>
public class CollectionJibbit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
{
    public Jibbit jibbit;
    public AK.Wwise.Event onSelectSound;

    bool _hovering;
    bool hovering
    {
        get
        {
            return _hovering;
        }
        set
        {
            if (value != _hovering)
            {
                _hovering = value;
                if (value && !selected)
                {
                    Highlight();
                } else if (!value && !selected)
                {
                    Unhighlight();
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
            if (value != _selected)
            {
                _selected = value;
                if (value && !hovering)
                {
                    Highlight();
                }
                else if (!value && !hovering)
                {
                    Unhighlight();
                }
            }
        }
    }
    
    MeshRenderer renderer;
    UIMainMenu mainMenu;
    static GameObject lastSelected;

    //hover handlers
    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }

    /// <summary>
    /// Handles being selected by the user
    /// </summary>
    public void OnSelect(BaseEventData eventData)
    {
        onSelectSound.Post(gameObject);
        lastSelected = gameObject;
        selected = true;
        if (mainMenu)
            mainMenu.ShowJibbitLabel(jibbit.displayName, jibbit.hint);
    }

    /// <summary>
    /// handles unselect code, including when no other jibbit is selected
    /// </summary>
    private void Update()
    {
        if (selected && EventSystem.current.currentSelectedGameObject != gameObject)
        {
            selected = false;
            if (EventSystem.current.currentSelectedGameObject != lastSelected)
            {
                if (mainMenu)
                    mainMenu.HideJibbitLabel();
            }
        }
    }

    /// <summary>
    /// Enables highlighting on the outline of this jibbit
    /// </summary>
    void Highlight()
    {
        if (!renderer)
            renderer = GetComponentInChildren<MeshRenderer>();
        foreach (Material material in renderer.materials)
        {
            material.SetFloat("_IsOutlined", 1);
        }
    }

    /// <summary>
    /// Removes the highlighting on the outline of this jibbit
    /// </summary>
    void Unhighlight()
    {
        if (!renderer)
            renderer = GetComponentInChildren<MeshRenderer>();
        foreach (Material material in renderer.materials)
        {
            material.SetFloat("_IsOutlined", 0);
        }
    }

    private void Awake()
    {
        Invoke("CheckJibbitAcquired", 0.05f);
    }

    void Start()
    {
        renderer = GetComponentInChildren<MeshRenderer>();
        mainMenu = FindObjectOfType<UIMainMenu>();
    }

    /// <summary>
    /// Checks if the player has this jibbit yet, if not, set materials to appear black
    /// </summary>
    public void CheckJibbitAcquired()
    {
        if (!JibbitManager.HasJibbit(jibbit.id))
        {
            if (!renderer)
                renderer = GetComponentInChildren<MeshRenderer>();
            foreach (Material material in renderer.materials)
            {
                material.SetFloat("_Acquired", 0);
            }
        }
    }
}
