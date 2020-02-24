using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakHingeKickable : MonoBehaviour, IKickable, IAIInteractable
{
    // Start is called before the first frame update
    private List<HingeJoint> _hingeJoints;
    [SerializeField] private bool breakAllHinges;
    private GameObject preBroken = null;
    private bool broken;
    [SerializeField] private int fixTime = 4;
    public AIInterest[] aIInterests;
    
    void Start()
    {
        _hingeJoints = new List<HingeJoint>(GetComponents<HingeJoint>());
    }

    public void OnKick(GameObject kicker)
    {
        if (breakAllHinges)
        {
            foreach (var hinge in _hingeJoints)
            {
                Destroy(hinge);
            }
        }
        else if (_hingeJoints.Count > 0)
        {
            if (!preBroken && !broken)
            {
                preBroken = Instantiate(gameObject, transform.parent);
                preBroken.SetActive(false);
            }
            int randIndex = Random.Range(0, _hingeJoints.Count);
            HingeJoint hingeJoint = _hingeJoints[randIndex];
            _hingeJoints.RemoveAt(randIndex);
            Destroy(hingeJoint);
        }
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

    public void AIFinishInteract()
    {
        broken = false;
        preBroken.SetActive(true);
        preBroken = null;
        Destroy(gameObject);
    }

    public void AIInteracting(float interactProgress)
    {

    }

    public AIInterest[] InterestingToWhatAI()
    {
        return aIInterests;
    }
}
