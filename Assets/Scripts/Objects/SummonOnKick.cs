﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonOnKick : MonoBehaviour, IKickable
{
    public AIInterest[] AIs;
    public GameObject to;

    private void Start()
    {
        if (AIs.Length == 0)
            Debug.LogWarning("SummonOnKick component will never summon any AIs");
        if (to == null)
            Debug.LogError("SummonOnKick component has no target to summon AIs to");
    }

    public void OnKick(GameObject kicker)
    {
        AIAgent.SummonAI(to, AIs);
    }
}