public class AkLinuxSettings : AkWwiseInitializationSettings.CommonPlatformSettings
{
#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoadMethod]
	private static void AutomaticPlatformRegistration()
	{
		RegisterPlatformSettingsClass<AkLinuxSettings>("Linux");
	}
#endif // UNITY_EDITOR
}
