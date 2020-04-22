using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : MonoBehaviour, IKickable
{
    public Rigidbody[] possibleSpawns;
    public float spawnForce;
    public GameObject spawner;

    public void OnKick(GameObject kicker)
    {
        if (possibleSpawns != null && possibleSpawns.Length > 0)
        {
            int idx = Random.Range(0, possibleSpawns.Length);
            Rigidbody item = Instantiate(possibleSpawns[idx], spawner.transform.position, spawner.transform.rotation);
            item.AddForce(spawner.transform.forward * spawnForce);
        }
    }
}
