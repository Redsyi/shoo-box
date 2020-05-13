using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class responsible for loading player settings at beginning of game and keeping static cached references to values
/// </summary>
public class PlayerData : MonoBehaviour
{
    public class ResolutionComparer : IComparer
    {
        public int Compare(object res1, object res2)
        {
            Resolution? r1 = res1 as Resolution?;
            Resolution? r2 = res2 as Resolution?;
            if (r1 == null || r2 == null)
                return 0;
            else
            {
                if (r1.Value.width == r2.Value.width)
                {
                    if (r1.Value.height > r2.Value.height)
                        return 1;
                    else
                        return -1;
                } else
                {
                    if (r1.Value.width > r2.Value.width)
                        return 1;
                    else
                        return -1;
                }
            }
        }
    }

    static Resolution[] resolutions;
    static string[] qualityNames;

    static bool loadedData;
    static Dictionary<string, float> floatSettings;
    static Dictionary<string, int> intSettings;
    static Dictionary<string, string> stringSettings;
    static Dictionary<GraphicsSetting, bool> dirtyGraphicsSettings;

    static string audioSettingPrefix = "AudioSettings";
    static string ambienceSettingName = "AmbienceVolume";
    static string musicSettingName = "MusicVolume";
    static string sfxSettingName = "SFXVolume";
    static string masterSettingName = "MasterVolume";

    static string resolutionSettingName = "GraphicsSettingsResolution";
    static string fullscreenSettingName = "GraphicsSettingsFullscreen";
    static string qualitySettingName = "GraphicsSettingsQuality";

    static string currLevelName = "ContinueLevelName";
    static string currCheckpointName = "ContinueCheckpoint";

    static string rotateSensitivityName = "LookSensitivity";

    static string _currLevel;
    public static string currLevel
    {
        get
        {
            return _currLevel;
        }
        set
        {
            if (_currLevel != value)
            {
                _currLevel = value;
                PlayerPrefs.SetString(currLevelName, value);
            }
        }
    }
    static int _currCheckpoint;
    public static int currCheckpoint
    {
        get
        {
            return _currCheckpoint;
        }
        set
        {
            if (_currCheckpoint != value)
            {
                _currCheckpoint = value;
                PlayerPrefs.SetInt(currCheckpointName, value);
            }
        }
    }

    public Level[] _levels;
    public static Dictionary<string, Level> levels;
    public static Level defaultLevel;
    public static Dictionary<string, bool> unlockedLevels;
    public static float mouseRotateSensitivity = 0.035f;
    public static float normalizedRotateSensitivity;
    public static float sensitivityMultiplier
    {
        get
        {
            print($"{sensitivityScale}, {normalizedRotateSensitivity}, {Mathf.Lerp(sensitivityScale.x, sensitivityScale.y, normalizedRotateSensitivity)}");
            return Mathf.Lerp(sensitivityScale.x, sensitivityScale.y, normalizedRotateSensitivity);
        }
        set
        {
            normalizedRotateSensitivity = Mathf.InverseLerp(sensitivityScale.x, sensitivityScale.y, value);
        }
    }
    public static readonly Vector2 sensitivityScale = new Vector2(0.05f, 2.5f);

    private void Awake()
    {
        InitLevels();
        CheckLoadedData();
    }

    /// <summary>
    /// loads the level information
    /// </summary>
    void InitLevels()
    {
        levels = new Dictionary<string, Level>();
        unlockedLevels = new Dictionary<string, bool>();
        defaultLevel = _levels[0];
        foreach (Level level in _levels)
        {
            levels[level.saveID] = level;
            if (level.saveID == defaultLevel.saveID)
            {
                unlockedLevels[level.saveID] = true;
            } else
            {
                string levelKey = "Unlocked" + level.saveID;
                if (PlayerPrefs.HasKey(levelKey))
                {
                    unlockedLevels[level.saveID] = PlayerPrefs.GetInt(levelKey) != 0;
                } else
                {
                    unlockedLevels[level.saveID] = false;
                }
            }
        }
        LoadContinues();
    }

    /// <summary>
    /// resets all player progress
    /// </summary>
    public static void ResetAllProgress()
    {
        //lock levels
        if (levels != null)
        {
            foreach (Level level in levels.Values)
            {
                if (level.saveID != defaultLevel.saveID)
                {
                    LockLevel(level);
                }
            }
        }

        //reset checkpoints
        currLevel = defaultLevel.saveID;
        currCheckpoint = 0;

        //lock jibbitz
        JibbitManager.ClearJibz();
    }

    /// <summary>
    /// unlocks the given level in the level select
    /// </summary>
    public static void UnlockLevel(Level level)
    {
        if (unlockedLevels != null) {
            unlockedLevels[level.saveID] = true;
            PlayerPrefs.SetInt("Unlocked" + level.saveID, 1);
        }
    }

    /// <summary>
    /// locks the given level in the level select
    /// </summary>
    public static void LockLevel(Level level)
    {
        if (unlockedLevels != null)
        {
            unlockedLevels[level.saveID] = false;
            PlayerPrefs.SetInt("Unlocked" + level.saveID, 0);
        }
    }

