using UnityEngine;

/// <summary>
/// interface attached to anything that can be kicked. also needs a trigger collider and rigidbody.
/// </summary>
public interface IKickable
{
    void OnKick(GameObject kicker);
}