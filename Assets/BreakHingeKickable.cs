using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakHingeKickable : MonoBehaviour, IKickable
{
    // Start is called before the first frame update
    private List<HingeJoint> _hingeJoints;
    [SerializeField] private bool breakAllHinges;
    
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
            int randIndex = Random.Range(0, _hingeJoints.Count);
            HingeJoint hingeJoint = _hingeJoints[randIndex];
            _hingeJoints.RemoveAt(randIndex);
            Destroy(hingeJoint);
        }
    }
}
