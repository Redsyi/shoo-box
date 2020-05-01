using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class to manage the player flinging a sandal
/// </summary>
public class SandalSlinger : MonoBehaviour
{
    //custom class to manage a sandal target, sorted by angle difference between desired angle and actual angle
    private class SandalTarget : System.IComparable
    {
        public Transform target;
        public float angleDiff;

        public int CompareTo(object obj)
        {
            SandalTarget other = obj as SandalTarget;
            if (other == null)
            {
                return -1;
            } else
            {
                return (int) Mathf.Sign(angleDiff - other.angleDiff);
            }
        }
    }

    public SandalProjectile sandalPrefab;
    public bool slinging;
    public LineRenderer shotPreview;
    private bool _showShotPreview;
    public bool holdingShot
    {
        get
        {
            return _showShotPreview;
        }
        set
        {
            if (_showShotPreview != value)
            {
                _showShotPreview = value;
                shotPreview.enabled = value;
                targetCollider.enabled = value;
                if (!value)
                {
                    targets.Clear();
                    currTarget = null;
                }
            }
        }
    }
    public float maxDist;
    public Transform currTarget;
    public Vector2 desiredForward;
    private HashSet<Transform> targets;
    public SphereCollider targetCollider;
    public Vector3 vectorToTarget => (currTarget ? (currTarget.position - transform.position) : Vector3.zero);
    public SkinnedMeshRenderer rightSandalModel;
    LayerMask previewMask;
    LayerMask blockingMask;
    public AK.Wwise.Event sandalFlingSound;

    //remove a sandal target from consideration
    public void ClearTarget(Transform target)
    {
        if (targets.Contains(target))
            targets.Remove(target);
    }

    private void Start()
    {
        targets = new HashSet<Transform>();
        previewMask = LayerMask.GetMask("Default", "Obstructions");
        blockingMask = LayerMask.GetMask("Obstructions");
    }

    /// <summary>
    /// initiate a sandal fling at the current target
    /// </summary>
    public void Sling()
    {
        if (!slinging)
        {
            SandalProjectile sandal = Instantiate(sandalPrefab, transform.position, transform.rotation);
            sandal.slinger = this;
            sandal.maxDist = maxDist;
            sandal.target = currTarget;
            rightSandalModel.enabled = false;
            slinging = true;
            sandalFlingSound.Post(gameObject);
        }
    }

    /// <summary>
    /// sets shot preview destination
    /// </summary>
    private void FixedUpdate()
    {
        if (holdingShot)
        {
            CalculateBestTarget();
            if (currTarget)
            {
                shotPreview.SetPosition(1, currTarget.position);
            } else
            {
                RaycastHit hitResult;
                if (Physics.Raycast(transform.position, transform.forward, out hitResult, maxDist, previewMask))
                {
                    shotPreview.SetPosition(1, hitResult.point);
                } else
                {
                    shotPreview.SetPosition(1, transform.position + transform.forward*maxDist);
                }
            }
        }
    }

    /// <summary>
    /// sets shot preview origin
    /// </summary>
    private void LateUpdate()
    {
        if (holdingShot)
        {
            shotPreview.SetPosition(0, shotPreview.transform.position);
        }
    }

    /// <summary>
    /// figure out the best target to fling a sandal at from current available targets
    /// </summary>
    void CalculateBestTarget()
    {
        if (targets.Count == 0)
        {
            currTarget = null;
            return;
        }
        
        //build sorted list of sandal targets
        List<SandalTarget> potentialTargets = new List<SandalTarget>();
        foreach (Transform target in targets)
        {
            if (target)
            {
                Vector3 vectToTarget = (target.position - transform.position);
                Vector2 normalizedVectToTarget = new Vector2(vectToTarget.x, vectToTarget.z).normalized;
                float angleDiff = Mathf.Acos(Vector2.Dot(desiredForward, normalizedVectToTarget));
                potentialTargets.Add(new SandalTarget() { target = target, angleDiff = angleDiff });
            }
        }

        currTarget = null;
        potentialTargets.Sort();

        //select best one, disqualifying any ones that are blocked by something
        foreach (SandalTarget target in potentialTargets)
        {
            Vector3 vectToTarget = (target.target.position - transform.position);
            if (!Physics.Raycast(transform.position, vectToTarget, vectToTarget.magnitude, blockingMask))
            {
                currTarget = target.target;
                return;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!targets.Contains(other.transform))
            targets.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (targets.Contains(other.transform))
            targets.Remove(other.transform);
    }

    private void OnDrawGizmosSelected()
    {
        if (targetCollider)
        {
            targetCollider.radius = maxDist;
        }
    }
}
