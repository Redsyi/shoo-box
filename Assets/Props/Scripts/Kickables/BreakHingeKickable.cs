using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine;

/// <summary>
/// used for paintings, wall signs, etc
/// </summary>
public class BreakHingeKickable : MonoBehaviour, IKickable, IAIInteractable
{
    // Start is called before the first frame update
    private List<HingeJoint> _hingeJoints;
    [SerializeField] private bool breakAllHinges;
    public int intactJoints => (_hingeJoints != null ? _hingeJoints.Count : 2);
    private bool broken;
    [SerializeField] private int fixTime = 4;
    public AIInterest[] aIInterests;
    public GameObject fixables;
    private Queue<Vector3> hingePurgatory;
    private Vector3 originalPos;
    private Quaternion originalRot;
    
    void Start()
    {
        _hingeJoints = new List<HingeJoint>(GetComponents<HingeJoint>());
        originalPos = transform.position;
        originalRot = transform.rotation;
        hingePurgatory = new Queue<Vector3>();
    }

    public void OnKick(GameObject kicker)
    {
        if (breakAllHinges)
        {
            while (_hingeJoints.Count > 0)
                BreakRandomHinge();
        }
        else if (_hingeJoints.Count > 0)
        {
            BreakRandomHinge();
        }
    }

    void BreakRandomHinge()
    {
        int randIndex = UnityEngine.Random.Range(0, _hingeJoints.Count);
        HingeJoint hingeJoint = _hingeJoints[randIndex];
        _hingeJoints.RemoveAt(randIndex);
        hingePurgatory.Enqueue(hingeJoint.anchor);
        Destroy(hingeJoint);
        broken = true;
    }
        
    public bool NeedsInteraction()
    {
        return broken;
    }
    
    public void AIInteract()
    {
    }

    public float AIInteractTime()
    {
        return fixTime;
    }

    public void AIFinishInteract(AIAgent ai)
    {
        broken = false;
        transform.position = originalPos;
        transform.rotation = originalRot;
        while (hingePurgatory.Count > 0)
        {
            Vector3 jointAnchor = hingePurgatory.Dequeue();
            HingeJoint newJoint = gameObject.AddComponent<HingeJoint>();
            newJoint.anchor = jointAnchor;
            newJoint.autoConfigureConnectedAnchor = true;
            newJoint.useSpring = false;
        }
        _hingeJoints = new List<HingeJoint>(GetComponents<HingeJoint>());
    }

    public void AIInteracting(float interactProgress)
    {

    }

    public AIInterest[] InterestingToWhatAI()
    {
        return aIInterests;
    }
}
