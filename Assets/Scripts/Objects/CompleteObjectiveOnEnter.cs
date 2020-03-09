using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteObjectiveOnEnter : MonoBehaviour
{
    public int objectiveNum;

    private void OnTriggerEnter(Collider other)
    {
        ObjectiveTracker.instance.CompleteObjective(objectiveNum);
    }
}
