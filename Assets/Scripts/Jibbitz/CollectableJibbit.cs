using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CollectableJibbit : MonoBehaviour
{
    public Rigidbody rigidbody;
    public VisualEffect effect;
    public TrailRenderer line;
    public AK.Wwise.Event collectSound;
    public Color color;
    public float size;
    public float timeToCollectable;
    public Jibbit jibbit;

    Jibbit myJibbit;
    float lifetime;
    float maxLifetime;

    public void Launch(Vector3 force, Color color, float size, Jibbit jibbit, float timeToCollectable)
    {
        rigidbody.AddForce(force);
        effect.SetVector4("Color", color);
        line.startColor = color;
        line.endColor = color * new Color(1, 1, 1, 0.5f);
        myJibbit = jibbit;
        maxLifetime = timeToCollectable;
        transform.localScale = new Vector3(size, size, size);
    }

    public void Launch(Vector3 force)
    {
        Launch(force, color, size, jibbit, timeToCollectable);
    }

    private void Update()
    {
        lifetime += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (lifetime >= maxLifetime && maxLifetime != 0 && other.gameObject.CompareTag("Player"))
        {
            JibbitManager.AcquireJibbit(myJibbit.id);
            collectSound.Post(gameObject);
            JibbitAcquiredPopup.current.Acquire(myJibbit);
            Destroy(gameObject);
        }
    }
}
