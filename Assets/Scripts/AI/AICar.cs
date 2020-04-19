﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICar : MonoBehaviour, IKickable
{
    public float speed;
    public bool canDrive;
    public float turnRate;

    Direction movingDirection;
    CityRoad currRoad;
    float turnRateMultiplier = 1f;
    [HideInInspector]
    public bool moving;

    void Start()
    {
        switch (transform.localEulerAngles.y)
        {
            case 0:
            case 360:
                movingDirection = Direction.WEST;
                break;
            case 90:
            case -270:
                movingDirection = Direction.NORTH;
                break;
            case 180:
            case -180:
                movingDirection = Direction.EAST;
                break;
            case 270:
            case -90:
                movingDirection = Direction.SOUTH;
                break;
        }

        StartCoroutine(WaitForRoads());
    }

    IEnumerator WaitForRoads()
    {
        yield return null;
        bool foundSuitable = false;
        foreach (CityRoad road in CityRoad.roads)
        {
            if (Utilities.VectorsAreClose(transform.position, road.transform.position, 2.45f))
            {
                currRoad = road;
                foundSuitable = true;
                break;
            }
        }

        if (foundSuitable)
        {
            currRoad.assignedCar[movingDirection] = true;
            transform.position = currRoad.carPositions[movingDirection];
            if (canDrive)
            {
                StartCoroutine(Drive());
                StartCoroutine(AdjustDirection());
            }
        } else
        {
            canDrive = false;
        }
    }

    IEnumerator Drive()
    {
        Vector3 currDest;
        moving = true;
        while (true && canDrive)
        {
            //face the next road in the path
            currDest = currRoad.carPositions[movingDirection];

            //move towards the next road in the path
            while (!Utilities.VectorsAreClose(transform.position, currDest, 0.5f) && canDrive)
            {
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
                yield return null;
            }

            //decide which direction to go next, stop if no directions available
            List<Direction> validTurns = currRoad.AvailableDirections(movingDirection);
            while (validTurns.Count == 0 && canDrive)
            {
                moving = false;
                yield return null;
                validTurns = currRoad.AvailableDirections(movingDirection);
            }

            if (!canDrive)
                break;

            moving = true;

            //take turn (or go straight) based on decided direction, set next road
            int chosenTurn = Random.Range(0, validTurns.Count);
            currRoad.assignedCar[movingDirection] = false;
            currRoad = currRoad.surroundingRoads[movingDirection];
            if (validTurns[chosenTurn] == DirectionUtil.Left(movingDirection))
                StartCoroutine(MakeLeftTurn());
            movingDirection = validTurns[chosenTurn];
            currRoad.assignedCar[movingDirection] = true;
        }
    }

    IEnumerator MakeLeftTurn()
    {
        Vector2 vectToTarget = Utilities.Flatten(currRoad.carPositions[movingDirection] - transform.position);
        float targetAngle = Utilities.ClampAngle0360(Utilities.VectorToDegrees(vectToTarget));
        float currAngle = Utilities.ClampAngle0360(transform.eulerAngles.y);
        if (Utilities.DirectionToRotate(currAngle, targetAngle) == -1)
        {
            turnRateMultiplier = 0;
            yield return new WaitForSeconds(0.5f);
            turnRateMultiplier = 0.3f;
            yield return new WaitForSeconds(0.3f);
            turnRateMultiplier = 1;
        }
    }

    IEnumerator AdjustDirection()
    {
        while (canDrive)
        {
            bool reachedAngle = false;
            while (!reachedAngle)
            {
                Vector2 vectToTarget = Utilities.Flatten(currRoad.carPositions[movingDirection] - transform.position);
                float targetAngle = Utilities.ClampAngle0360(Utilities.VectorToDegrees(vectToTarget));
                float currAngle = Utilities.ClampAngle0360(transform.eulerAngles.y);
                float angleDiff = Mathf.Abs(currAngle - targetAngle);
                if (angleDiff <= (turnRate * turnRateMultiplier * Time.fixedDeltaTime))
                {
                    transform.eulerAngles = new Vector3(0, targetAngle);
                    reachedAngle = true;
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, currAngle + Utilities.DirectionToRotate(currAngle, targetAngle) * turnRate * turnRateMultiplier * Time.deltaTime);
                }
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void OnKick(GameObject kicker)
    {
        if (canDrive)
        {
            Crash();
            CityDirector.current.IncreaseIntensity(0.2f);
        }
    }

    public void Crash()
    {
        if (canDrive)
        {
            canDrive = false;
            moving = false;
            currRoad.assignedCar[movingDirection] = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (canDrive)
        {
            AICar otherCar = collision.gameObject.GetComponent<AICar>();
            if (otherCar && (!otherCar.canDrive || (moving && otherCar.moving)) && Utilities.OnScreen(transform.position))
            {
                Crash();
                GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 600);
            }
            else
            {
                AITank tank = collision.gameObject.GetComponentInParent<AITank>();
                if (tank)
                {
                    Crash();
                }
            }
        }
    }
}
