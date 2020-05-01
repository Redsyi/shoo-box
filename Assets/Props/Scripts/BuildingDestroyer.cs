using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that can be attached to a physics object, allows that object to destroy buildings
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BuildingDestroyer : MonoBehaviour
{
    public float minVelocity;
    public bool isDonut;
    float minVelocitySqr => minVelocity * minVelocity;
    Rigidbody rigidbody;
    public AK.Wwise.Event groundImpactSound;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //on collision with building above minimum velocity, destroy it
        DestroyBuilding building = collision.gameObject.GetComponent<DestroyBuilding>();
        if (building)
        {
            if (rigidbody.velocity.sqrMagnitude >= minVelocitySqr)
            {
                building.OnKick(gameObject);
                AkEventOnKick soundComponent = building.GetComponent<AkEventOnKick>();
                if (soundComponent)
                {
                    building.GetComponent<AkEventOnKick>().OnKick(gameObject);
                } else
                {
                    Debug.LogWarning($"{building.gameObject.name} has no AkEventOnKick");
                }

                //special case; if we are the donut, and we hit the ice cream shop, award the jibbit
                if (isDonut && !JibbitDonutGiver.given)
                {
                    JibbitDonutGiver donutGiver = building.GetComponent<JibbitDonutGiver>();
                    if (donutGiver)
                    {
                        donutGiver.HitByDonut();
                    }
                }
            }
        } else if (!collision.gameObject.CompareTag("Player"))
        {
            if (rigidbody.velocity.sqrMagnitude >= minVelocitySqr)
            {
                groundImpactSound.Post(gameObject);
            }
        }
    }
}
