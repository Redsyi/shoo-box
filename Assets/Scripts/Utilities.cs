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

    public static bool fEqual(float one, float two)
    {
        const float EPSILON = 0.003f;
        return (Mathf.Abs(one - two) < EPSILON) && (Mathf.Abs(two - one) < EPSILON);
    }
   

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

    public static Vector3 RotateVector(Vector3 baseVector, float radians)
    {
        return new Vector3(baseVector.x * Mathf.Cos(radians) - baseVector.y * Mathf.Sin(radians), baseVector.y * Mathf.Cos(radians) + baseVector.x * Mathf.Sin(radians));
    }

    public static Vector3 RotateVectorDegrees(Vector3 baseVector, float degrees)
    {
        return RotateVector(baseVector, degrees * Mathf.Deg2Rad);
    }

    public static float VectorToRads(Vector2 vector)
    {
        return Mathf.Atan2(vector.y, vector.x);
    }

    public static float VectorToRads(Vector3 vector)
    {
        return Mathf.Atan2(vector.y, vector.x);
    }

    public static float VectorToDegrees(Vector2 vector)
    {
        return Mathf.Atan2(vector.y, vector.x)*Mathf.Rad2Deg;
    }

    public static float VectorToDegrees(Vector3 vector)
    {
        return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
    }

#if UNITY_EDITOR
    /*[MenuItem("Utilities/Go to main scene")]
    public static void MainScene()
    {
        EditorSceneManager.SaveOpenScenes();
        EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
    }*/
#endif



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