using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedMovableFixable : AnimatedFixable
{
    private Vector3 originalPosition;
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
    }

    public override void AIFinishInteract()
    {
        base.AIFinishInteract();
        transform.position = originalPosition;
    }
}
