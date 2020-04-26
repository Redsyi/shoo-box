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
    public float zoomTime = 0.3f;
    public bool zoomed => !player.legForm;
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
            } else
            {
                Cursor.lockState = CursorLockMode.None;
                cameraAngle = originalAngle;
            }
        }
    }
    [HideInInspector] public Vector2 mouseDelta;
    [HideInInspector] public float cinematicAngleDelta;
    [HideInInspector] public float cinematicZoomDelta;
    [HideInInspector] public float cinematicRaiseDelta;
    public float cinematicSensitivity = 1f;
    public float cinematicRotateSensitivity = 0.2f;
    public float cinematicAngleSensitivity = 20;
    public float cinematicZoomSensitivity = 0.2f;
    public float cinematicRaiseSensitivity = 0.1f;
    [HideInInspector] public float originalAngle;

    private void Awake()
    {
        _cameraAngle = -transform.localEulerAngles.x;
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
        if (StealFocusWhenSeen.activeThief == null && !cinematicMode)
        {
            if (remainingRotation != 0f)
            {
                int direction = (remainingRotation > 0f ? 1 : -1);
                float rotationAmount = direction * cameraSnapRotateSpeed * Time.deltaTime;
                if (Mathf.Abs(rotationAmount) > Mathf.Abs(remainingRotation))
                    rotationAmount = remainingRotation;
                remainingRotation -= rotationAmount;
                cameraRotation += rotationAmount;
            }

            transform.position = player.transform.position + currScreenShake;

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
                float delta = cinematicAngleDelta * Time.deltaTime * cinematicAngleSensitivity;
                cameraAngle += delta;
            }
            if (cinematicZoomDelta != 0f)
            {
                float delta = cinematicZoomDelta * Time.deltaTime * cinematicZoomSensitivity * -1;
                camera.orthographicSize += delta;
            }
            if (cinematicRaiseDelta != 0f)
            {
                float delta = cinematicRaiseDelta * Time.deltaTime * cinematicRaiseSensitivity;
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

    public void SmoothRotate(float direction)
    {
        if (direction != 0)
        {
            cameraRotation += direction * Time.deltaTime * smoothRotationSpeed * (cinematicMode ? cinematicRotateSensitivity : 1);
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
