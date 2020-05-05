using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBreakable : MonoBehaviour, IAIInteractable
{
    private bool broken;
    public Material fixedMaterial;
    public Material brokenMaterial;
    public MeshRenderer renderer;
    public AIInterest[] interests;

    private void Start()
    {
        renderer.material = fixedMaterial;
    }

    public void AIFinishInteract(AIAgent ai)
    {
        broken = false;
        renderer.material = fixedMaterial;
    }

    public void AIInteracting(float interactProgress)
    {

    }

    public float AIInteractTime()
    {
        return 2.5f;
    }

    public AIInterest[] InterestingToWhatAI()
    {
        return interests;
    }

    public bool NeedsInteraction()
    {
        return broken;
    }

    public void Break()
    {
        renderer.material = brokenMaterial;
        broken = true;
    }
}
