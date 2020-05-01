using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that manages the shoe sight ability for the playewr
/// </summary>
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

    //main ability driver: it's a wall of text but pretty simple at it's core
    IEnumerator DoShoeSight() {
        sighting = true;

        //first, we get all active mesh renderers in the scene
        MeshRenderer[] renderers = FindObjectsOfType<MeshRenderer>();
        SkinnedMeshRenderer[] skinnedMeshRenderers = FindObjectsOfType<SkinnedMeshRenderer>();
        float sightStrength = 0;

        //then we lerp the "_SightBlend" property of all the renderer's materials up...
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
        //wait for a bit...
        yield return new WaitForSeconds(sightTime);
        //then lerp them down again.
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
        //finally setting them to 0 to prevent residue glow
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
