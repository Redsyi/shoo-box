using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveTracker : MonoBehaviour
{
    public string[] objectives;
    private int currObjective;
    public static ObjectiveTracker instance;
    private List<UIObjective> objectiveUIs;
    public UIObjective objectivePrefab;

    private void Awake()
    {
        instance = this;
        objectiveUIs = new List<UIObjective>();
        foreach (string objectiveText in objectives)
        {
            UIObjective newObjective = Instantiate(objectivePrefab, transform);
            newObjective.SetText(objectiveText);
            objectiveUIs.Add(newObjective);
        }
    }

    public void CompleteObjective(int objectiveNum)
    {
        UIShoeSightReminder.instance.ObjectiveCompleted();
        objectiveUIs[objectiveNum].Complete();
        currObjective = objectiveNum;
    }
}
