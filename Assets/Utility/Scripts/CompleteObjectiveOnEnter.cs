using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// completes an objective when something enters a trigger
/// </summary>
public class CompleteObjectiveOnEnter : MonoBehaviour
{
    public int objectiveNum;

    private void OnTriggerEnter(Collider other)
    {
        ObjectiveTracker.instance.CompleteObjective(objectiveNum);
    }
}
