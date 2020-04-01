﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public AK.Wwise.Event onShoeSight;

    private void Start()
    {
        instance = this;
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        if (!player.lockShoeSight)
        {
            timeSinceShoeSightUsed += Time.deltaTime;
        }
        timeSinceObjectiveCompleted += Time.deltaTime;
        bool timeUp = timeSinceObjectiveCompleted >= objectiveCompleteTimeLimit && timeSinceShoeSightUsed >= shoeSightUseTimeLimit;
        if  (timeUp && !active)
        {
            group.alpha = 1f;
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
        onShoeSight.Post(gameObject);
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