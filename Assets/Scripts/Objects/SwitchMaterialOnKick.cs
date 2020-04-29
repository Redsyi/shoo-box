using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMaterialOnKick : MonoBehaviour, IKickable
{
    public Renderer renderer;
    public Material targetMaterial;

    public void OnKick(GameObject kicker)
    {
        renderer.material = targetMaterial;
    }

    private void Start()
    {
        if (renderer == null)
            Debug.LogError("render not attached to materialkick");
    }
}
