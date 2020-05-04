using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that can be attached to a physics object, allows that object to destroy buildings
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BuildingDestroyer : MonoBehaviour, IKickable
{
    public float minVelocity;
    public bool isDonut;
    float minVelocitySqr => minVelocity * minVelocity;
    Rigidbody rigidbody;
    public AK.Wwise.Event groundImpactSound;
    bool kicked;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //on collision with kickable above minimum velocity, kick it
        if (kicked)
        {
            IKickable[] kickables = collision.gameObject.GetComponents<IKickable>();
            if (kickables.Length > 0)
            {
                if (rigidbody.velocity.sqrMagnitude >= minVelocitySqr)
                {
                    foreach (IKickable kickable in kickables)
                    {
                        kickable.OnKick(gameObject);
                    }

                    //special case; if we are the donut, and we hit the ice cream shop, award the jibbit
                    if (isDonut && !JibbitDonutGiver.given)
                    {
                        JibbitDonutGiver donutGiver = collision.gameObject.GetComponent<JibbitDonutGiver>();
                        if (donutGiver)
                        {
                            donutGiver.HitByDonut();
                        }
                    }
                }
            }
            else if (!collision.gameObject.CompareTag("Player"))
            {
                if (rigidbody.velocity.sqrMagnitude >= minVelocitySqr)
                {
                    groundImpactSound.Post(gameObject);
                }

                AIHeli helicopter = collision.gameObject.GetComponentInParent<AIHeli>();
                if (helicopter)
                {
                    helicopter.HitBySandal();
                }
            }
        }
    }

    public void OnKick(GameObject kicker)
    {
        kicked = true;
    }
}
