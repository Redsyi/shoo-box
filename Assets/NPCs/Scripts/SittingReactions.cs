﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that sets animation triggers based on getting kicked or hit by a sandal
/// </summary>
public class SittingReactions : MonoBehaviour, IKickable, ISandalable
{
    public Animator animator;
    public void HitBySandal()
    {
        animator.SetTrigger("hitBySandal");
    }

    public void OnKick(GameObject kicker)
    {
        animator.SetTrigger("hitByBoots");
    }
}
