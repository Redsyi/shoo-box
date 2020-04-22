using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BuildingDestroyer : MonoBehaviour
{
    public float minVelocity;
    float minVelocitySqr => minVelocity * minVelocity;
    Rigidbody rigidbody;
    public AK.Wwise.Event groundImpactSound;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
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
