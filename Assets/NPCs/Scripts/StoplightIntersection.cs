using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that emulates a stoplight-managed intersection for cars
/// </summary>
[RequireComponent(typeof(CityRoad))]
public class StoplightIntersection : MonoBehaviour
{
    [Header("References")]
    public Stoplight northLight;
    public Stoplight eastLight;
    public Stoplight southLight;
    public Stoplight westLight;
    [Header("Timings")]
    public float lightTiming;
    public float yellowLength;
    public float greenDelay;
    [Header("Sound")]
    public AK.Wwise.Event lightChangeEvent;

    CityRoad road;
    Dictionary<Direction, bool> alwaysOn;

    void Start()
    {
        alwaysOn = new Dictionary<Direction, bool>();
        road = GetComponent<CityRoad>();
        DirectionUtil.FillDict(alwaysOn, false);
        StartCoroutine(RemoveIrrelevantLights());
        StartCoroutine(CycleLights());
    }

    /// <summary>
    /// queries the attached road for it's surrounding roads, and removes lights that face nothing
    /// </summary>
    IEnumerator RemoveIrrelevantLights()
    {
        yield return null;
        if (!road.surroundingRoads[Direction.NORTH])
        {
            DisableLight(Direction.SOUTH);
            southLight.gameObject.SetActive(false);
        }
        if (!road.surroundingRoads[Direction.SOUTH])
        {
            DisableLight(Direction.NORTH);
            northLight.gameObject.SetActive(false);
        }
        if (!road.surroundingRoads[Direction.EAST])
        {
            DisableLight(Direction.WEST);
            westLight.gameObject.SetActive(false);
        }
        if (!road.surroundingRoads[Direction.WEST])
        {
            DisableLight(Direction.EAST);
            eastLight.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Light manager
    /// </summary>
    IEnumerator CycleLights()
    {
        //initial state
        northLight.TurnColor(StoplightColor.RED);
        southLight.TurnColor(StoplightColor.RED);
        road.canGo[Direction.NORTH] = false;
        road.canGo[Direction.SOUTH] = false;

        while (true)
        {
            //kind of a wall of text i know, but actually pretty simple. just changing light colors, setting the underlying road's car permissions, and waiting
            yield return new WaitForSeconds(lightTiming - yellowLength - greenDelay);
            eastLight.TurnColor(StoplightColor.YELLOW);
            westLight.TurnColor(StoplightColor.YELLOW);
            if (!alwaysOn[Direction.EAST])
                road.canGo[Direction.EAST] = false;
            if (!alwaysOn[Direction.WEST])
                road.canGo[Direction.WEST] = false;
            yield return new WaitForSeconds(yellowLength);
            eastLight.TurnColor(StoplightColor.RED);
            westLight.TurnColor(StoplightColor.RED);
            yield return new WaitForSeconds(greenDelay);
            if (Utilities.OnScreen(transform.position))
                lightChangeEvent.Post(gameObject);
            northLight.TurnColor(StoplightColor.GREEN);
            southLight.TurnColor(StoplightColor.GREEN);
            road.canGo[Direction.NORTH] = true;
            road.canGo[Direction.SOUTH] = true;

            yield return new WaitForSeconds(lightTiming - yellowLength - greenDelay);
            northLight.TurnColor(StoplightColor.YELLOW);
            southLight.TurnColor(StoplightColor.YELLOW);
            if (!alwaysOn[Direction.NORTH])
                road.canGo[Direction.NORTH] = false;
            if (!alwaysOn[Direction.SOUTH])
                road.canGo[Direction.SOUTH] = false;
            yield return new WaitForSeconds(yellowLength);
            northLight.TurnColor(StoplightColor.RED);
            southLight.TurnColor(StoplightColor.RED);
            yield return new WaitForSeconds(greenDelay);
            if (Utilities.OnScreen(transform.position))
                lightChangeEvent.Post(gameObject);
            eastLight.TurnColor(StoplightColor.GREEN);
            westLight.TurnColor(StoplightColor.GREEN);
            road.canGo[Direction.EAST] = true;
            road.canGo[Direction.WEST] = true;
        }
    }

    /// <summary>
    /// Notifies this intersection that the designated stoplight is broken, preventing it from setting permissions on the underlying road
    /// </summary>
    public void StoplightBroken(Stoplight stoplight)
    {
        DisableLight(stoplight.controlDirection);
    }

    void DisableLight(Direction direction)
    {
        road.canGo[direction] = true;
        alwaysOn[direction] = true;
    }
}
