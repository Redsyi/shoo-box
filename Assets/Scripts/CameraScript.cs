﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private GameObject player;
    public enum Direction { NORTH, EAST, SOUTH, WEST};
    private Direction currDirection;
    public enum RotationDirection { CLOCKWISE, COUNTERCLOCKWISE};
    public float cameraSnapRotateSpeed;
    private float remainingRotation;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = player.transform.position;
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

    public void Rotate(RotationDirection direction)
    {
        int rotationDirection = (direction == RotationDirection.CLOCKWISE ? -1 : 1);
        if (remainingRotation == 0f)
        {
            remainingRotation = 90f * rotationDirection;
        }
    }
}
