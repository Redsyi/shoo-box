using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityRoad : MonoBehaviour
{
    public bool assignedHeli;
    public bool assignedTank;
    public static CityRoad[] intersections;

    private void Start()
    {
        intersections = FindObjectsOfType<CityRoad>();
    }
}
