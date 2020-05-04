using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// class to manage the behavior of a police car in the city
/// </summary>
public class AIPolice : MonoBehaviour, IKickable
{
    [Header("Components")]
    public AICar carAI;
    public RoadPathfinder pathfinder;
    public Animator sirenAnimator;
    [Header("Sounds")]
    public AK.Wwise.Event sirenStartEvent;
    public AK.Wwise.Event sirenStopEvent;

    bool engaged;
    bool destroyed;
    bool _arresting;
    bool arresting
    {
        get
        {
            return _arresting;
        }
        set
        {
            if (value != _arresting)
            {
                _arresting = value;
                if (value)
                    CityPlayerHelper.numArresters++;
                else
                    CityPlayerHelper.numArresters--;
            }
        }
    }


    void Update()
    {
        //check if we need to switch to chasing behavior
        if (!engaged && !destroyed && CityDirector.current.intensity > 0)
        {
            EngagePlayer();
        }

        //see if we're arresting the player
        arresting = (engaged && pathfinder.AtDestination());
    }

    public void OnKick(GameObject kicker)
    {
        Destroy();
    }

    /// <summary>
    /// notifies this car to go into "cop mode", attempting to chase down the player
    /// </summary>
    public void EngagePlayer()
    {
        sirenStartEvent.Post(gameObject);
        CityDirector.numCops++;
        engaged = true;
        carAI.canDrive = false;
        carAI.enabled = false;
        pathfinder.enabled = true;
        StartCoroutine(UpdatePathfinder());
        sirenAnimator.SetBool("On", true);
    }

    /// <summary>
    /// Destroy this police car
    /// </summary>
    public void Destroy()
    {
        if (!destroyed)
        {
            sirenStopEvent.Post(gameObject);
            if (engaged)
                CityDirector.numCops--;
            destroyed = true;
            engaged = false;
            pathfinder.enabled = false;
            CityDirector.current.IncreaseIntensity(0.2f);
            sirenAnimator.SetBool("On", false);
        }
    }

    /// <summary>
    /// periodically updates the navmeshagent's destination to be the player's position
    /// </summary>
    IEnumerator UpdatePathfinder()
    {
        while (engaged)
        {
            pathfinder.destination = CityRoad.currPlayerRoad;
            yield return new WaitForSeconds(1f);
        }
    }
}
