using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// component that detects the player on the attached road - and which directions the player is blocking
/// </summary>
public class RoadPlayerDetector : MonoBehaviour
{
    bool playerBlocking;
    [HideInInspector] public Direction vBlock;
    [HideInInspector] public Direction hBlock;
    Collider collider;

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }

    private void OnTriggerStay(Collider other)
    {
        playerBlocking = true;
    }

    private void OnTriggerExit(Collider other)
    {
        playerBlocking = false;
    }

    public bool IsBlocking(Direction direction)
    {
        return playerBlocking && (direction == hBlock || direction == vBlock);
    }

    public void Wake()
    {
        collider.enabled = true;
    }

    public void Sleep()
    {
        collider.enabled = false;
        playerBlocking = false;
    }
}
