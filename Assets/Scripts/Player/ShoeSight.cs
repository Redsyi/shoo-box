using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoeSight : MonoBehaviour
{
    private bool sighting;
    private const float sightSpoolTime = .5f;
    private const float sightTime = 3f;
    private const float sightCooldownTime = 0f;
    public AK.Wwise.Event onShoeSight;

    public void ActivateSight()
    {
        if (!sighting)
        {
            onShoeSight.Post(gameObject);
            StartCoroutine(DoShoeSight());
        }
    }

    IEnumerator DoShoeSight() {
        sighting = true;
        MeshRenderer[] renderers = FindObjectsOfType<MeshRenderer>();
        SkinnedMeshRenderer[] skinnedMeshRenderers = FindObjectsOfType<SkinnedMeshRenderer>();
        float sightStrength = 0;
        while (sightStrength < 1)
        {
            foreach (MeshRenderer renderer in renderers)
            {
                if (renderer)
                {
                    foreach (Material material in renderer.materials)
                    {
                        material.SetFloat("_SightBlend", sightStrength);
                    }
                }
            }
            foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
            {
                if (renderer)
                {
                    foreach (Material material in renderer.materials)
                    {
                        material.SetFloat("_SightBlend", sightStrength);
                    }
                }
            }
            sightStrength += Time.deltaTime / sightSpoolTime;
            yield return null;
        }
        yield return new WaitForSeconds(sightTime);
        while (sightStrength > 0)
        {
            foreach (MeshRenderer renderer in renderers)
            {
                if (renderer)
                {
                    foreach (Material material in renderer.materials)
                    {
                        material.SetFloat("_SightBlend", sightStrength);
                    }
                }
            }
            foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
            {
                if (renderer)
                {
                    foreach (Material material in renderer.materials)
                    {
                        material.SetFloat("_SightBlend", sightStrength);
                    }
                }
            }
            sightStrength -= Time.deltaTime / sightSpoolTime;
            yield return null;
        }
        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer)
            {
                foreach (Material material in renderer.materials)
                {
                    material.SetFloat("_SightBlend", 0);
                }
            }
        }
        foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
        {
            if (renderer)
            {
                foreach (Material material in renderer.materials)
                {
                    material.SetFloat("_SightBlend", 0);
                }
            }
        }
        yield return new WaitForSeconds(sightCooldownTime);
        sighting = false;
    }
}
