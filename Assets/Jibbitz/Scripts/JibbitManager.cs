using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that manages acquired jibbitz
/// </summary>
public class JibbitManager : MonoBehaviour
{
    public Jibbit[] jibbitz;
    public CollectableJibbit collectableJibbitPrefab;
    static CollectableJibbit _collectableJibbitPrefab;
    static Jibbit[] staticJibs;
    static Dictionary<string, bool> acquired;
    static bool initialLoad;

    private void Awake()
    {
        if (!initialLoad)
        {
            staticJibs = jibbitz;
            initialLoad = true;
            acquired = new Dictionary<string, bool>();
            LoadJibz();
            _collectableJibbitPrefab = collectableJibbitPrefab;
        }
    }

    /// <summary>
    /// load the jibbits acquired from disk
    /// </summary>
    public static void LoadJibz()
    {
        if (staticJibs != null)
        {
            foreach (Jibbit jibbit in staticJibs)
            {
                if (PlayerPrefs.HasKey("HasJibbit" + jibbit.id))
                {
                    acquired[jibbit.id] = PlayerPrefs.GetInt("HasJibbit" + jibbit.id) != 0;
                } else
                {
                    PlayerPrefs.SetInt("HasJibbit" + jibbit.id, 0);
                    acquired[jibbit.id] = false;
                }
            }
        }
    }

    /// <summary>
    /// Clear all jibbit progress on disk
    /// </summary>
    public static void ClearJibz()
    {
        if (staticJibs != null)
        {
            foreach (Jibbit jibbit in staticJibs)
            {
                acquired[jibbit.id] = false;
                PlayerPrefs.SetInt("HasJibbit" + jibbit.id, 0);
            }
        }
    }

    /// <summary>
    /// acquire the jibbit given by jibbitID and save this fact to disk
    /// </summary>
    public static void AcquireJibbit(Jibbit jibbit)
    {
        if (acquired != null && acquired.ContainsKey(jibbit.id) && !acquired[jibbit.id])
        {
            if (!acquired[jibbit.id])
            {
                /*if (Steamworks.SteamManager.initialized)
                {
                    Steamworks.SteamUserStats.SetAchievement(jibbit.steamAchievementID);
                    Steamworks.SteamUserStats.StoreStats();
                }*/
            }
            acquired[jibbit.id] = true;
            PlayerPrefs.SetInt("HasJibbit" + jibbit.id, 1);
        }
    }

    /// <summary>
    /// Returns if the player has acquired the given jibbit id
    /// </summary>
    public static bool HasJibbit(string jibbitID)
    {
        return (acquired != null && acquired.ContainsKey(jibbitID) && acquired[jibbitID]);
    }

    /// <summary>
    /// Launches a collectable jibbit prefab
    /// </summary>
    public static void LaunchCollectableJibbit(Jibbit jibbit, Vector3 position, Vector3 magnitude, float size, Color color, float timeUntilCollectable)
    {
        CollectableJibbit newJib;
        if (_collectableJibbitPrefab == null)
        {
            newJib = Instantiate(Resources.Load<CollectableJibbit>("IngameJibbit"), position, Quaternion.identity);
        } else
        {
            newJib = Instantiate(_collectableJibbitPrefab, position, Quaternion.identity);
        }

        newJib.Launch(magnitude, color, size, jibbit, timeUntilCollectable);
    }
}
