using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class AIHeli : MonoBehaviour
{
    private Transform intersections;
    private Transform currIntersection;
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

    void Start()
    {
        intersections = GameObject.FindGameObjectWithTag("Intersections")?.transform;
        player = FindObjectOfType<Player>();
        StartCoroutine(FireGuns());
        StartCoroutine(RecalculateBestIntersection());
    }


    void Update()
    {
        if (player)
        {
            model.LookAt(player.AISpotPoint);
        }
    }

    IEnumerator FireGuns()
    {
        while (true && guns.Length > 0)
        {
            while (!InRange())
                yield return null;
            int currGunIndex = 0;
            while (InRange())
            {
                //fire gun
                HeliBullet bullet = Instantiate(bulletPrefab, guns[currGunIndex].transform.position, guns[currGunIndex].transform.rotation);
                bullet.Fire();
                currGunIndex = (currGunIndex + 1) % guns.Length;
                yield return new WaitForSeconds(fireDelay);
            }
        }
    }

    //checks if its current intersection is non-ideal, if so, find a new intersection
    IEnumerator RecalculateBestIntersection()
    {
        bool firstTime = true;
        currIntersection = intersections.GetChild(0);
        while (true)
        {
            float intersectionDistToPlayer = (currIntersection.position - player.transform.position).sqrMagnitude;
            float minCloseness = Mathf.Abs(intersectionDistToPlayer - idealDist);
            Vector3 vectToPlayer = player.AISpotPoint.position - (currIntersection.position + model.localPosition);
            hasLineOfSight = !Physics.Raycast(model.transform.position, vectToPlayer, vectToPlayer.magnitude, LayerMask.GetMask("Obstructions"));

            //check if invalid: either we haven't calculated any destination yet, or we are too close, or too far, or there is something obstructing us
            if (firstTime || intersectionDistToPlayer < playerMinDistSqrd || intersectionDistToPlayer > playerMaxDistSqrd || !hasLineOfSight)
            {
                //find new candidate
                foreach (Transform intersection in intersections)
                {
                    //we want the intersection that's closest to the average distance between min and max
                    float dist = (intersection.position - player.transform.position).sqrMagnitude;
                    float closeness = Mathf.Abs(dist - idealDist);
                    if (dist >= playerMinDistSqrd && dist <= playerMaxDistSqrd && closeness < minCloseness)
                    {
                        vectToPlayer = player.AISpotPoint.position - (intersection.position + model.localPosition);
                        Vector3 vectToDest = intersection.position - transform.position;

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
                pathfinder.SetDestination(currIntersection.position);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    bool InRange()
    {
        return hasLineOfSight && (currIntersection.position - player.transform.position).sqrMagnitude < playerMaxDistSqrd;
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
            Gizmos.DrawCube(currIntersection.position, Vector3.one * 3);
        }
    }
}
