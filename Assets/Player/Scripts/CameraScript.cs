﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake
{
    public float remainingTime;
    public float intensity;
}

/// <summary>
/// script that manages the movement of the player camera
/// </summary>
public class CameraScript : MonoBehaviour
{
    private Player player;
    public float cameraSnapRotateSpeed;
    private float remainingRotation;
    private LinkedList<ScreenShake> currShakes;
    private Vector3 currScreenShake;
    public static CameraScript current;
    [HideInInspector]
    public Camera camera;
    private const float shakeUpdateFreq = 0.03f;
    [Header("Zoom")]
    public float closeZoomLevel = 2f;
    public float farZoomLevel = 7f;
    [Tooltip("How long in seconds it takes to switch between the zoom levels")]
    static float zoomTime = 0.75f;
    public bool zoomed => !player.legForm;
    [Tooltip("Range of the possible angles from the ground the camera can be")]
    private Vector2 angleRange = new Vector2(20f, 75f);
    public float anglePercent
    {
        get
        {
            return Mathf.InverseLerp(angleRange.x, angleRange.y, cameraAngle);
        }
    }

    //camera angle: rotation relative to ground
    private float _cameraAngle;
    public float cameraAngle
    {
        get
        {
            return _cameraAngle;
        }
        set
        {
            if (_cameraAngle != value)
            {
                _cameraAngle = value;
                Vector3 eulerAngles = transform.localEulerAngles;
                eulerAngles.x = -value;
                transform.localEulerAngles = eulerAngles;
            }
        }
    }

    //camera rotation: rotation relative to player
    private float _cameraRotation;
    public float cameraRotation
    {
        get
        {
            return _cameraRotation;
        }
        set
        {
            if (_cameraRotation != value)
            {
                _cameraRotation = value;
                Vector3 eulerAngles = transform.localEulerAngles;
                eulerAngles.y = value;
                transform.localEulerAngles = eulerAngles;
            }
        }
    }

    //camera distance: affects clipping
    private float _cameraDist;
    public float cameraDist
    {
        get
        {
            return _cameraDist;
        }
        set
        {
            if (_cameraDist != value)
            {
                _cameraDist = value;
                Vector3 position = camera.transform.localPosition;
                position.z = value;
                camera.transform.localPosition = position;
            }
        }
    }
    public float smoothRotationSpeed;
    bool _cinematicMode;
    public bool cinematicMode
    {
        get
        {
            return _cinematicMode;
        }
        set
        {
            _cinematicMode = value;
            if (value)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            } else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                cameraAngle = originalAngle;
            }
        }
    }
    [HideInInspector] public Vector2 mouseDelta;
    [HideInInspector] public float cinematicAngleDelta;
    [HideInInspector] public float cinematicZoomDelta;
    [HideInInspector] public float cinematicRaiseDelta;

    //cinematic camera properties
    [Header("Cinematic camera")]
    public float cinematicSensitivity = 1f;
    public float cinematicRotateSensitivity = 0.2f;
    public float cinematicAngleSensitivity = 20;
    public float cinematicZoomSensitivity = 0.2f;
    public float cinematicRaiseSensitivity = 0.1f;
    [HideInInspector] public float originalAngle;

    private void Awake()
    {
        _cameraAngle = Utilities.ClampAngle0360(-transform.localEulerAngles.x);
        originalAngle = _cameraAngle;
        _cameraRotation = transform.localEulerAngles.y;
        camera = GetComponentInChildren<Camera>();
        _cameraDist = camera.transform.localPosition.z;
        current = this;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<Player>();
        currShakes = new LinkedList<ScreenShake>();
        StartCoroutine(UpdateScreenShakes());
    }
    
    //Attach to player in lateupdate so there is no visual lag
    void LateUpdate()
    {
        //don't move if cutscene currently using camera or we're in cinematic mode
        if (StealFocusWhenSeen.activeThief == null && !cinematicMode)
        {
            //remainingRotation: deprecated as it was used by snap camera.
            if (remainingRotation != 0f)
            {
                int direction = (remainingRotation > 0f ? 1 : -1);
                float rotationAmount = direction * cameraSnapRotateSpeed * Time.deltaTime;
                if (Mathf.Abs(rotationAmount) > Mathf.Abs(remainingRotation))
                    rotationAmount = remainingRotation;
                remainingRotation -= rotationAmount;
                cameraRotation += rotationAmount;
            }

            //snap to player
            transform.position = (player.legForm ? player.transform.position : player.AISpotPoint.position) + currScreenShake;

            //zoom in/out depending on player legform
            if (!CameraZoomZone.active)
            {
                if (player.legForm && camera.orthographicSize < farZoomLevel)
                {
                    camera.orthographicSize = Mathf.Min(farZoomLevel, camera.orthographicSize + (1 / zoomTime) * Time.deltaTime * (farZoomLevel - closeZoomLevel));
                }
                else if (!player.legForm && camera.orthographicSize > closeZoomLevel)
                {
                    camera.orthographicSize = Mathf.Max(closeZoomLevel, camera.orthographicSize - (1 / zoomTime) * Time.deltaTime * (farZoomLevel - closeZoomLevel));
                }
            }
        }

        //manage cinematic camera movement
        else if (cinematicMode)
        {
            if (mouseDelta != Vector2.zero)
            {
                Vector2 adjustedMovement = Utilities.RotateVectorDegrees(mouseDelta * cinematicSensitivity, 180 - transform.eulerAngles.y);
                mouseDelta = Vector2.zero;
                transform.position += new Vector3(adjustedMovement.x, 0, adjustedMovement.y);
            }
            if (cinematicAngleDelta != 0f)
            {
                float delta = cinematicAngleDelta * Time.unscaledDeltaTime * cinematicAngleSensitivity;
                cameraAngle += delta;
            }
            if (cinematicZoomDelta != 0f)
            {
                float delta = cinematicZoomDelta * Time.unscaledDeltaTime * cinematicZoomSensitivity * -1;
                camera.orthographicSize += delta;
            }
            if (cinematicRaiseDelta != 0f)
            {
                float delta = cinematicRaiseDelta * Time.unscaledDeltaTime * cinematicRaiseSensitivity;
                transform.position += new Vector3(0, delta);
            }
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
    /// smoothly rotate in the specified direction
    /// </summary>
    public void SmoothRotate(float direction)
    {
        if (direction != 0)
        {
            cameraRotation += direction * (cinematicMode ? Time.unscaledDeltaTime : Time.deltaTime) * smoothRotationSpeed * (cinematicMode ? cinematicRotateSensitivity : 1) * PlayerData.sensitivityMultiplier;
        }
    }

    /// <summary>
    /// smoothly rotate the angle to ground in the specified direction
    /// </summary>
    public void SmoothAngleRotate(float direction)
    {
        if (direction != 0)
        {
            cameraAngle = Mathf.Clamp(cameraAngle + direction * (cinematicMode ? Time.unscaledDeltaTime : Time.deltaTime) * smoothRotationSpeed * (cinematicMode ? cinematicRotateSensitivity : 1) * PlayerData.sensitivityMultiplier, angleRange.x, angleRange.y);
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

    /// <summary>
    /// iterate through current screen shakes, using the most powerful one and removing expired ones
    /// </summary>
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
