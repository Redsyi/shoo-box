using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomShoeSight : MonoBehaviour
{
    public ShoeSightType type;
    private List<Material> materials;
    
    void Start()
    {
        materials = new List<Material>();
        foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
        {
            foreach (Material material in renderer.materials)
            {
                material.SetColor("_SightColor", GetColor());
                material.SetFloat("_UseCustomSightColor", 1);
                materials.Add(material);
            }
        }
        foreach (SkinnedMeshRenderer renderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            foreach (Material material in renderer.materials)
            {
                material.SetColor("_SightColor", GetColor());
                material.SetFloat("_UseCustomSightColor", 1);
                materials.Add(material);
            }
        }
        SetType(type);
    }

    public void SetType(ShoeSightType newType)
    {
        if (type != newType)
        {
            type = newType;
            foreach (Material material in materials)
            {
                material.SetColor("_SightColor", GetColor());
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
            case ShoeSightType.BLIND_ENEMY:
                return Color.yellow;
        }
        return Color.white;
    }
}
