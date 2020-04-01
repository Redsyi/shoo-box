﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandalSlinger : MonoBehaviour
{
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

    public void ClearTarget(Transform target)
    {
        if (targets.Contains(target))
            targets.Remove(target);
    }

    private void Start()
    {
        targets = new HashSet<Transform>();
    }

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
        }
    }

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
                if (Physics.Raycast(transform.position, transform.forward, out hitResult, maxDist, LayerMask.GetMask("Default", "Obstructions")))
                {
                    shotPreview.SetPosition(1, hitResult.point);
                } else
                {
                    shotPreview.SetPosition(1, transform.position + transform.forward*maxDist);
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (holdingShot)
        {
            shotPreview.SetPosition(0, shotPreview.transform.position);
        }
    }

    void CalculateBestTarget()
    {
        if (targets.Count == 0)
        {
            currTarget = null;
            return;
        }
        float minAngleDiff = 99999;
        foreach (Transform target in targets)
        {
            Vector3 vectToTarget = (target.position - transform.position);
            Vector2 normalizedVectToTarget = new Vector2(vectToTarget.x, vectToTarget.z).normalized;
            float angleDiff = Mathf.Acos(Vector2.Dot(desiredForward, normalizedVectToTarget));
            if (angleDiff < minAngleDiff)
            {
                currTarget = target;
                minAngleDiff = angleDiff;
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
