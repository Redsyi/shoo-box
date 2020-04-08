using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandalProjectile : MonoBehaviour
{
    private bool triggeredSomething;
    private bool reversing;
    public Collider hitbox;
    public float speed = 1;
    [HideInInspector]
    public float maxDist;
    private Vector3 originalPos;
    [HideInInspector]
    public SandalSlinger slinger;
    public Transform target;
    public Transform model;
    public float spinSpeed;

    private void Start()
    {
        originalPos = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        reversing = true;
        hitbox.enabled = false;
        CameraScript.current.ShakeScreen(ShakeStrength.MEDIUM, ShakeLength.SHORT);
        Player.ControllerRumble(RumbleStrength.MEDIUM, 0.2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggeredSomething)
        {
            foreach (ISandalable sandalable in other.GetComponents<ISandalable>())
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
            if (target != null)
            {
                transform.LookAt(target);
            }
            if ((transform.position - originalPos).sqrMagnitude >= Mathf.Pow(maxDist, 2) || (target != null && (transform.position - target.position).sqrMagnitude < 1f))
            {
                reversing = true;
                hitbox.enabled = false;
            }
        }

        transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);
        if (reversing && (transform.position - slinger.transform.position).sqrMagnitude <= 0.5f)
        {
            slinger.rightSandalModel.enabled = true;
            slinger.slinging = false;
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        model.localEulerAngles = new Vector3(0, model.localEulerAngles.y + Time.deltaTime * spinSpeed, 0);
    }
}
