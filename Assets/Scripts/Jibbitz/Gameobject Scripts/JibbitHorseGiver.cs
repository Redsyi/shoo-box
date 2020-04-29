using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JibbitHorseGiver : MonoBehaviour
{
    public CollectableJibbit jibbit;
    public float force;

    bool given;
    float timePassed;
    public static bool qualified;

    private void Start()
    {
        qualified = true;
    }

    private void Update()
    {
        timePassed += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!given && qualified && timePassed > 5)
        {
            given = true;
            CollectableJibbit jib = Instantiate(jibbit, transform.position, Quaternion.identity);
            jib.Launch(transform.forward * force);
        }
    }
}
