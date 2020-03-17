using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomShoeSight : MonoBehaviour
{
    public ShoeSightType type;
    
    void Start()
    {
        foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
        {
            foreach (Material material in renderer.materials)
            {
                material.SetColor("_SightColor", GetColor());
                material.SetFloat("_UseCustomSightColor", 1);
            }
        }
        foreach (SkinnedMeshRenderer renderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            foreach (Material material in renderer.materials)
            {
                material.SetColor("_SightColor", GetColor());
                material.SetFloat("_UseCustomSightColor", 1);
            }
        }
    }

    private Color GetColor()
    {
        switch (type)
        {
            case ShoeSightType.ENEMY:
                return Color.red;
            case ShoeSightType.INTERACTABLE:
                return Color.green;
            case ShoeSightType.OBJECTIVE:
                return Color.blue;
        }
        return Color.white;
    }
}
