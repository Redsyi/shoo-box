using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JibbitSuitcaseGiver : MonoBehaviour, IKickable
{
    public CollectableJibbit jibbit;
    public Transform launcher;
    public Animator animator;
    public GameObject[] luggage;
    public float force;

    bool given;

    public void OnKick(GameObject kicker)
    {
        if (!given)
        {
            given = true;
            foreach (GameObject bag in luggage)
            {
                bag.SetActive(true);
            }
            CollectableJibbit jib = Instantiate(jibbit, launcher.position, Quaternion.identity);
            jib.Launch(launcher.forward * force);
            animator.SetTrigger("Open");
        }
    }
}
