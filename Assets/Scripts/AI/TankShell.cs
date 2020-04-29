using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShell : MonoBehaviour
{
    public float trailLifetime;
    public LineRenderer renderer;
    public static Queue<TankShell> reusableShells;
    private bool active;
    public Gradient initialGradient;
    public Gradient fadeGradient;
    [Range(0, 1)]
    public float fadeBlend;
    private float currFadeBlend;
    public Animator animator;
    public GameObject impactParticles;
    public float hitForce;
    public float damage;
    public AK.Wwise.Event onFire;
    public bool firedByPlayer;

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

    IEnumerator DoFire()
    {
        RaycastHit raycastHit;
        animator.SetTrigger("Fire");
        renderer.SetPosition(0, transform.position);
        onFire.Post(gameObject);
        LayerMask layers = (firedByPlayer ? LayerMask.GetMask("Obstructions", "Default") : LayerMask.GetMask("Player", "Obstructions", "Default"));
        if (Physics.Raycast(transform.position, transform.forward, out raycastHit, 40, layers))
        {
            renderer.SetPosition(1, raycastHit.point);
            impactParticles.transform.position = raycastHit.point;
            foreach (ParticleSystem system in impactParticles.GetComponentsInChildren<ParticleSystem>())
            {
                system.Stop();
                system.Play();
            }
            if (raycastHit.collider.CompareTag("Player"))
            {
                raycastHit.collider.GetComponentInParent<Player>().HitByEnemy(damage);
                raycastHit.rigidbody.AddForce(transform.forward * hitForce);
                CameraScript.current.ShakeScreen(ShakeStrength.INTENSE, ShakeLength.SHORT);
                Player.ControllerRumble(RumbleStrength.INTENSE, 0.3f);
            }

            foreach (IKickable kickable in raycastHit.collider.GetComponents<IKickable>())
            {
                kickable.OnKick(gameObject);
            }

            AITank tank = raycastHit.collider.GetComponentInParent<AITank>();
            if (tank)
            {
                tank.Destroy();
            }

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
        yield return new WaitForSeconds(trailLifetime);
        Deactivate();
    }

    public void Reactivate()
    {
        active = true;
        renderer.enabled = true;
    }

    private void Queue()
    {
        reusableShells.Enqueue(this);
    }

    public void Deactivate()
    {
        active = false;
        renderer.enabled = false;
        Invoke("Queue", 0.05f);
    }

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

    private void OnDestroy()
    {
        if (reusableShells != null)
        {
            reusableShells = null;
        }
    }
}
