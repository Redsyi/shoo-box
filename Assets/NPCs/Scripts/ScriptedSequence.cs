using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

/// <summary>
/// class that overrides an NPC's behavior with a scripted sequence of events. Can by started immediately and/or manually triggered at any point.
/// Multiple sequences can be attached to a single NPC, though running 2 at once is undefined
/// </summary>
public class ScriptedSequence : MonoBehaviour
{
    [System.Serializable]
    public class ScriptedStep
    {
        public StepType type;
        public float seconds;
        public UnityEvent invokeOnStart;
        public Vector3 position;
        public bool continueOnReached;
    }

    public ScriptedStep[] sequence;
    public bool startImmediately = true;

    [HideInInspector]
    public bool running;
    AIAgent AI;
    int currStepIdx;
    ScriptedStep currStep => sequence[currStepIdx];

    private void Start()
    {
        AI = GetComponent<AIAgent>();
        if (startImmediately)
            StartCoroutine(DoSequence());
    }

    /// <summary>
    /// manually start this sequence
    /// </summary>
    public void Trigger()
    {
        if (!running)
        {
            currStepIdx = 0;
            StartCoroutine(DoSequence());
        }
    }

    /// <summary>
    /// manually interrupt this sequence
    /// </summary>
    public void Interrupt()
    {
        running = false;
        AI.animator.SetBool("Interacting", false);
        AI.animator.SetBool("Investigating", false);
        AI.animator.SetBool("Running", false);
        AI.animator.SetBool("Moving", false);
        AI.pathfinder.enabled = true;
    }

    /// <summary>
    /// coroutine to run the sequence
    /// </summary>
    IEnumerator DoSequence()
    {
        running = true;
        //one while loop iteration = one step
        while (currStepIdx < sequence.Length && running)
        {
            currStep.invokeOnStart.Invoke();
            //clear animator variables
            AI.animator.SetBool("Interacting", false);
            AI.animator.SetBool("Investigating", false);
            AI.animator.SetBool("Running", false);
            AI.animator.SetBool("Moving", false);
            AI.pathfinder.enabled = true;

            //set animator variables and pathfinder destination based on current step
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

            //keeps the npc facing where it needs to face
            float timeLeft = currStep.seconds;
            while (timeLeft > 0 && running && (!currStep.continueOnReached || (transform.position - currStep.position).sqrMagnitude > 0.6f))
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
        running = false;

        //clears variables
        AI.animator.SetBool("Interacting", false);
        AI.animator.SetBool("Investigating", false);
        AI.animator.SetBool("Running", false);
        AI.animator.SetBool("Moving", false);
        AI.pathfinder.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (sequence != null)
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
}
