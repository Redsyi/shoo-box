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

    /// <summary>
    /// clamps an angle between 0 and 360 degrees
    /// </summary>
    public static float ClampAngle0360(float angle)
    {
        while (angle < 0)
            angle += 360;
        while (angle >= 360)
            angle -= 360;
        return angle;
    }

    public static int DirectionToRotate(float angleFrom, float angleTo)
    {
        float diff = angleTo - angleFrom;
        float absDiff = Mathf.Abs(diff);
        return (diff > 0 ? 1 : -1) * (absDiff > 180 ? -1 : 1);
    }

#if UNITY_EDITOR
    [MenuItem("Scenes/Main Menu")] public static void MainMenuScene() => 
        OpenScene("Assets/Scenes/MainMenu.unity");
    [MenuItem("Scenes/Main/Hotel Tutorial")] public static void TutorialScene() => 
        OpenScene("Assets/Scenes/Hotel/hotel_tutorial.unity");
    [MenuItem("Scenes/Main/Hotel Top Floor")] public static void TopFloorScene() => 
        OpenScene("Assets/Scenes/Hotel/hotel_top_level.unity");
    [MenuItem("Scenes/Main/Hotel Lobby")] public static void LobbyScene() => 
        OpenScene("Assets/Scenes/Hotel/hotel_lobby.unity");
    [MenuItem("Scenes/Main/Airport Conveyors 1")] public static void Conveyors1() => 
        OpenScene("Assets/Scenes/Airport/conveyorBeltWarehouse_1.unity");
    [MenuItem("Scenes/Main/Airport Conveyors 2")] public static void Conveyors2() => 
        OpenScene("Assets/Scenes/Airport/conveyorBeltWarehouse_2.unity");
    [MenuItem("Scenes/Main/Airport Terminal 1")] public static void Terminal1() => 
        OpenScene("Assets/Scenes/Airport/Terminal_1.unity");
    [MenuItem("Scenes/Main/Airport Terminal 2")] public static void Terminal2() => 
        OpenScene("Assets/Scenes/Airport/Terminal_2.unity");
    [MenuItem("Scenes/Main/City")] public static void City() => 
        OpenScene("Assets/Scenes/City/city.unity");
    [MenuItem("Scenes/Cutscenes/Intro Cutscene")] public static void IntroCS() => 
        OpenScene("Assets/Scenes/Cutscenes/IntroCutscene.unity");
    [MenuItem("Scenes/Cutscenes/Airport Cutscene")] public static void AirportCS() => 
        OpenScene("Assets/Scenes/Cutscenes/AirportCutscene.unity");
    [MenuItem("Scenes/Testing/Jake's Shadow Test")] public static void ShadowTest() => 
        OpenScene("Assets/Scenes/Hotel/jake-hotel-test.unity");
    [MenuItem("Scenes/Add or Edit Scene Menu Options")] public static void EditMenu() =>
        AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath("Assets/Scripts/Utility/Utilities.cs", typeof(Object)), 56);
    private static void OpenScene(string scenePath)
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene(scenePath);
    }
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