using UnityEditor;
using UnityEngine;

//EditorUtility.SetDirty(target);
[CustomEditor(typeof(CityTile))]
public class CityTileInspector : Editor
{
    bool expandDefault;
    public override void OnInspectorGUI()
    {
        CityTile tile = (CityTile)target;

        expandDefault = EditorGUILayout.Foldout(expandDefault, "Default Inspector");

        if (expandDefault)
            base.DrawDefaultInspector();

        EditorGUILayout.LabelField("Create New Tiles", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Tile North"))
        {
            CreateTile(Vector3.back * 5 * tile.transform.localScale.z);
        }
        if (GUILayout.Button("Create Tile East"))
        {
            CreateTile(Vector3.left * (tile.big ? 10 : 5) * tile.transform.localScale.x);
        }
        if (GUILayout.Button("Create Tile South"))
        {
            CreateTile(Vector3.forward * (tile.big ? 10 : 5) * tile.transform.localScale.z);
        }
        if (GUILayout.Button("Create Tile West"))
        {
            CreateTile(Vector3.right * 5 * tile.transform.localScale.x);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Change Tile Content", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        tile.currPrefabID = EditorGUILayout.Popup(tile.currPrefabID, (!tile.big ? tile.prefabNames : tile.bigPrefabNames));
        if (GUILayout.Button("Save"))
        {
            SwapPrefab((!tile.big ? tile.possiblePrefabs[tile.currPrefabID] : tile.possibleBigPrefabs[tile.currPrefabID]));
            EditorUtility.SetDirty(target);
        }
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button((tile.big ? "Make small" : "Make big")))
        {
            DestroyPrefab(true);
            tile.big = !tile.big;
            EditorUtility.SetDirty(target);
            if (tile.big)
            {
                tile.previewTile.transform.localScale = new Vector3(1, 1, 1);
                tile.previewTile.transform.localPosition = new Vector3(-2.5f, 0, 2.5f);
                tile.prefabSlot.transform.localPosition = new Vector3(-2.5f, 0, 2.5f);
            } else
            {
                tile.previewTile.transform.localScale = new Vector3(0.5f, 1, 0.5f);
                tile.previewTile.transform.localPosition = Vector3.zero;
                tile.prefabSlot.transform.localPosition = Vector3.zero;
            }
        }
        EditorGUILayout.LabelField("Move Tile", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Rotate Counter-Clockwise"))
        {
            tile.prefabSlot.localEulerAngles = new Vector3(0, tile.prefabSlot.localEulerAngles.y - 90);
        }
        if (GUILayout.Button("Rotate Clockwise"))
        {
            tile.prefabSlot.localEulerAngles = new Vector3(0, tile.prefabSlot.localEulerAngles.y + 90);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Shift North"))
        {
            tile.transform.Translate(Vector3.back * 5 * tile.transform.localScale.z);
        }
        if (GUILayout.Button("Shift East"))
        {
            tile.transform.Translate(Vector3.left * 5 * tile.transform.localScale.x);
        }
        if (GUILayout.Button("Shift South"))
        {
            tile.transform.Translate(Vector3.forward * 5 * tile.transform.localScale.z);
        }
        if (GUILayout.Button("Shift West"))
        {
            tile.transform.Translate(Vector3.right * 5 * tile.transform.localScale.x);
        }
        EditorGUILayout.EndHorizontal();
    }

    void CreateTile(Vector3 relativePos)
    {
        CityTile tile = (CityTile)target;
        string assetpath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tile.blankPrefab);
        GameObject go = AssetDatabase.LoadAssetAtPath(assetpath, typeof(GameObject)) as GameObject;
        GameObject newTile = PrefabUtility.InstantiatePrefab(go) as GameObject;
        newTile.transform.SetParent(tile.transform.parent);
        newTile.transform.position = tile.transform.position + relativePos;
        newTile.transform.localScale = tile.transform.localScale;
    }

    void SwapPrefab(GameObject newPrefab)
    {
        CityTile tile = (CityTile)target;
        DestroyPrefab(newPrefab == null);
        if (newPrefab != null)
        {
            tile.previewTile.enabled = false;
            GameObject prefab = PrefabUtility.InstantiatePrefab(newPrefab) as GameObject;
            prefab.transform.SetParent(tile.prefabSlot);
            prefab.transform.localPosition = Vector3.zero;
        }
    }

    void DestroyPrefab(bool clearPrefabID)
    {
        CityTile tile = (CityTile)target;
        if (tile.prefabSlot.childCount > 0)
        {
            DestroyImmediate(tile.prefabSlot.GetChild(0).gameObject);
            tile.previewTile.enabled = true;
            if (clearPrefabID)
                tile.currPrefabID = 0;
            EditorUtility.SetDirty(target);
        }
    }
}
