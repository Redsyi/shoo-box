using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using UnityEngine;

public class MultiKickable : MonoBehaviour, IKickable
{
    private int _currKick;
    private Animator animator;

    [SerializeField] private float animSpeed = 1f;
    [SerializeField] private float animBackwardsSpeed = -1.5f;
    [SerializeField] private int maxKicks;
    
    
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    
    public void OnKick(GameObject kicker)
    {
        SetCurrentKick(_currKick + 1);
        PlayCurrentKick(animSpeed);
    }

    private void SetCurrentKick(int currKick)
    {
        _currKick = currKick;
        _currKick = Math.Min(_currKick, maxKicks);
        _currKick = Math.Max(_currKick, 0);
    }

    private void PlayCurrentKick(float speed)
    {
        animator.SetFloat("speed", speed);
        animator.SetInteger("state", _currKick);
    }

    public void UndoCurrentKick()
    {
        SetCurrentKick(_currKick - 1);
        PlayCurrentKick(animBackwardsSpeed);
    }
    
}
