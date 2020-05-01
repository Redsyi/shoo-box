using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that represents a round fired by a tank
/// </summary>
public class TankShell : MonoBehaviour
{
    [Header("Components")]
    public LineRenderer renderer;
    public Animator animator;
    public GameObject impactParticles;
    [Header("Stats")]
    public float trailLifetime;
    public float hitForce;
    public float damage;
    [Header("Effects")]
    public Gradient initialGradient;
    public Gradient fadeGradient;
    public AK.Wwise.Event onFire;
    [Range(0, 1)]
    public float fadeBlend;

    //queue of reusable shells for optimization
    public static Queue<TankShell> reusableShells;
    private bool active;
    private float currFadeBlend;
    [HideInInspector]
    public bool firedByPlayer;

    /// <summary>
    /// fires this round
    /// </summary>
    public void Fire()
    {
        StartCoroutine(DoFire());
    }

    private void Start()
    {
        if (reusableShells == null)
            reusableShells = new Queue<TankShell>();
    }

    private void Update()
    {
        if (currFadeBlend != fadeBlend)
        {
            currFadeBlend = fadeBlend;
            renderer.colorGradient = LerpGradient(initialGradient, fadeGradient, fadeBlend);
        }
    }

    /// <summary>
    /// helper method to lerp between two gradients, because unity doesn't support that by default
    /// </summary>
    private Gradient LerpGradient(Gradient gradient1, Gradient gradient2, float blend)
    {
        Gradient result = new Gradient();
        var colorKeys = new GradientColorKey[gradient1.colorKeys.Length];
        var alphaKeys = new GradientAlphaKey[gradient1.alphaKeys.Length];
        for (int i = 0; i < result.colorKeys.Length; ++i)
        {
            colorKeys[i].color = Color.Lerp(gradient1.colorKeys[i].color, gradient2.colorKeys[i].color, blend);
            colorKeys[i].time = Mathf.Lerp(gradient1.colorKeys[i].time, gradient2.colorKeys[i].time, blend);
        }
        for (int i = 0; i < result.alphaKeys.Length; ++i)
        {
            alphaKeys[i].alpha = Mathf.Lerp(gradient1.alphaKeys[i].alpha, gradient2.alphaKeys[i].alpha, blend);
            alphaKeys[i].time = Mathf.Lerp(gradient1.alphaKeys[i].time, gradient2.alphaKeys[i].time, blend);
        }
        result.SetKeys(colorKeys, alphaKeys);
        return result;
    }

    //coroutine to manage the firing process
    IEnumerator DoFire()
    {
        RaycastHit raycastHit;
        animator.SetTrigger("Fire");
        renderer.SetPosition(0, transform.position);
        onFire.Post(gameObject);
        LayerMask layers = (firedByPlayer ? LayerMask.GetMask("Obstructions", "Default") : LayerMask.GetMask("Player", "Obstructions", "Default"));

        //performs the hit detection raycast
        if (Physics.Raycast(transform.position, transform.forward, out raycastHit, 40, layers))
        {
            renderer.SetPosition(1, raycastHit.point);
            impactParticles.transform.position = raycastHit.point;
            foreach (ParticleSystem system in impactParticles.GetComponentsInChildren<ParticleSystem>())
            {
                system.Stop();
                system.Play();
            }

            //manages hitting the player
            if (raycastHit.collider.CompareTag("Player"))
            {
                raycastHit.collider.GetComponentInParent<Player>().HitByEnemy(damage);
                raycastHit.rigidbody.AddForce(transform.forward * hitForce);
                CameraScript.current.ShakeScreen(ShakeStrength.INTENSE, ShakeLength.SHORT);
                Player.ControllerRumble(RumbleStrength.INTENSE, 0.3f);
            }

            //emulates kicking objects
            foreach (IKickable kickable in raycastHit.collider.GetComponents<IKickable>())
            {
                kickable.OnKick(gameObject);
            }

            //destroys tanks
            AITank tank = raycastHit.collider.GetComponentInParent<AITank>();
            if (tank)
            {
                tank.Destroy();
            }

            //performs screen shake if fired by player
            if (firedByPlayer)
            {
                CameraScript.current.ShakeScreen(ShakeStrength.INTENSE, ShakeLength.SHORT);
                Player.ControllerRumble(RumbleStrength.INTENSE, 0.3f);
            }
        }
        else
        {
            renderer.SetPosition(1, transform.position + transform.forward * 40);
        }
        renderer.enabled = true;

        //lingers for a bit, then deactivates for reuse
        yield return new WaitForSeconds(trailLifetime);
        Deactivate();
    }

    /// <summary>
    /// reactivate this round for use
    /// </summary>
    public void Reactivate()
    {
        active = true;
        renderer.enabled = true;
    }

    /// <summary>
    /// add this round to the available round pool
    /// </summary>
    private void Queue()
    {
        reusableShells.Enqueue(this);
    }

    /// <summary>
    /// deactivates this round and adds it to the pool
    /// </summary>
    public void Deactivate()
    {
        active = false;
        renderer.enabled = false;
        Invoke("Queue", 0.05f);
    }

    /// <summary>
    /// spawns a new shell, pulling from the pool instead of instantiating if possible
    /// </summary>
    public static TankShell SpawnShell(TankShell prefab, Vector3 location, Quaternion rotation)
    {
        if (reusableShells == null || reusableShells.Count == 0)
        {
            return Instantiate(prefab, location, rotation);
        } else
        {
            TankShell shell = reusableShells.Dequeue();
            shell.transform.position = location;
            shell.transform.rotation = rotation;
            shell.Reactivate();
            return shell;
        }
    }

    //clears the pool so we don't pull nullrefs in the future
    private void OnDestroy()
    {
        if (reusableShells != null)
        {
            reusableShells = null;
        }
    }
}
