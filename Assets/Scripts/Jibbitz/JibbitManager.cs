using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JibbitManager : MonoBehaviour
{
    public Jibbit[] jibbitz;
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
    /// acquire the jibbit given by jibbitID and save this fact to disk
    /// </summary>
    public static void AcquireJibbit(string jibbitID)
    {
        if (acquired.ContainsKey(jibbitID) && !acquired[jibbitID])
        {
            acquired[jibbitID] = true;
            PlayerPrefs.SetInt("HasJibbit" + jibbitID, 1);
        }
    }

    /// <summary>
    /// Returns if the player has acquired the given jibbit id
    /// </summary>
    public static bool HasJibbit(string jibbitID)
    {
        return (acquired.ContainsKey(jibbitID) && acquired[jibbitID]);
    }
}
