using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// base class that helps with city construction, most of the hard work happens in the corresponding editor script
/// </summary>
public class CityTile : MonoBehaviour
{
    public CityTile blankPrefab;
    public Transform prefabSlot;
    public GameObject[] possiblePrefabs;
    public string[] prefabNames;
    public GameObject[] possibleBigPrefabs;
    public string[] bigPrefabNames;
    public bool big;
    public int currPrefabID;
    public MeshRenderer previewTile;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
}
