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

    private void Start()
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
        for (int i = currObjective ; i < objectiveNum + 1; ++i)
        {
            objectiveUIs[i].Complete();
        }
        currObjective = objectiveNum;
    }
}
