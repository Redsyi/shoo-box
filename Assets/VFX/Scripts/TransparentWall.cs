using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manages the transparency of a wall depending on camera angle. use GLOBAL x/z axis when determining which checkboxes to mark.
/// </summary>
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
        //automatically configure intervals based on the checkboxes
        intervals = new List<Vector2>();
        if (facingPositiveZ)
            intervals.Add(new Vector2(270, 90));
        if (facingNegativeZ)
            intervals.Add(new Vector2(90, 270));
        if (facingPositiveX)
            intervals.Add(new Vector2(0, 180));
        if (facingNegativeX)
            intervals.Add(new Vector2(180, 360));

        //fetch materials
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
        //see if current camera rotation is between any of our current intervals, and if so, find the maximum minimum difference between rotation and interval boundaries
        foreach (Vector2 interval in intervals)
        {
            if (debug)
            {
                print($"{cameraRotation}, {interval}");
            }
            if (AngleBetween(cameraRotation, interval))
            {
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

        //set material alphas based on previously determined maximum minimum difference
        foreach (Material material in materials)
        {
            material.SetFloat("_Alpha", Mathf.Lerp(1f, minAlpha, lerpControl));
        }
    }

    /// <summary>
    /// binds an angle between 0 and 360. kinda pointless since we already have Utilities.ClampAngle0360() but i forgot that existed
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    float NormalizeAngle(float angle)
    {
        while (angle < 0)
            angle += 360;
        return angle % 360;
    }

    /// <summary>
    /// returns if an angle is within an interval
    /// </summary>
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

    /// <summary>
    /// returns the minimum difference between an angle and interval bounds
    /// </summary>
    float AngleDiff(float angle, Vector2 interval)
    {
        return Mathf.Min(Mathf.Abs(angle - interval.x), Mathf.Abs(interval.y - angle));
    }
}
