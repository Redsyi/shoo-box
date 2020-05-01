using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// completes an objective on kick
/// </summary>
public class CompleteObjectiveOnKick : MonoBehaviour, IKickable
{
    public int objectiveNum;
    public void OnKick(GameObject kicker)
    {
        ObjectiveTracker.instance.CompleteObjective(objectiveNum);
    }
}
