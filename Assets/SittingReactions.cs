﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SittingReactions : MonoBehaviour, IKickable, ISandalable
{
    public Animator animator;
    public void HitBySandal()
    {
        print("Hit by sandal");
        animator.SetTrigger("hitBySandal");
    }

    public void OnKick(GameObject kicker)
    {
        print("Kicked");
        animator.SetTrigger("hitByBoots");
    }
}
