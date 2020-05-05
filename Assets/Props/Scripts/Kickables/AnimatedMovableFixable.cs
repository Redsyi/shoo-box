using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// subclass of AnimatedFixable that also resets its original position when fixed
/// </summary>
public class AnimatedMovableFixable : AnimatedFixable
{
    private Vector3 originalPosition;
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
    }

    public override void AIFinishInteract(AIAgent ai)
    {
        base.AIFinishInteract(ai);
        transform.position = originalPosition;
    }
}
