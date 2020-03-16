using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteObjectiveOnSandal : MonoBehaviour, ISandalable
{
    public int objectiveNum;
    public void HitBySandal()
    {
        ObjectiveTracker.instance.CompleteObjective(objectiveNum);
    }
}
