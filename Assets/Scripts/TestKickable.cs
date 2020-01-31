using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestKickable : MonoBehaviour, IKickable
{

    public void OnKick(GameObject kicker)
    {
        Debug.Log("I was kicked :(");
    }
}
