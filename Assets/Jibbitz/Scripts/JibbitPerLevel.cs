using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// class that shows how many jibbitz are collected in level select of main menu
/// </summary>
public class JibbitPerLevel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
{
    //create an expandable list of jibbitz
    [Tooltip("Add all jibbitz that appear in the level")]
    [SerializeField] List<Jibbit> JibzInLevel = new List<Jibbit>();
    [Tooltip("The text gameobject that displays collected out of total jibbitz")]
    [SerializeField] Text totalCollectable;
    int jibCounter = 0;
    int jibzCollected = 0;

    //jibbitz fraction changes on hover/unhover
    bool hoverOnLevel;
    static GameObject lastSelected;

    // Start is called before the first frame update
    void Start()
    {
        //Load what jibbitz the player has collected
        JibbitManager.LoadJibz();
        if (gameObject.tag == "BackButton")
        {
            //Skip back button when loading jibbit counter
            return;
        }
        UpdateJibFraction();
    }

    // Update is called once per frame
    /// <summary>
    /// handles level hover code when no other level is selected
    /// shamelessly copied from CollectionJibbit.cs
    /// </summary>
    void Update()
    {
        /// This only works with keyboard controls
        if (hoverOnLevel && EventSystem.current.currentSelectedGameObject != gameObject)
        {
            hoverOnLevel = false;
            if (EventSystem.current.currentSelectedGameObject != lastSelected)
            {
                totalCollectable.text = "- / -";
            }
        }

    }

    /// <summary>
    /// Takes a number & which placement in fraction it should have & replaces text w it
    /// </summary>
    void PlugNumsIntoText(int number, int whichNumber)
    {
        if (whichNumber == 1)
        {
            //plug number into the first value
            totalCollectable.text = number + " / " + jibCounter;
        }

        if (whichNumber == 2)
        {
            //plug number into the second value
            totalCollectable.text = jibzCollected + " / " + number;
        }
    }

    /// <summary>
    /// Uses PlugNums to change the jibbitz fraction
    /// </summary>
    public void UpdateJibFraction()
    {
        //Use the length of the list for total jibz collectable in level
        jibCounter = JibzInLevel.Count;

        PlugNumsIntoText(jibzCollected, 1);
        PlugNumsIntoText(jibCounter, 2);
    }

    /// <summary>
    /// Checks if the player has this jibbit yet, if yes, add to jibCount
    /// </summary>
    void CheckJibList()
    {
        jibzCollected = 0;
        foreach (Jibbit jib in JibzInLevel) {
            if (JibbitManager.HasJibbit(jib.id) == true)
            {
                jibzCollected += 1;
            }
        }

        UpdateJibFraction();

        if (jibzCollected == jibCounter)
        {
            //handles level hover code when no other level is selected
            //only works with pointer
            if (JibzInLevel.Count <=0 || JibzInLevel.Count ==  null)
            {
                //This is the back button
                totalCollectable.text = "- / -";
            }

            if (jibCounter != 0)
            {
                //turn jibbitz fraction green when all from a level are collected
                totalCollectable.text = "<color=#3BFF3B>" + totalCollectable.text + "</color>";
            }
        }
        
    }

    //Event handlers
    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverOnLevel = true;
        lastSelected = gameObject;
        CheckJibList();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverOnLevel = false;
        CheckJibList();
    }

    public void OnSelect(BaseEventData eventData)
    {
        hoverOnLevel = true;
        lastSelected = gameObject;
        CheckJibList();
    }
}
