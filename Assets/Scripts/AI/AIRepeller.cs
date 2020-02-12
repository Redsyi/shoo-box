using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIRepeller : MonoBehaviour
{
    public float endRadius;
    public float growTime;
    public NavMeshObstacle obstacle;
    public Collider collider;
    public float repelActiveTime;

    private bool repelling;
    
    public void Repel()
    {
        if (!repelling)
            StartCoroutine(DoRepel(repelActiveTime));
    }

    IEnumerator DoRepel(float seconds)
    {
        repelling = true;
        collider.enabled = true;
        obstacle.enabled = true;
        float timeLeft = growTime;
        while (timeLeft > 0)
        {
            obstacle.radius += Time.deltaTime * endRadius * (1 / growTime);
            yield return null;
            timeLeft -= Time.deltaTime;
        }
        yield return new WaitForSeconds(repelActiveTime - growTime);
        obstacle.radius = 0;
        collider.enabled = false;
        obstacle.enabled = false;
        repelling = false;
    }

    private void OnTriggerStay(Collider other)
    {
        AIAgent agent = other.GetComponentInParent<AIAgent>();
        if (agent)
        {
            agent.beingRepelled = true;
            agent.transform.LookAt(transform.position);
            agent.transform.localEulerAngles = new Vector3(agent.transform.localEulerAngles.x, agent.transform.localEulerAngles.y + 180, agent.transform.localEulerAngles.z);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        AIAgent agent = other.GetComponentInParent<AIAgent>();
        if (agent)
        {
            agent.beingRepelled = false;
        }
    }
}
