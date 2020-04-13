using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class Toilet : MonoBehaviour, IKickable, IAIInteractable
{
    public AIInterest[] interestMask;
    public float fixTime;

    bool broken;
    VisualEffect particles;

    private void Start()
    {
        particles = GetComponent<VisualEffect>();
    }

    public void AIFinishInteract()
    {
        broken = false;
        particles.Stop();
    }

    public void AIInteracting(float interactProgress)
    {
    }

    public float AIInteractTime()
    {
        return fixTime;
    }

    public AIInterest[] InterestingToWhatAI()
    {
        return interestMask;
    }

    public bool NeedsInteraction()
    {
        return broken;
    }

    public void OnKick(GameObject kicker)
    {
        broken = true;
        particles.Play();
    }
}
