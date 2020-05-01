using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that facilitates AI behavior and movement in the city
/// </summary>
public class CityRoad : MonoBehaviour
{
    public Dictionary<Direction, bool> canGo;                   //whether cars are allowed to travel in this direction - this can be controlled by traffic lights
    public Dictionary<Direction, bool> assignedCar;             //whether there is a car traveling the specified direction on this road
    public Dictionary<Direction, Vector3> carPositions;         //world-space positions of where cars traveling given directions should be
    public Dictionary<Direction, CityRoad> surroundingRoads;    //references to the roads immediately surrounding this one
    [HideInInspector]
    public bool assignedHeli;                                   //whether this road segment has been selected as a destination by a helicopter
    [HideInInspector]
    public bool assignedTank;                                   //see above, but for tanks

    public static CityRoad[] roads;                             //static array of all roads in the city
    const float posOffset = 1.3f;
    RoadPlayerDetector[] playerDetectors;

    private void Awake()
    {
        canGo = new Dictionary<Direction, bool>();
        DirectionUtil.FillDict(canGo, true);
        assignedCar = new Dictionary<Direction, bool>();
        DirectionUtil.FillDict(assignedCar, false);
        carPositions = new Dictionary<Direction, Vector3>();
        FillCarPositions();
        surroundingRoads = new Dictionary<Direction, CityRoad>();
        DirectionUtil.FillDict(surroundingRoads, null);

        playerDetectors = GetComponentsInChildren<RoadPlayerDetector>();
        foreach (RoadPlayerDetector detector in playerDetectors)
        {
            if (detector.transform.position.z > transform.position.z)
                detector.vBlock = Direction.SOUTH;
            else
                detector.vBlock = Direction.NORTH;
            if (detector.transform.position.x < transform.position.x)
                detector.hBlock = Direction.EAST;
            else
                detector.hBlock = Direction.WEST;
        }
    }

    /// <summary>
    /// Initialize the static road network
    /// </summary>
    private void Start()
    {
        if (roads == null || roads.Length == 0)
            roads = FindObjectsOfType<CityRoad>();
        FindSurroundingRoads();
        StartCoroutine(SleepPlayerDetectors());
    }

    /// <summary>
    /// Sets the positions cars should aim for on this road in each direction
    /// </summary>
    void FillCarPositions()
    {
        carPositions[Direction.EAST] = transform.position + new Vector3(-posOffset, 0, -posOffset);
        carPositions[Direction.WEST] = transform.position + new Vector3(posOffset, 0, posOffset);
        carPositions[Direction.NORTH] = transform.position + new Vector3(posOffset, 0, -posOffset);
        carPositions[Direction.SOUTH] = transform.position + new Vector3(-posOffset, 0, posOffset);
    }

    /// <summary>
    /// Finds neighbors from the static road network
    /// </summary>
    void FindSurroundingRoads()
    {
        foreach (CityRoad road in roads)
        {
            if (IsAdjacent(road, Direction.NORTH))
                surroundingRoads[Direction.NORTH] = road;
            else if (IsAdjacent(road, Direction.EAST))
                surroundingRoads[Direction.EAST] = road;
            else if (IsAdjacent(road, Direction.SOUTH))
                surroundingRoads[Direction.SOUTH] = road;
            else if (IsAdjacent(road, Direction.WEST))
                surroundingRoads[Direction.WEST] = road;
        }
    }

    /// <summary>
    /// returns if the other CityRoad is adjacent to this one in the given direction
    /// </summary>
    bool IsAdjacent(CityRoad other, Direction direction)
    {
        float tolerance = 1;
        Vector3 offset;
        switch (direction)
        {
            case Direction.NORTH:
                offset = new Vector3(5f, 0);
                break;
            case Direction.EAST:
                offset = new Vector3(0, 0, -5f);
                break;
            case Direction.SOUTH:
                offset = new Vector3(-5f, 0);
                break;
            case Direction.WEST:
                offset = new Vector3(0, 0, 5f);
                break;
            default:
                offset = Vector3.zero;
                break;
        }

        Vector3 offsetPos = transform.position + offset;
        return Utilities.VectorsAreClose(other.transform.position, offsetPos, tolerance);
    }

    private void OnDestroy()
    {
        if (roads != null)
            roads = null;
    }

    /// <summary>
    /// returns if it's physically for a car to go the specified direction on this road, not considering red lights
    /// </summary>
    public bool CanPass(Direction direction)
    {
        return !assignedCar[direction] && surroundingRoads[direction] != null;
    }

