using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRotateInPlace : MonoBehaviour
{
    public AIAgent agent;
    public Transform direction1;
    public Transform direction2;
    public float timeBetweenRotations;
    private Transform currDirection;
    private bool facingFirstDirection = true;
    private float timer;


    private void Start()
    {
        currDirection = direction1;
        timer = timeBetweenRotations;
    }

    private void Update()
    {
        if(agent.currState.state == AIState.IDLE && (agent.transform.position - agent.currState.location.position).sqrMagnitude < 0.4f)
        {
            print("Should be rotating");
            agent.transform.rotation = currDirection.rotation;
        }

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            print("Should be changing directions");
            timer = timeBetweenRotations; 
            facingFirstDirection = !facingFirstDirection;
            currDirection = (facingFirstDirection ? direction1 : direction2);
        }
    }
}
