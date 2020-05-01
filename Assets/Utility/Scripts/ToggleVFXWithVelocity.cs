using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// toggles a VFX graph on or off depending on the speed of the object
/// </summary>
[RequireComponent(typeof(VisualEffect))]
public class ToggleVFXWithVelocity : MonoBehaviour
{
    public float threshold;
    float thresholdSqr => threshold * threshold;

    Rigidbody rigidbody;
    VisualEffect visualEffect;
    bool active;

    void Start()
    {
        rigidbody = GetComponentInParent<Rigidbody>();
        visualEffect = GetComponent<VisualEffect>();
    }

    private void FixedUpdate()
    {
        bool shouldBeActive = rigidbody.velocity.sqrMagnitude >= thresholdSqr;
        if (shouldBeActive && !active)
        {
            active = true;
            visualEffect.Play();
        } else if (!shouldBeActive && active)
        {
            active = false;
            visualEffect.Stop();
        }
    }
}
