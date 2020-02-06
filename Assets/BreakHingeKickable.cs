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
    
    void Start()
    {
        _hingeJoints = new List<HingeJoint>(GetComponents<HingeJoint>());
    }

    public void OnKick(GameObject kicker)
    {
        Debug.Log("print");
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
                preBroken = Instantiate(gameObject);
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
        broken = false;
        preBroken.SetActive(true);
        preBroken = null;
        Destroy(gameObject);
    }

    public float AIInteractTime()
    {
        return fixTime;
    }
}
