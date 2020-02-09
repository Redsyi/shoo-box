using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine.UI;

public static class Utilities
{
    public static Vector2 AngleToVector2(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }
    
    public static Vector2 RotateVector(Vector2 baseVector, float radians)
    {
        return new Vector2(baseVector.x * Mathf.Cos(radians) - baseVector.y * Mathf.Sin(radians), baseVector.y * Mathf.Cos(radians) + baseVector.x * Mathf.Sin(radians));
    }

    public static Vector2 RotateVectorDegrees(Vector2 baseVector, float degrees)
    {
        return RotateVector(baseVector, degrees * Mathf.Deg2Rad);
    }

    public static float VectorToRads(Vector2 vector)
    {
        return Mathf.Atan2(vector.y, vector.x);
    }

    public static float VectorToDegrees(Vector2 vector)
    {
        return Mathf.Atan2(vector.y, vector.x)*Mathf.Rad2Deg;
    }

#if UNITY_EDITOR
    //todo we can add our own scenes here
    /*[MenuItem("Utilities/Go to main scene")]
    public static void MainScene()
    {
        EditorSceneManager.SaveOpenScenes();
        EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
    }*/
#endif

    //this one was somewhat niche but i'll keep it in in case we need it
    //sets a dropdown to the item in its list that has the value 'value'
    public static void SetDropdownValue(Dropdown dropdown, string value)
    {
        if (dropdown)
        {
            for (int i = 0; i < dropdown.options.Count; ++i)
            {
                if (dropdown.options[i].text == value)
                {
                    dropdown.value = i;
                }
            }
        }
    }
}