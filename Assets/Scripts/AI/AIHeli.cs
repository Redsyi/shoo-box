using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class AIHeli : MonoBehaviour, IKickable
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

    void Start()
    {
        player = FindObjectOfType<Player>();
        StartCoroutine(FireGuns());
        StartCoroutine(RecalculateBestIntersection());
        initialSwirlRate = heliSwirl.emission.rateOverTime.constant;
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
                HeliBullet bullet = GetBullet(guns[currGunIndex]);
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

    HeliBullet GetBullet(GameObject gun)
    {
        if (HeliBullet.reusableBullets == null || HeliBullet.reusableBullets.Count == 0)
            return Instantiate(bulletPrefab, gun.transform.position, gun.transform.rotation);
        else
        {
            HeliBullet bullet = HeliBullet.reusableBullets.Dequeue();
            bullet.gameObject.SetActive(true);
            bullet.transform.position = gun.transform.position;
            bullet.transform.rotation = gun.transform.rotation;
            bullet.Reactivate();
            return bullet;
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

                            //make sure selected candidate has line-of-sight to player, and we don't cross over the player when trying to get there
                            if (!Physics.Raycast(model.transform.position, vectToPlayer, vectToPlayer.magnitude, LayerMask.GetMask("Obstructions")) &&
                                !Physics.Raycast(transform.position + Vector3.up, vectToDest, vectToDest.magnitude, LayerMask.GetMask("Player")))
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
            Gizmos.DrawCube(currIntersection.transform.position, Vector3.one * 3);
        }
    }

    public void OnKick(GameObject kicker)
    {
        if (!destroyed)
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        destroyed = true;
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
    }
}
