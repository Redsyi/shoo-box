using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// plays a vfx graph on kick
/// </summary>
public class PlayEffectOnKick : MonoBehaviour, IKickable
{
    public VisualEffect effect;

    void Start()
    {
        if (!effect)
            effect = GetComponent<VisualEffect>();
    }

    public void OnKick(GameObject kicker)
    {
        effect.Play();
    }
}
