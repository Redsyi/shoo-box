using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that manages outline thickness depending on camera zoom level
/// </summary>
public class Outlineable : MonoBehaviour
{
    private List<Material> materials;
    public bool animate;
    private Color outlineColor;
    private float currShade;
    private bool animatingToWhite;
    private const float animateRate = 1;
    private CameraScript cameraScript;
    // Start is called before the first frame update
    void Start()
    {
        List<Material> allMats = new List<Material>();
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer)
            GetComponent<MeshRenderer>().GetMaterials(allMats);
        else
            GetComponent<SkinnedMeshRenderer>().GetMaterials(allMats);
        materials = new List<Material>();
        foreach (Material material in allMats)
        {
            if (material.shader.name.Equals("Shader Graphs/Outline"))
            {
                materials.Add(material);
            }
        }
        outlineColor = new Color(0, 0, 0, 1);
        cameraScript = FindObjectOfType<CameraScript>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (animate)
        {
            if (animatingToWhite)
            {
                currShade = Mathf.Min(1, currShade + Time.deltaTime * animateRate);
                if (currShade == 1)
                {
                    animatingToWhite = false;
                }
            }
            else
            {
                currShade = Mathf.Max(0, currShade - Time.deltaTime * animateRate);
                if (currShade == 0)
                {
                    animatingToWhite = true;
                }
            }
            outlineColor.r = outlineColor.g = outlineColor.b = currShade;
        }*/


        foreach (Material material in materials)
        {
            if (cameraScript)
                material.SetFloat("_NormalsSensitivity", (cameraScript.zoomed ? 1 : 0.75f));
            /*if (animate)
                material.SetColor("_OutlineColor", outlineColor);*/
        }
    }
}
