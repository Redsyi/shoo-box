using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// occassionally shows a reminder on how to use shoe sight, based on how long it's been since the last time the player used it and how long it's been since the last time the player completed an objective
/// </summary>
public class UIShoeSightReminder : MonoBehaviour
{
    public CanvasGroup group;
    private float timeSinceShoeSightUsed;
    public float shoeSightUseTimeLimit;
    private float timeSinceObjectiveCompleted;
    public float objectiveCompleteTimeLimit;
    public static UIShoeSightReminder instance;
    private bool active;
    private Player player;
    public bool doReminders = true;
    //public AK.Wwise.Event onShoeSight;

    private void Start()
    {
        instance = this;
        player = Player.current;
    }

    void Update()
    {
        if (!player.lockShoeSight)
        {
            timeSinceShoeSightUsed += Time.deltaTime;
        }
        timeSinceObjectiveCompleted += Time.deltaTime;
        bool timeUp = timeSinceObjectiveCompleted >= objectiveCompleteTimeLimit && timeSinceShoeSightUsed >= shoeSightUseTimeLimit;
        if  (timeUp && !active && doReminders)
        {
            group.alpha = 0.8f;
            active = true;
        } else if (active && !timeUp)
        {
            active = false;
            group.alpha = 0f;
        }
    }

    public void ShoeSightUsed()
    {
        timeSinceShoeSightUsed = 0f;
        /*onShoeSight.Post(gameObject);*/
    }

    public void ObjectiveCompleted()
    {
        timeSinceObjectiveCompleted = 0f;
    }

    public void Trigger() {
        timeSinceShoeSightUsed = shoeSightUseTimeLimit;
        timeSinceObjectiveCompleted = objectiveCompleteTimeLimit;
    }
}
