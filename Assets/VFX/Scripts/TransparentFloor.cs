using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manages the transparency of a floor; showing as transparent when player is below and opaque when above
/// </summary>
public class TransparentFloor : MonoBehaviour
{
    [Header("Components")]
    public MeshRenderer transparentRenderer;
    public MeshRenderer opaqueRenderer;
    [Header("Properties")]
    public float yOffset;
    [Range(0, 1)]
    public float minAlpha;
    public float lerpRange;

    float currAlpha;
    List<Material> materials;

    void Start()
    {
        //fetch materials
        materials = new List<Material>();
        materials.AddRange(transparentRenderer.materials);
    }
    
    void Update()
    {
        float playerY = Player.current.transform.position.y;
        float myY = transform.position.y + yOffset;

        //sets alpha to a value between minAlpha and 1, based on the player's position relative to the floor position
        float alpha = Mathf.Lerp(minAlpha, 1, Mathf.InverseLerp(myY - lerpRange, myY, Mathf.Clamp(playerY, myY - lerpRange, myY)));
        if (alpha != currAlpha)
        {
            currAlpha = alpha;
            foreach (Material material in materials)
            {
                material.SetFloat("_Alpha", alpha);
            }
            if (alpha == 1f && transparentRenderer.enabled)
            {
                transparentRenderer.enabled = false;
                opaqueRenderer.enabled = true;
            }
            else if (alpha < 1f && opaqueRenderer.enabled)
            {
                transparentRenderer.enabled = true;
                opaqueRenderer.enabled = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 border = transform.position + new Vector3(0, yOffset - lerpRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, border);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(border, transform.position + new Vector3(0, yOffset));
    }
}
