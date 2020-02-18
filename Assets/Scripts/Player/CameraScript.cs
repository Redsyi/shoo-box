using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake
{
    public float remainingTime;
    public float intensity;
}

public class CameraScript : MonoBehaviour
{
    private GameObject player;
    public float cameraSnapRotateSpeed;
    private float remainingRotation;
    private LinkedList<ScreenShake> currShakes;
    private Vector3 currScreenShake;
    public static CameraScript current;
    public Camera camera;
    private const float shakeUpdateFreq = 0.03f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currShakes = new LinkedList<ScreenShake>();
        current = this;
        camera = GetComponentInChildren<Camera>();
        StartCoroutine(UpdateScreenShakes());
    }
    
    //Attach to player in lateupdate so there is no visual lag
    void LateUpdate()
    {
        transform.position = player.transform.position + currScreenShake;
        if (remainingRotation != 0f)
        {
            int direction = (remainingRotation > 0f ? 1 : -1);
            float rotationAmount = direction * cameraSnapRotateSpeed * Time.deltaTime;
            if (Mathf.Abs(rotationAmount) > Mathf.Abs(remainingRotation))
                rotationAmount = remainingRotation;
            remainingRotation -= rotationAmount;
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + rotationAmount, transform.localEulerAngles.z);
        }
    }

    /// <summary>
    /// Rotate the camera 90 degrees in the specified direction
    /// </summary>
    public void Rotate(RotationDirection direction)
    {
        int rotationDirection = (direction == RotationDirection.CLOCKWISE ? -1 : 1);
        if (remainingRotation == 0f)
        {
            remainingRotation = 90f * rotationDirection;
        }
    }

    /// <summary>
    /// Shake the screen for length at strength intensity
    /// </summary>
    public void ShakeScreen(ShakeStrength strength, ShakeLength length)
    {
        ScreenShake newShake = new ScreenShake();
        switch (strength)
        {
            case ShakeStrength.WEAK:
                newShake.intensity = .08f;
                break;
            case ShakeStrength.MEDIUM:
                newShake.intensity = .2f;
                break;
            case ShakeStrength.INTENSE:
                newShake.intensity = .6f;
                break;
        }
        switch (length)
        {
            case ShakeLength.SHORT:
                newShake.remainingTime = 0.09f;
                break;
            case ShakeLength.MEDIUM:
                newShake.remainingTime = 0.2f;
                break;
            case ShakeLength.LONG:
                newShake.remainingTime = 0.6f;
                break;
        }
        currShakes.AddLast(newShake);
    }

    private IEnumerator UpdateScreenShakes()
    {
        float maxIntensity = 0f;
        while (true)
        {
            maxIntensity = 0f;
            LinkedListNode<ScreenShake> node = currShakes.First;
            while (node != null)
            {
                LinkedListNode<ScreenShake> next = node.Next;
                //clear screen shakes that have expired
                if (node.Value.remainingTime <= 0f)
                    currShakes.Remove(node);
                else
                {
                    node.Value.remainingTime -= shakeUpdateFreq;
                    if (node.Value.intensity > maxIntensity)
                        maxIntensity = node.Value.intensity;
                }
                node = next;
            }

            currScreenShake.x = Random.Range(0, maxIntensity);
            currScreenShake.y = Random.Range(0, maxIntensity);
            currScreenShake.z = Random.Range(0, maxIntensity);
            yield return new WaitForSeconds(shakeUpdateFreq);
        }
    }
}
