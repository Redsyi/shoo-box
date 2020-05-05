using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// trashcan: flings its contents upward when kicked
/// </summary>
public class Trashcan : MonoBehaviour, IKickable, IAIInteractable
{
    private struct TrashcanContent
    {
        public Rigidbody rigidbody;
        public Transform transform;
        public Vector3 originalPosition;
        public Quaternion originalRotation;
    }

    private List<TrashcanContent> contents;
    private bool broken;
    public AIInterest[] interestMask;
    public float launchVelocity;
    public Transform contentRoot;

    public void AIFinishInteract(AIAgent ai)
    {
        foreach (TrashcanContent content in contents)
        {
            content.rigidbody.velocity = Vector3.zero;
            content.transform.localPosition = content.originalPosition;
            content.transform.localRotation = content.originalRotation;
        }
        broken = false;
    }

    public void AIInteracting(float interactProgress)
    {

    }

    public float AIInteractTime()
    {
        return 4.5f;
    }

    public AIInterest[] InterestingToWhatAI()
    {
        return interestMask;
    }

    public bool NeedsInteraction()
    {
        return broken;
    }

    void Start()
    {
        contents = new List<TrashcanContent>();
        foreach (Transform child in contentRoot)
        {
            TrashcanContent content = new TrashcanContent();
            content.transform = child;
            content.rigidbody = child.GetComponent<Rigidbody>();
            content.originalPosition = child.localPosition;
            content.originalRotation = child.localRotation;
            contents.Add(content);
        }
    }

    public void OnKick(GameObject kicker)
    {
        if (!broken)
        {
            broken = true;
            foreach (TrashcanContent content in contents)
            {
                content.rigidbody.AddForce((Vector3.up + new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.1f ,0.3f), Random.Range(-0.3f, 0.3f))) * launchVelocity);
            }
        }
    }
}
