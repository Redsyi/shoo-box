using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltSwitch : MonoBehaviour, IKickable, ISandalable
{


    public void HitBySandal()
    {
        ConveyorBelt.ToggleActive();
    }

    public void OnKick(GameObject kicker)
    {
        ConveyorBelt.ToggleActive();
    }
}
