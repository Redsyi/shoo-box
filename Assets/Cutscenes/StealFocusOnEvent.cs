using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// variant of StealFocusWhenSeen that has a delay option
/// </summary>
public class StealFocusOnEvent : StealFocusWhenSeen
{

    [SerializeField] private float delay;
    protected override void Update()
    {

    }

    public override void Trigger()
    {
        if (!focusStolen && activeThief == null)
        {
            if (delay <= 0)
            {
                StartCoroutine(StealFocus());
            } else
            {
                StartCoroutine(DelayedStealFocus(delay));
            }
        }
    }

    private IEnumerator DelayedStealFocus(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        StartCoroutine(StealFocus());
    }
}
