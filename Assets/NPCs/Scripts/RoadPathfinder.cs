using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class to assist with road-based pathfinding in the city
/// automatically updates pathfinding when destination is assigned
/// </summary>
public class RoadPathfinder : MonoBehaviour
{
    public float speed;
    public float turnRate;
    public float timeToFullSpeed;
    public bool debug;

    CityRoad _currRoad;
    CityRoad currRoad
    {
        get
        {
            return _currRoad;
        }
        set
        {
            if (value != _currRoad)
            {
                _currRoad = value;
                RefetchNextRoad();
            }
        }
    }
    CityRoad _destination;
    public CityRoad destination
    {
        get
        {
            return _destination;
        }
        set
        {
            if (value != _destination)
            {
                _destination = value;
                RefetchNextRoad();
            }
        }
    }
    int nextRoad = -1;
    float speedMultiplier = 1f;

    private void Start()
    {
        StartCoroutine(AdjustDirection());
    }

    private void FixedUpdate()
    {
        if (CityRoad.finishedBuildingPathfinders)
        {
            //figure out which road we are on right now, if not the current one
            if (!currRoad || !Utilities.VectorsAreClose(transform.position, currRoad.transform.position, 2.49f))
            {
                //check roads immediately surrounding our previous road first
                bool foundNextToCurrent = false;
                if (currRoad)
                {
                    foreach (CityRoad road in currRoad.surroundingRoads.Values)
                    {
                        if (road && Utilities.VectorsAreClose(transform.position, road.transform.position, 2.49f))
                        {
                            currRoad = road;
                            foundNextToCurrent = true;
                            break;
                        }
                    }
                }

                //otherwise check the other roads
                if (!foundNextToCurrent)
                {
                    foreach (CityRoad road in CityRoad.roads)
                    {
                        if (Utilities.VectorsAreClose(transform.position, road.transform.position, 2.49f))
                        {
                            currRoad = road;
                            break;
                        }
                    }
                }
            }

            //manage acceleration/deceleration
            if (nextRoad != -1)
            {
                //decelerate if we are on our destination tile
                if (CloseToDestination())
                {
                    if (AtDestination())
                    {
                        speedMultiplier = Mathf.Clamp01(speedMultiplier - Time.fixedDeltaTime / timeToFullSpeed);
                    } else
                    {
                        speedMultiplier = Mathf.Clamp(speedMultiplier - Time.fixedDeltaTime / timeToFullSpeed, 0.3f, 1f);
                    }
                }
                //otherwise, accelerate
                else
                {
                    speedMultiplier = Mathf.Clamp01(speedMultiplier + Time.fixedDeltaTime / timeToFullSpeed);
                }
            }

            //drive forwards
            transform.Translate(Vector3.forward * speed * speedMultiplier * Time.fixedDeltaTime, Space.Self);
        }
    }


    /// <summary>
    /// returns if this pathfinder has reached its destination
    /// </summary>
    public bool AtDestination()
    {
        return (nextRoad != -1 && nextRoad == currRoad.pathfindingID);
    }

    /// <summary>
    /// returns if this pathfinder is on or adjacent to its destination
    /// </summary>
    public bool CloseToDestination()
    {
        return AtDestination() || (destination && nextRoad == destination.pathfindingID);
    }
    
    /// <summary>
    /// turns the pathfinder towards its current destination
    /// </summary>
    IEnumerator AdjustDirection()
    {
        while (true)
        {
            //don't turn if pathfinder isn't running
            while (!enabled || nextRoad == -1 || !CityRoad.finishedBuildingPathfinders)
                yield return new WaitForSeconds(0.1f);
            
            bool reachedAngle = false;
            while (!reachedAngle)
            {
                Vector2 vectToTarget = Utilities.Flatten(CityRoad.roads[nextRoad].transform.position - transform.position);
                float targetAngle = Utilities.ClampAngle0360(Utilities.VectorToDegrees(vectToTarget));
                float currAngle = Utilities.ClampAngle0360(transform.eulerAngles.y);
                float angleDiff = Mathf.Abs(currAngle - targetAngle);
                if (angleDiff <= (turnRate * Time.fixedDeltaTime))
                {
                    transform.eulerAngles = new Vector3(0, targetAngle);
                    reachedAngle = true;
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, currAngle + Utilities.DirectionToRotate(currAngle, targetAngle) * turnRate * Time.deltaTime);
                }
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }


    /// <summary>
    /// sets the next road in the path
    /// </summary>
    void RefetchNextRoad()
    {
        if (CityRoad.finishedBuildingPathfinders && destination && currRoad) {
            nextRoad = currRoad.nextRoadTo[destination.pathfindingID];
        } else
        {
            nextRoad = -1;
        }
    }

    private void OnDrawGizmos()
    {
        if (debug && currRoad && destination && nextRoad != -1)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(currRoad.transform.position, 2.5f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(CityRoad.roads[nextRoad].transform.position, 2.5f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(CityRoad.currPlayerRoad.transform.position, 2.5f);
        }
    }
}
