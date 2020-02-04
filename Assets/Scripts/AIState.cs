using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateNode
{
    public AIState state;
    public Transform location;
}

public enum AIState { IDLE, INVESTIGATE, INVESTIGATING, CHASE, INTERACT}