using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliBullet : MonoBehaviour
{
    public Rigidbody rigidbody;
    public Collider collider;
    public ParticleSystem particleSystem;
    public TrailRenderer trailRenderer;
    public float velocity;
    public static Queue<HeliBullet> reusableBullets;
    public float lifetime;
    private float currLifetime;
    private bool active;
    public AK.Wwise.Event onFire;
    public AK.Wwise.Event onPlayer;
    public float damage;
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
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponentInParent<Player>().HitByEnemy(damage);
            Player.ControllerRumble(RumbleStrength.WEAK, 0.1f);
            if (impacts %30 == 0){
                onPlayer.Post(gameObject);
            }
        }
    }

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

    private void Queue()
    {
        reusableBullets.Enqueue(this);
    }

    public void Reactivate()
    {
        active = true;
        rigidbody.isKinematic = false;
        collider.enabled = true;
        trailRenderer.enabled = true;
    }

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

    private void OnDestroy()
    {
        if (reusableBullets != null)
        {
            reusableBullets = null;
        }
    }
}
