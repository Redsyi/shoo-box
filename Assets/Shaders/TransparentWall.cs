using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentWall : MonoBehaviour
{
    [Header("Facing Directions")]
    public bool facingPositiveZ;
    public bool facingNegativeZ;
    public bool facingPositiveX;
    public bool facingNegativeX;

    private List<Vector2> intervals;
    private const float lerpRange = 50;
    private const float minAlpha = 0.55f;
    private List<Material> materials;

    public bool debug;

    void Start()
    {
        intervals = new List<Vector2>();
        if (facingPositiveZ)
            intervals.Add(new Vector2(290, 70));
        if (facingNegativeZ)
            intervals.Add(new Vector2(110, 250));
        if (facingPositiveX)
            intervals.Add(new Vector2(20, 160));
        if (facingNegativeX)
            intervals.Add(new Vector2(200, 340));

        materials = new List<Material>();
        foreach(MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
        {
            materials.AddRange(renderer.materials);
        }
    }
    
    void Update()
    {
        float maxDiff = 0;
        float cameraRotation = NormalizeAngle(CameraScript.current.cameraRotation);
        foreach (Vector2 interval in intervals)
        {
            if (AngleBetween(cameraRotation, interval))
            {
                if (debug)
                {
                    print("yes");
                }
                float diff = AngleDiff(cameraRotation, interval);
                if (diff > maxDiff)
                {
                    maxDiff = diff;
                }
            }
        }
        if(debug)
        print(maxDiff);
        float lerpControl = Mathf.Clamp01(maxDiff / lerpRange);
        if (debug)
            print($"lerping: {lerpControl}");
        foreach (Material material in materials)
        {
            material.SetFloat("_Alpha", Mathf.Lerp(1f, minAlpha, lerpControl));
        }
    }

    float NormalizeAngle(float angle)
    {
        return (angle + 360) % 360;
    }

    bool AngleBetween(float angle, Vector2 interval)
    {
        if (debug)
        {
            print($"Is {angle} between {interval}?");
        }
        if (interval.x < interval.y)
            return (angle >= interval.x && angle <= interval.y);
        else
            return (angle >= interval.x || angle <= interval.y);
    }

    float AngleDiff(float angle, Vector2 interval)
    {
        return Mathf.Min(Mathf.Abs(angle - interval.x), Mathf.Abs(interval.y - angle));
    }
}