    /// <summary>
    /// Returns a list of directions a car can go from this road, given they are heading currDirection
    /// </summary>
    public List<Direction> AvailableDirections(Direction currDirection)
    {
        List<Direction> available = new List<Direction>();
        foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
        {
            CityRoad nextRoad = surroundingRoads[currDirection];
            if (direction != DirectionUtil.Opposite(currDirection) && nextRoad != null)
            {
                if (nextRoad.canGo[currDirection] && //make sure you're allowed to go into the road tile
                                                     //conditions to turn left: either the opposite side has a red, the opposite side doesn't exist, or the opposite side has no car on it
                        (direction != DirectionUtil.Left(currDirection) ||
                        !nextRoad.canGo[DirectionUtil.Opposite(currDirection)] ||
                        !nextRoad.surroundingRoads[currDirection] ||
                            (!nextRoad.surroundingRoads[currDirection].assignedCar[DirectionUtil.Opposite(currDirection)]) &&
                            !nextRoad.assignedCar[DirectionUtil.Opposite(currDirection)] &&
                            !nextRoad.assignedCar[DirectionUtil.Left(currDirection)] &&
                            !nextRoad.assignedCar[DirectionUtil.Right(currDirection)])
                    && nextRoad.CanPass(direction)
                    && !nextRoad.assignedTank
                    && !nextRoad.BlockedByPlayer(currDirection)
                    && !nextRoad.BlockedByPlayer(direction)
                    )
                {
                    available.Add(direction);
                }
            }
        }
        return available;
    }
    
    /// <summary>
    /// Draws the calculated road paths
    /// </summary>
    private void OnDrawGizmos()
    {
        if (surroundingRoads != null)
        {
            if (surroundingRoads[Direction.EAST] != null)
            {
                Gizmos.color = (canGo[Direction.EAST] ? Color.green : Color.red);
                Gizmos.DrawRay(carPositions[Direction.EAST] + Vector3.up, Vector3.back * 1.5f);
                Gizmos.DrawSphere(carPositions[Direction.EAST] + Vector3.up + Vector3.back * 1.5f, .25f);
            }
            if (surroundingRoads[Direction.WEST] != null)
            {
                Gizmos.color = (canGo[Direction.WEST] ? Color.green : Color.red);
                Gizmos.DrawRay(carPositions[Direction.WEST] + Vector3.up, Vector3.forward * 1.5f);
                Gizmos.DrawSphere(carPositions[Direction.WEST] + Vector3.up + Vector3.forward * 1.5f, .25f);
            }
            if (surroundingRoads[Direction.NORTH] != null)
            {
                Gizmos.color = (canGo[Direction.NORTH] ? Color.green : Color.red);
                Gizmos.DrawRay(carPositions[Direction.NORTH] + Vector3.up, Vector3.right * 1.5f);
                Gizmos.DrawSphere(carPositions[Direction.NORTH] + Vector3.up + Vector3.right * 1.5f, .25f);
            }
            if (surroundingRoads[Direction.SOUTH] != null)
            {
                Gizmos.color = (canGo[Direction.SOUTH] ? Color.green : Color.red);
                Gizmos.DrawRay(carPositions[Direction.SOUTH] + Vector3.up, Vector3.left * 1.5f);
                Gizmos.DrawSphere(carPositions[Direction.SOUTH] + Vector3.up + Vector3.left * 1.5f, .25f);
            }

            Gizmos.color = Color.cyan;
            if (assignedCar[Direction.EAST])
                Gizmos.DrawCube(carPositions[Direction.EAST], Vector3.one);
            if (assignedCar[Direction.WEST])
                Gizmos.DrawCube(carPositions[Direction.WEST], Vector3.one);
            if (assignedCar[Direction.NORTH])
                Gizmos.DrawCube(carPositions[Direction.NORTH], Vector3.one);
            if (assignedCar[Direction.SOUTH])
                Gizmos.DrawCube(carPositions[Direction.SOUTH], Vector3.one);
        }
    }

    /// <summary>
    /// Returns true if this road is blocked by the player in the given direction
    /// </summary>
    public bool BlockedByPlayer(Direction dir)
    {
        foreach (RoadPlayerDetector detector in playerDetectors)
        {
            if (detector.IsBlocking(dir))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Disables player detector colliders if we're far from the player to cut down on physics overhead
    /// </summary>
    IEnumerator SleepPlayerDetectors()
    {
        bool sleeping = false;
        while (true)
        {
            Vector3 distToPlayer = transform.position - Player.current.transform.position;
            bool far = distToPlayer.sqrMagnitude > 180;
            if (far && !sleeping)
            {
                sleeping = true;
                foreach (RoadPlayerDetector detector in playerDetectors)
                {
                    detector.Sleep();
                }
            }
            else if (!far && sleeping)
            {
                sleeping = false;
                foreach (RoadPlayerDetector detector in playerDetectors)
                {
                    detector.Wake();
                }
            }
            yield return new WaitForSeconds(.15f);
        }
    }
}
