using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandalProjectile : MonoBehaviour
{
    private bool triggeredSomething;
    private bool reversing;
    public Collider hitbox;
    public float speed = 1;
    public float maxDist = 30f;
    private Vector3 originalPos;
    [HideInInspector]
    public SandalSlinger slinger;

    private void Start()
    {
        originalPos = transform.position;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.isTrigger)
        {
            reversing = true;
            hitbox.enabled = false;
        }

        if (!triggeredSomething)
        {
            foreach (ISandalable sandalable in collision.collider.GetComponents<ISandalable>())
            {
                triggeredSomething = true;
                sandalable.HitBySandal();
            }
        }
    }

    private void FixedUpdate()
    {
        if (reversing)
        {
            transform.LookAt(slinger.transform);
        } else
        {
            if ((transform.position - originalPos).sqrMagnitude >= Mathf.Pow(maxDist, 2))
            {
                reversing = true;
                hitbox.enabled = false;
            }
        }

        transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);
        if (reversing && (transform.position - slinger.transform.position).sqrMagnitude <= 0.5f)
        {
            Destroy(gameObject);
        }
    }
}
