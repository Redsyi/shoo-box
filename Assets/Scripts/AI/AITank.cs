using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AITank : MonoBehaviour, IKickable
{
    private CityRoad currIntersection;
    private Player player;
    public Transform model;
    public GameObject gunBarrel;
    public Animator gunAnimator;
    public Transform turret;
    public NavMeshAgent pathfinder;
    public float playerMinDist;
    private float playerMinDistSqrd => playerMinDist * playerMinDist;
    public float playerMaxDist;
    private float playerMaxDistSqrd => playerMaxDist * playerMaxDist;
    private float idealDist => Mathf.Pow((playerMinDist + playerMaxDist) / 2, 2);
    public float fireDelay;
    private float timeSinceLastFire;
    private bool hasLineOfSight;
    public TankShell shellPrefab;
    private bool destroyed;
    public GameObject movementParticleRoot;
    private ParticleSystem[] movementParticles;
    private bool moving;
    private Vector3 prevPosition;
    public ParticleSystem muzzleFlashParticles;
    [Header("Destuction Components")]
    public Animator[] onDestroyAnimators;
    public Collider mainCollider;
    public Rigidbody mainRigidbody;
    public Rigidbody[] onDestroyRigidbodies;
    public Rigidbody gunRigidbody;
    public Rigidbody bubbleRigidbody;
    public float bubbleLaunchVelocity;
    public float gunLaunchVelocity;
    public ForceOnKick[] onDestroyKickables;
    public AK.Wwise.Event onStart;
    public AK.Wwise.Event onHit;

    void Start()
    {
        player = FindObjectOfType<Player>();
        StartCoroutine(RecalculateBestIntersection());
        onStart.Post (gameObject);
        movementParticles = movementParticleRoot.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem moveParticles in movementParticles)
        {
            moveParticles.Pause();
        }
    }

    void Update()
    {
        if (!destroyed && player)
        {
            timeSinceLastFire += Time.deltaTime;
            turret.LookAt(player.AISpotPoint);
            if (player.legForm)
                turret.localEulerAngles = new Vector3(0, turret.localEulerAngles.y);
            if (timeSinceLastFire >= fireDelay && hasLineOfSight)
            {
                Fire();
            }
        }
    }

    private void Fire()
    {
        TankShell shell = TankShell.SpawnShell(shellPrefab, gunBarrel.transform.position, gunBarrel.transform.rotation);
        shell.Fire();
        timeSinceLastFire = 0;
        muzzleFlashParticles.Stop();
        muzzleFlashParticles.Play();
        gunAnimator.SetTrigger("Fire");
    }

    private void FixedUpdate()
    {
        if (!destroyed)
        {
            bool movingNow = (transform.position - prevPosition).sqrMagnitude > 0.001f;
            if (moving && !movingNow)
            {
                foreach (ParticleSystem moveParticles in movementParticles)
                {
                    moveParticles.Stop();
                }
            }
            else if (!moving && movingNow)
            {
                foreach (ParticleSystem moveParticles in movementParticles)
                {
                    moveParticles.Play();
                }
            }
            moving = movingNow;
            prevPosition = transform.position;
        }
    }

    IEnumerator RecalculateBestIntersection()
    {
        yield return null;
        bool firstTime = true;
        currIntersection = CityRoad.intersections[0];
        while (true)
        {
            float intersectionDistToPlayer = (currIntersection.transform.position - player.transform.position).sqrMagnitude;
            float minCloseness = Mathf.Abs(intersectionDistToPlayer - idealDist);
            Vector3 vectToPlayer = player.AISpotPoint.position - (currIntersection.transform.position + model.localPosition);
            hasLineOfSight = !Physics.Raycast(gunBarrel.transform.position, vectToPlayer, vectToPlayer.magnitude, LayerMask.GetMask("Obstructions"));
            if (!hasLineOfSight)
                minCloseness = 1000000;

            //check if invalid: either we haven't calculated any destination yet, or we are too close, or too far, or there is something obstructing us
            if (firstTime || intersectionDistToPlayer < playerMinDistSqrd || intersectionDistToPlayer > playerMaxDistSqrd || !hasLineOfSight)
            {
                currIntersection.assignedTank = false;
                //find new candidate
                foreach (CityRoad intersection in CityRoad.intersections)
                {
                    if (!intersection.assignedTank)
                    {
                        //we want the intersection that's closest to the average distance between min and max
                        float dist = (intersection.transform.position - player.transform.position).sqrMagnitude;
                        float closeness = Mathf.Abs(dist - idealDist);
                        if (dist >= playerMinDistSqrd && dist <= playerMaxDistSqrd && closeness < minCloseness)
                        {
                            vectToPlayer = player.AISpotPoint.position - (intersection.transform.position + model.localPosition);
                            Vector3 vectToDest = intersection.transform.position - transform.position;

                            //make sure selected candidate has line-of-sight to player
                            if (!Physics.Raycast(intersection.transform.position + model.localPosition, vectToPlayer, vectToPlayer.magnitude, LayerMask.GetMask("Obstructions")))
                            {
                                minCloseness = closeness;
                                currIntersection = intersection;
                                firstTime = false;
                            }
                        }
                    }
                }
                currIntersection.assignedTank = true;
                pathfinder.SetDestination(currIntersection.transform.position);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, playerMinDist);
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawSphere(transform.position, playerMaxDist);
        if (currIntersection)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(currIntersection.transform.position, Vector3.one * 2);
        }
    }

    public void Destroy()
    {
        destroyed = true;
        mainCollider.enabled = false;
        mainRigidbody.isKinematic = true;
        onHit.Post(gameObject);
        foreach (Animator animator in onDestroyAnimators)
        {
            if (!animator.enabled)
                animator.enabled = true;
            animator.SetTrigger("Destroy");
        }
        foreach (Rigidbody rigidbody in onDestroyRigidbodies)
        {
            rigidbody.useGravity = true;
            rigidbody.isKinematic = false;
        }

        bubbleRigidbody.transform.SetParent(transform);
        Vector3 bubbleForce = Vector3.up * 3;
        bubbleForce += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0, 0.5f), Random.Range(-0.5f, 0.5f));
        bubbleForce.Normalize();
        bubbleForce *= bubbleLaunchVelocity;
        bubbleRigidbody.AddForce(bubbleForce);
        bubbleRigidbody.AddTorque(Random.Range(-4, 4), Random.Range(-4, 4), Random.Range(-4, 4));

        Vector3 gunForce = (player.transform.position - turret.transform.position).normalized;
        gunForce *= 2;
        gunForce += new Vector3(Random.Range(-1, 1), Random.Range(1.5f, 2f), Random.Range(-1, 1));
        gunForce.Normalize();
        gunForce *= gunLaunchVelocity;
        gunRigidbody.AddForce(gunForce);
        gunRigidbody.AddTorque(Random.Range(-4, 4), Random.Range(-4, 4), Random.Range(-4, 4));
        
        foreach (ParticleSystem moveParticles in movementParticles)
        {
            moveParticles.Stop();
        }
        pathfinder.enabled = false;
        StopAllCoroutines();
        Invoke("EnableKickables", 0.5f);

        CityDirector.numTanks--;
        CityDirector.current.IncreaseIntensity(0.4f);
    }

    void EnableKickables()
    {
        foreach (ForceOnKick kickable in onDestroyKickables)
        {
            kickable.kickEnabled = true;
        }
    }

    public void OnKick(GameObject kicker)
    {
        if (!destroyed)
        {
            Destroy();
        }
    }
}