    /// <summary>
    /// ensures that we have loaded settings since the game launched, if not, load them
    /// </summary>
    public static void CheckLoadedData()
    {
        if (!loadedData)
        {
            loadedData = true;
            floatSettings = new Dictionary<string, float>();
            intSettings = new Dictionary<string, int>();
            stringSettings = new Dictionary<string, string>();
            LoadSoundSettings();

            resolutions = Screen.resolutions;
            System.Array.Sort(resolutions, new ResolutionComparer());
            qualityNames = QualitySettings.names;
            dirtyGraphicsSettings = new Dictionary<GraphicsSetting, bool>();
            LoadGraphicsSettings();
            LoadControlSettings();
        }
    }

    /// <summary>
    /// loads the saved controls info (currently just sensitivity)
    /// </summary>
    public static void LoadControlSettings()
    {
        if (PlayerPrefs.HasKey(rotateSensitivityName))
            normalizedRotateSensitivity = PlayerPrefs.GetFloat(rotateSensitivityName);
        else
            normalizedRotateSensitivity = 0.4f;
    }
    
    /// <summary>
    /// saves the controls info (currently just sensitivity)
    /// </summary>
    public static void SaveControlSettings()
    {
        PlayerPrefs.SetFloat(rotateSensitivityName, normalizedRotateSensitivity);
    }

    /// <summary>
    /// loads the furthest reached point in the game
    /// </summary>
    public static void LoadContinues()
    {
        if (PlayerPrefs.HasKey(currLevelName))
            _currLevel = PlayerPrefs.GetString(currLevelName);
        else
            currLevel = defaultLevel.saveID;

        if (PlayerPrefs.HasKey(currCheckpointName))
            _currCheckpoint = PlayerPrefs.GetInt(currCheckpointName);
        else
            currCheckpoint = 0;
    }

    /// <summary>
    /// sets which resolution we should use, as an index into the resolutions array
    /// </summary>
    public static void SetResolutionIndex(int index)
    {
        CheckLoadedData();
        if (index < 0 || index >= resolutions.Length)
            return;

        intSettings[resolutionSettingName] = index;
        dirtyGraphicsSettings[GraphicsSetting.RESOLUTION_INDEX] = true;
    }

    /// <summary>
    /// sets whether we should be fullscreen or not
    /// </summary>
    public static void SetFullscreen(bool fullscreen)
    {
        CheckLoadedData();
        intSettings[fullscreenSettingName] = (fullscreen ? 1 : 0);
        dirtyGraphicsSettings[GraphicsSetting.FULLSCREEN] = true;
    }

    /// <summary>
    /// sets which quality level we should use, as an index into the qualityNames array
    /// </summary>
    public static void SetQualityIndex(int index)
    {
        CheckLoadedData();
        if (index < 0 || index >= qualityNames.Length)
            return;

        intSettings[qualitySettingName] = index;
        dirtyGraphicsSettings[GraphicsSetting.QUALITY_INDEX] = true;
    }

    /// <summary>
    /// applies any graphics settings that have been changed, or force = true to apply everything
    /// </summary>
    public static void ApplyGraphicsSettings(bool force = false)
    {
        CheckLoadedData();
        if (dirtyGraphicsSettings[GraphicsSetting.FULLSCREEN] || dirtyGraphicsSettings[GraphicsSetting.RESOLUTION_INDEX] || force)
        {
            Resolution chosenResolution = resolutions[intSettings[resolutionSettingName]];
            FullScreenMode fullScreenMode = (intSettings[fullscreenSettingName] != 0 ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed);
            Screen.SetResolution(chosenResolution.width, chosenResolution.height, fullScreenMode);
            dirtyGraphicsSettings[GraphicsSetting.FULLSCREEN] = false;
            dirtyGraphicsSettings[GraphicsSetting.RESOLUTION_INDEX] = false;

            PlayerPrefs.SetInt(resolutionSettingName, intSettings[resolutionSettingName]);
            PlayerPrefs.SetInt(fullscreenSettingName, intSettings[fullscreenSettingName]);
        }

        if (dirtyGraphicsSettings[GraphicsSetting.QUALITY_INDEX] || force)
        {
            QualitySettings.SetQualityLevel(intSettings[qualitySettingName]);
            dirtyGraphicsSettings[GraphicsSetting.QUALITY_INDEX] = false;

            PlayerPrefs.SetInt(qualitySettingName, intSettings[qualitySettingName]);
        }
    }

    /// <summary>
    /// gets a list of WIDTHxHEIGHT names for the available resolutions
    /// </summary>
    public static string[] GetResolutionNames()
    {
        CheckLoadedData();
        string[] results = new string[resolutions.Length];
        for (int i = 0; i < resolutions.Length; ++i)
        {
            results[i] = $"{resolutions[i].width}x{resolutions[i].height}";
        }
        return results;
    }

    /// <summary>
    /// gets a list of names for the quality settings
    /// </summary>
    public static string[] GetQualityNames()
    {
        CheckLoadedData();
        return qualityNames;
    }

