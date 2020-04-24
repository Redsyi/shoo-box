﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveTracker : MonoBehaviour
{
    public string[] objectives;
    private int currObjective = 0;
    public static ObjectiveTracker instance;
    private List<UIObjective> objectiveUIs;
    public UIObjective objectivePrefab;
    public Text objectiveTagText;

    private void Awake()
    {
        instance = this;
        /* objectiveUIs = new List<UIObjective>();
         foreach (string objectiveText in objectives)
         {
             UIObjective newObjective = Instantiate(objectivePrefab, transform);
             newObjective.SetText(objectiveText);
             objectiveUIs.Add(newObjective);
         }*/
        if (objectiveTagText && objectives.Length > 0)
        {
            objectiveTagText.text = objectives[currObjective];
        }
       
    }

    public void CompleteObjective(int objectiveNum)
    {
        UIShoeSightReminder.instance.ObjectiveCompleted();
        //objectiveUIs[objectiveNum].Complete();
        currObjective++;
        if (currObjective < objectives.Length && objectiveTagText)
            objectiveTagText.text = objectives[currObjective]; // Update the objective tag with the next 
    }
}
