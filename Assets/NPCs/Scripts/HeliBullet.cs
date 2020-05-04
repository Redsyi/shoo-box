using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that represents a bullet fired by a helicopter minigun
/// </summary>
public class HeliBullet : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody rigidbody;
    public Collider collider;
    public ParticleSystem particleSystem;
    public TrailRenderer trailRenderer;
    [Header("Stats")]
    public float velocity;
    public float lifetime;
    public float damage;
    [Header("Sound")]
    public AK.Wwise.Event onFire;
    public AK.Wwise.Event onPlayer;

    //static pool of available bullets for optimization
    public static Queue<HeliBullet> reusableBullets;
    private float currLifetime;
    private bool active;
    static int impacts;

    private void Awake()
    {
        if (reusableBullets == null)
        {
            reusableBullets = new Queue<HeliBullet>();
        }
        currLifetime = lifetime;
        active = true;
    }

    private void Update()
    {
        if (active)
        {
            currLifetime -= Time.deltaTime;
            if (currLifetime <= 0)
            {
                Deactivate();
            }
        }
    }

    /// <summary>
    /// fires this bullet forwards
    /// </summary>
    public void Fire()
    {
        rigidbody.AddForce(transform.forward * velocity);
        onFire.Post(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.isKinematic = true;
        collider.enabled = false;
        particleSystem.Play();

        impacts++;

        //handles special case of hitting player
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponentInParent<CityPlayerHelper>().TakeDamage(damage, DamageSource.HELICOPTER);
            Player.ControllerRumble(RumbleStrength.WEAK, 0.1f);
            if (impacts %30 == 0){
                onPlayer.Post(gameObject);
            }
        }
    }

    /// <summary>
    /// deactivates this bullet for use later
    /// </summary>
    public void Deactivate()
    {
        active = false;
        rigidbody.velocity = Vector3.zero;
        currLifetime = lifetime;
        rigidbody.isKinematic = true;
        particleSystem.Stop();
        trailRenderer.enabled = false;
        Invoke("Queue", 0.05f);
        //gameObject.SetActive(false);
    }

    /// <summary>
    /// adds this bullet to the reuse pool
    /// </summary>
    private void Queue()
    {
        reusableBullets.Enqueue(this);
    }

    /// <summary>
    /// reactivates this bullet
    /// </summary>
    public void Reactivate()
    {
        active = true;
        rigidbody.isKinematic = false;
        collider.enabled = true;
        trailRenderer.enabled = true;
    }

    /// <summary>
    /// spawns a bullet, reactivating from the pool instead of instantiating when possible
    /// </summary>
    public static HeliBullet SpawnBullet(HeliBullet prefab, Vector3 location, Quaternion rotation)
    {
        if (reusableBullets == null || reusableBullets.Count == 0)
        {
            return Instantiate(prefab, location, rotation);
        }
        else
        {
            HeliBullet bullet = reusableBullets.Dequeue();
            bullet.transform.position = location;
            bullet.transform.rotation = rotation;
            bullet.Reactivate();
            return bullet;
        }
    }

    /// <summary>
    /// clears the reuse pool
    /// </summary>
    private void OnDestroy()
    {
        if (reusableBullets != null)
        {
            reusableBullets = null;
        }
    }
}
