using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// class to manage the "?" / "!" bubbles for NPCs
/// </summary>
public class UINPCBubble : MonoBehaviour
{
    public Transform worldAnchor;
    public GameObject spottedBubble;
    public GameObject investigateBubble;
    public RectTransform investigateBG;
    public Vector2 investigateOffset;
    public Vector2 offset;
    public Vector2 investigateBounds;
    private bool spotting;
    private bool investigating => (shouldInvestigate || stealthProgress > 0) && !spotting;
    public GameObject stealthMeter;
    public Image stealthProgressMeter;
    private float investigateAngleOffset;
    public GameObject investigateArrow;
    private float _stealthProgress;
    public float stealthProgress
    {
        get
        {
            return _stealthProgress;
        }
        set
        {
            _stealthProgress = value;
            stealthMeter.SetActive(value > 0);
            stealthProgressMeter.fillAmount = value;
        }
    }
    private bool shouldInvestigate;
    public float distanceScaleRange;
    public Vector2 scaleRange;

    private void Start()
    {
        investigateAngleOffset = investigateArrow.transform.localEulerAngles.z;
        stealthProgress = 0f;
    }

    public void Spotted()
    {
        spotting = true;
        spottedBubble.SetActive(true);
    }

    public void Lost()
    {
        spotting = false;
        spottedBubble.SetActive(false);
    }

    public void Investigating()
    {
        shouldInvestigate = true;
    }

    public void StopInvestigating()
    {
        shouldInvestigate = false;
    }

    /// <summary>
    /// sets position to the screen position of the NPC...
    /// </summary>
    private void LateUpdate()
    {
        investigateBubble.SetActive(investigating);
        if (worldAnchor != null) {
            if (spotting || investigating)
            {
                Vector2 desiredPoint = (Vector2)CameraScript.current.camera.WorldToScreenPoint(worldAnchor.position) + offset;
                if (spotting)
                {
                    transform.position = desiredPoint;
                } else
                //...but if it's the "?" and it's off-screen, put it on-screen and point the arrow at the npc
                {
                    Vector2 desiredPosition = desiredPoint + investigateOffset;
                    Vector2 screenSize = new Vector2(CameraScript.current.camera.pixelWidth, CameraScript.current.camera.pixelHeight);
                    Vector2 actualPoint = desiredPosition;
                    actualPoint.x = Mathf.Max(investigateBounds.x, actualPoint.x);
                    actualPoint.x = Mathf.Min(screenSize.x - investigateBounds.x, actualPoint.x);
                    actualPoint.y = Mathf.Max(investigateBounds.y, actualPoint.y);
                    actualPoint.y = Mathf.Min(screenSize.y - investigateBounds.y, actualPoint.y);
                    
                    //if off-screen, lerp scale
                    float distOffScreen = (actualPoint != desiredPosition ? (actualPoint - desiredPosition).magnitude : 0);
                    float scaleLerpValue = Mathf.InverseLerp(distanceScaleRange, 0, distOffScreen);
                    float scale = Mathf.Lerp(scaleRange.x, scaleRange.y, scaleLerpValue);
                    transform.localScale = new Vector3(scale, scale, scale);

                    if (actualPoint.x == investigateBounds.x)
                        actualPoint.x = investigateBounds.x * scale;
                    if (actualPoint.x == screenSize.x - investigateBounds.x)
                        actualPoint.x = screenSize.x - investigateBounds.x * scale;
                    if (actualPoint.y == investigateBounds.y)
                        actualPoint.y = investigateBounds.y * scale;
                    if (actualPoint.y == screenSize.y - investigateBounds.y)
                        actualPoint.y = screenSize.y - investigateBounds.y * scale;

                    float angle = Utilities.VectorToDegrees(desiredPoint - actualPoint);
                    investigateArrow.transform.localEulerAngles = new Vector3(0, 0, angle + investigateAngleOffset);
                    transform.position = actualPoint;
                }
            }
        } else
        {
            Destroy(gameObject);
        }
    }
}