    /// <summary>
    /// gets the specified graphics setting
    /// </summary>
    public static int GetGraphicSetting(GraphicsSetting settingName)
    {
        CheckLoadedData();
        switch (settingName)
        {
            case GraphicsSetting.FULLSCREEN:
                return intSettings[fullscreenSettingName];
            case GraphicsSetting.QUALITY_INDEX:
                return intSettings[qualitySettingName];
            case GraphicsSetting.RESOLUTION_INDEX:
                return intSettings[resolutionSettingName];
            default:
                return 0;
        }
    }

    /// <summary>
    /// loads and applies graphics settings
    /// </summary>
    static void LoadGraphicsSettings()
    {
        CheckLoadedData();
        dirtyGraphicsSettings[GraphicsSetting.FULLSCREEN] = false;
        dirtyGraphicsSettings[GraphicsSetting.QUALITY_INDEX] = false;
        dirtyGraphicsSettings[GraphicsSetting.RESOLUTION_INDEX] = false;

        if (PlayerPrefs.HasKey(resolutionSettingName))
        {
            int index = PlayerPrefs.GetInt(resolutionSettingName);
            if (index < 0 || index >= resolutions.Length)
            {
                index = resolutions.Length - 1;
            }
            intSettings[resolutionSettingName] = index;
        } else
        {
            intSettings[resolutionSettingName] = resolutions.Length - 1;
        }

        if (PlayerPrefs.HasKey(qualitySettingName))
        {
            int index = PlayerPrefs.GetInt(qualitySettingName);
            if (index < 0 || index >= qualityNames.Length)
            {
                index = qualityNames.Length - 1;
            }
            intSettings[qualitySettingName] = index;
        }
        else
        {
            intSettings[qualitySettingName] = qualityNames.Length - 1;
        }

        if (PlayerPrefs.HasKey(fullscreenSettingName))
        {
            int fullscreen = PlayerPrefs.GetInt(fullscreenSettingName);
            intSettings[fullscreenSettingName] = fullscreen;
        }
        else
        {
            intSettings[fullscreenSettingName] = 1;
        }

        ApplyGraphicsSettings(true);
    }

    /// <summary>
    /// Loads sound settings from disk
    /// </summary>
    static void LoadSoundSettings()
    {
        CheckLoadedData();
        LoadVolume(masterSettingName, audioSettingPrefix + masterSettingName);
        LoadVolume(musicSettingName, audioSettingPrefix + musicSettingName);
        LoadVolume(sfxSettingName, audioSettingPrefix + sfxSettingName);
        LoadVolume(ambienceSettingName, audioSettingPrefix + ambienceSettingName);
    }

    /// <summary>
    /// loads the given volume setting and sets the corresponding volume
    /// </summary>
    static void LoadVolume(string rtpcName, string prefsName)
    {
        CheckLoadedData();
        if (PlayerPrefs.HasKey(prefsName))
        {
            float volume = PlayerPrefs.GetFloat(prefsName);
            AkSoundEngine.SetRTPCValue(rtpcName, volume * 100);
            floatSettings[prefsName] = volume;
        }
        else
        {
            floatSettings[prefsName] = 1;
        }
    }

    /// <summary>
    /// gets the designated volume value as a value between 0 and 1
    /// </summary>
    public static float GetVolume(SoundSettings type)
    {
        CheckLoadedData();
        string setting = audioSettingPrefix;
        switch(type)
        {
            case SoundSettings.AMBIENCE_VOLUME:
                setting += ambienceSettingName;
                break;
            case SoundSettings.MASTER_VOLUME:
                setting += masterSettingName;
                break;
            case SoundSettings.MUSIC_VOLUME:
                setting += musicSettingName;
                break;
            case SoundSettings.SFX_VOLUME:
                setting += sfxSettingName;
                break;
        }
        return floatSettings[setting];
    }

    /// <summary>
    /// sets the designated volume value to a value between 0 and 1
    /// </summary>
    public static void SetVolume(SoundSettings type, float value)
    {
        CheckLoadedData();
        string setting = audioSettingPrefix;
        switch (type)
        {
            case SoundSettings.AMBIENCE_VOLUME:
                setting += ambienceSettingName;
                break;
            case SoundSettings.MASTER_VOLUME:
                setting += masterSettingName;
                break;
            case SoundSettings.MUSIC_VOLUME:
                setting += musicSettingName;
                break;
            case SoundSettings.SFX_VOLUME:
                setting += sfxSettingName;
                break;
        }
        floatSettings[setting] = value;
        PlayerPrefs.SetFloat(setting, value);
    }

    /// <summary>
    /// gets the Wwise name for a volume
    /// </summary>
    public static string GetVolumeRTPCName(SoundSettings type)
    {
        CheckLoadedData();
        switch (type)
        {
            case SoundSettings.AMBIENCE_VOLUME:
                return ambienceSettingName;
            case SoundSettings.MASTER_VOLUME:
                return masterSettingName;
            case SoundSettings.MUSIC_VOLUME:
                return musicSettingName;
            case SoundSettings.SFX_VOLUME:
                return sfxSettingName;
            default:
                return "";
        }
    }
}
