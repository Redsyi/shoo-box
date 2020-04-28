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

    private void Awake()
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
        }
    }

    public static void SetResolutionIndex(int index)
    {
        if (index < 0 || index >= resolutions.Length)
            return;

        intSettings[resolutionSettingName] = index;
        dirtyGraphicsSettings[GraphicsSetting.RESOLUTION_INDEX] = true;
    }

    public static void SetFullscreen(bool fullscreen)
    {
        intSettings[fullscreenSettingName] = (fullscreen ? 1 : 0);
        dirtyGraphicsSettings[GraphicsSetting.FULLSCREEN] = true;
    }

    public static void SetQualityIndex(int index)
    {
        if (index < 0 || index >= qualityNames.Length)
            return;

        intSettings[qualitySettingName] = index;
        dirtyGraphicsSettings[GraphicsSetting.QUALITY_INDEX] = true;
    }

    public static void ApplyGraphicsSettings(bool force = false)
    {
        if (dirtyGraphicsSettings[GraphicsSetting.FULLSCREEN] || dirtyGraphicsSettings[GraphicsSetting.RESOLUTION_INDEX] || force)
        {
            Resolution chosenResolution = resolutions[intSettings[resolutionSettingName]];
            FullScreenMode fullScreenMode = (intSettings[fullscreenSettingName] != 0 ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed);
            Screen.SetResolution(chosenResolution.width, chosenResolution.height, fullScreenMode);
            dirtyGraphicsSettings[GraphicsSetting.FULLSCREEN] = false;
            dirtyGraphicsSettings[GraphicsSetting.RESOLUTION_INDEX] = false;
        }

        if (dirtyGraphicsSettings[GraphicsSetting.QUALITY_INDEX] || force)
        {
            QualitySettings.SetQualityLevel(intSettings[qualitySettingName]);
            dirtyGraphicsSettings[GraphicsSetting.QUALITY_INDEX] = false;
        }
    }

    public static string[] GetResolutionNames()
    {
        string[] results = new string[resolutions.Length];
        for (int i = 0; i < resolutions.Length; ++i)
        {
            results[i] = $"{resolutions[i].width}x{resolutions[i].height}";
        }
        return results;
    }

    public static string[] GetQualityNames()
    {
        return qualityNames;
    }

    public static int GetGraphicSetting(GraphicsSetting settingName)
    {
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

    void LoadGraphicsSettings()
    {
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

    void LoadSoundSettings()
    {
        LoadVolume(masterSettingName, audioSettingPrefix + masterSettingName);
        LoadVolume(musicSettingName, audioSettingPrefix + musicSettingName);
        LoadVolume(sfxSettingName, audioSettingPrefix + sfxSettingName);
        LoadVolume(ambienceSettingName, audioSettingPrefix + ambienceSettingName);
    }

    void LoadVolume(string rtpcName, string prefsName)
    {
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

    public static float GetVolume(SoundSettings type)
    {
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

    public static void SetVolume(SoundSettings type, float value)
    {
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

    public static string GetVolumeRTPCName(SoundSettings type)
    {
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
