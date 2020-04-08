using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class AIHeli : MonoBehaviour, ISandalable
{
    private CityIntersection currIntersection;
    private Player player;
    public Transform model;
    public float roundsPerSecond;
    public GameObject[] guns;
    public NavMeshAgent pathfinder;
    public float playerMinDist;
    private float playerMinDistSqrd => playerMinDist * playerMinDist;
    public float playerMaxDist;
    private float playerMaxDistSqrd => playerMaxDist * playerMaxDist;
    private float idealDist => Mathf.Pow((playerMinDist + playerMaxDist) / 2, 2);
    private float fireDelay => 1 / roundsPerSecond;
    private bool hasLineOfSight;
    public HeliBullet bulletPrefab;
    private bool destroyed;
    public ParticleSystem heliSwirl;
    private float initialSwirlRate;
    public float swirlDestroyedCDTime;
    private float currSwirlCDTime;
    public AK.Wwise.Event onHit; 
    public AK.Wwise.Event onFly;
    public AK.Wwise.Event onHitAfter;
    public ForceOnKick[] onDestroyKickables;
    public GameObject sandalTarget;

    void Start()
    {
        player = FindObjectOfType<Player>();
        StartCoroutine(FireGuns());
        StartCoroutine(RecalculateBestIntersection());
        initialSwirlRate = heliSwirl.emission.rateOverTime.constant;
        onFly.Post(gameObject);
        CityDirector.numHelis++;
    }


    void Update()
    {
        if (!destroyed && player)
        {
            model.LookAt(player.AISpotPoint);
            if (model.localEulerAngles.x > 70)
            {
                model.localEulerAngles = new Vector3(70, model.localEulerAngles.y, model.localEulerAngles.z);
            }
        } else if (destroyed)
        {
            if (currSwirlCDTime < swirlDestroyedCDTime)
            {
                currSwirlCDTime = Mathf.Min(currSwirlCDTime + Time.deltaTime, swirlDestroyedCDTime);
                ParticleSystem.EmissionModule emission = heliSwirl.emission;
                ParticleSystem.MinMaxCurve rate = emission.rateOverTime;
                rate.constant = Mathf.Lerp(initialSwirlRate, 0, currSwirlCDTime / swirlDestroyedCDTime);
                emission.rateOverTime = rate;
            }
        }
    }

    IEnumerator FireGuns()
    {
        while (true && guns.Length > 0)
        {
            while (!InRange())
                yield return null;
            int currGunIndex = 0;
            foreach (GameObject gun in guns)
            {
                gun.GetComponentInParent<Animator>()?.SetBool("Firing", true);
            }
            while (InRange())
            {
                //fire gun
                HeliBullet bullet = HeliBullet.SpawnBullet(bulletPrefab, guns[currGunIndex].transform.position, guns[currGunIndex].transform.rotation);
                bullet.Fire();
                currGunIndex = (currGunIndex + 1) % guns.Length;
                yield return new WaitForSeconds(fireDelay);
            }
            foreach (GameObject gun in guns)
            {
                gun.GetComponentInParent<Animator>()?.SetBool("Firing", false);
            }
        }
    }
    

    //checks if its current intersection is non-ideal, if so, find a new intersection
    IEnumerator RecalculateBestIntersection()
    {
        yield return null;
        bool firstTime = true;
        currIntersection = CityIntersection.intersections[0];
        while (true)
        {
            float intersectionDistToPlayer = (currIntersection.transform.position - player.transform.position).sqrMagnitude;
            float minCloseness = Mathf.Abs(intersectionDistToPlayer - idealDist);
            Vector3 vectToPlayer = player.AISpotPoint.position - (currIntersection.transform.position + model.localPosition);
            hasLineOfSight = !Physics.Raycast(model.transform.position, vectToPlayer, vectToPlayer.magnitude, LayerMask.GetMask("Obstructions"));
            if (!hasLineOfSight)
                minCloseness = 1000000;

            //check if invalid: either we haven't calculated any destination yet, or we are too close, or too far, or there is something obstructing us
            if (firstTime || intersectionDistToPlayer < playerMinDistSqrd || intersectionDistToPlayer > playerMaxDistSqrd || !hasLineOfSight)
            {
                currIntersection.assignedHeli = false;
                //find new candidate
                foreach (CityIntersection intersection in CityIntersection.intersections)
                {
                    if (!intersection.assignedHeli)
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
                currIntersection.assignedHeli = true;
                pathfinder.SetDestination(currIntersection.transform.position);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    bool InRange()
    {
        return hasLineOfSight && (currIntersection.transform.position - player.transform.position).sqrMagnitude < playerMaxDistSqrd;
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
            Gizmos.DrawCube(currIntersection.transform.position + model.localPosition, Vector3.one * 2);
        }
    }

    public void HitBySandal()
    {
        if (!destroyed)
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        destroyed = true;
        onHit. Post(gameObject);
        onHitAfter.Post(gameObject);
        foreach (Animator animator in GetComponentsInChildren<Animator>())
        {
            animator.SetTrigger("Destroyed");
        }
        StopAllCoroutines();
        pathfinder.enabled = false;
        foreach (Collider collider in model.GetComponentsInChildren<Collider>())
        {
            collider.enabled = true;
        }
        foreach (Rigidbody rigidbody in model.GetComponentsInChildren<Rigidbody>())
        {
            rigidbody.isKinematic = false;
        }

        foreach (ForceOnKick kickable in onDestroyKickables)
        {
            kickable.kickEnabled = true;
        }
        player.shoeManager.sandalSlinger.ClearTarget(sandalTarget.transform);
        sandalTarget.SetActive(false);
        CityDirector.numHelis--;
        CityDirector.current.IncreaseIntensity(0.4f);
    }
}
