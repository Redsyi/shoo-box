using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityIntersection : MonoBehaviour
{
    public bool assignedHeli;
    public bool assignedTank;
    public static CityIntersection[] intersections;

    private void Start()
    {
        intersections = FindObjectsOfType<CityIntersection>();
    }
}
