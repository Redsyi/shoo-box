using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stoplight : MonoBehaviour, IKickable
{
    public GameObject[] greenLights;
    public GameObject[] yellowLights;
    public GameObject[] redLights;
    public StoplightIntersection intersectionController;
    public Direction controlDirection;

    [HideInInspector]
    public bool broken;
    StoplightColor currColor;

    private void Awake()
    {
        currColor = StoplightColor.GREEN;
    }

    public void TurnColor(StoplightColor color)
    {
        if (!broken)
        {
            if (currColor == StoplightColor.GREEN)
                SetActive(greenLights, false);
            if (currColor == StoplightColor.YELLOW)
                SetActive(yellowLights, false);
            if (currColor == StoplightColor.RED)
                SetActive(redLights, false);

            if (color == StoplightColor.GREEN)
                SetActive(greenLights, true);
            if (color == StoplightColor.YELLOW)
                SetActive(yellowLights, true);
            if (color == StoplightColor.RED)
                SetActive(redLights, true);

            currColor = color;
        }
    }
        
    public void OnKick(GameObject kicker)
    {
        if (!broken)
        {
            broken = true;
            intersectionController.StoplightBroken(this);
            if (currColor == StoplightColor.GREEN)
                SetActive(greenLights, false);
            if (currColor == StoplightColor.YELLOW)
                SetActive(yellowLights, false);
            if (currColor == StoplightColor.RED)
                SetActive(redLights, false);
        }
    }

    void SetActive(GameObject[] lights, bool active)
    {
        foreach (GameObject light in lights)
        {
            light.SetActive(active);
        }
    }
}
