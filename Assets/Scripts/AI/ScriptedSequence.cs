using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class ScriptedSequence : MonoBehaviour
{
    [System.Serializable]
    public class ScriptedStep
    {
        public StepType type;
        public float seconds;
        public UnityEvent invokeOnStart;
        public Vector3 position;
    }

    public ScriptedStep[] sequence;
    AIAgent AI;
    int currStepIdx;
    ScriptedStep currStep => sequence[currStepIdx];
    private void Start()
    {
        AI = GetComponent<AIAgent>();
        StartCoroutine(DoSequence());
    }

    IEnumerator DoSequence()
    {
        while (currStepIdx < sequence.Length)
        {
            currStep.invokeOnStart.Invoke();
            AI.animator.SetBool("Interacting", false);
            AI.animator.SetBool("Investigating", false);
            AI.animator.SetBool("Running", false);
            AI.animator.SetBool("Moving", false);
            AI.pathfinder.enabled = true;
            switch (currStep.type)
            {
                case StepType.MOVE:
                    AI.animator.SetBool("Moving", true);
                    AI.pathfinder.SetDestination(currStep.position);
                    AI.pathfinder.speed = AI.walkSpeed;
                    break;
                case StepType.RUSH:
                    AI.animator.SetBool("Moving", true);
                    AI.animator.SetBool("Running", true);
                    AI.pathfinder.SetDestination(currStep.position);
                    AI.pathfinder.speed = AI.runSpeed;
                    break;
                case StepType.INTERACT:
                    AI.animator.SetBool("Interacting", true);
                    AI.Face(currStep.position);
                    AI.pathfinder.enabled = false;
                    break;
                case StepType.INVESTIGATE:
                    AI.animator.SetBool("Investigating", true);
                    AI.Face(currStep.position);
                    AI.pathfinder.enabled = false;
                    break;
                case StepType.STAY:
                    AI.pathfinder.SetDestination(transform.position);
                    AI.pathfinder.enabled = false;
                    break;
                case StepType.DIE:
                    Destroy(gameObject);
                    break;
            }

            float timeLeft = currStep.seconds;
            while (timeLeft > 0)
            {
                yield return null;
                timeLeft -= Time.deltaTime;
                switch (currStep.type)
                {
                    case StepType.INTERACT:
                        AI.Face(currStep.position);
                        break;
                    case StepType.INVESTIGATE:
                        AI.Face(currStep.position);
                        break;
                    default:
                        break;
                }
            }
            currStepIdx++;
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach (ScriptedStep step in sequence)
        {
            switch (step.type)
            {
                case StepType.MOVE:
                    Gizmos.color = Color.blue;
                    break;
                case StepType.RUSH:
                    Gizmos.color = Color.cyan;
                    break;
                case StepType.INTERACT:
                    Gizmos.color = Color.green;
                    break;
                case StepType.INVESTIGATE:
                    Gizmos.color = Color.yellow;
                    break;
                case StepType.STAY:
                    Gizmos.color = Color.white;
                    break;
                case StepType.DIE:
                    Gizmos.color = Color.red;
                    break;
            }

            Gizmos.color = Gizmos.color * new Color(1, 1, 1, 0.5f);
            Gizmos.DrawSphere(step.position, 0.5f);
        }
    }
}
