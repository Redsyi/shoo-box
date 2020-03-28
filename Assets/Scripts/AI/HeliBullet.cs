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
        if(impacts %30 == 0){
            if (collision.gameObject.CompareTag("Player"))
            {
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

    /* old linerenderer-based bullet code
    public float trailLifetime;
    public LineRenderer renderer;

    public void Fire()
    {
        StartCoroutine(DoFire());
    }

    IEnumerator DoFire()
    {
        RaycastHit raycastHit;
        renderer.SetPosition(0, transform.position);
        if (Physics.Raycast(transform.position, transform.forward, out raycastHit, 40, LayerMask.GetMask("Player", "Obstructions")))
        {
            renderer.SetPosition(1, raycastHit.point);
        } else
        {
            renderer.SetPosition(1, transform.position + transform.forward * 40);
        }
        renderer.enabled = true;
        yield return new WaitForSeconds(trailLifetime);
        Destroy(gameObject);
    }
    */
}
