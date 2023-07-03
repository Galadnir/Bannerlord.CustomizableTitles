using Bannerlord.TitlesForLords.src.main.Core.Settings;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace Bannerlord.TitleOverhaul.src.main.Core.Settings.SettingsPatches {

	// in case there are unsafed modifications due to renames
	[HarmonyPatch(typeof(Campaign))]
	[HarmonyPatch("OnInitialize")]
	internal static class ReloadModSettingsOnNewCampaignLoadPatch {

		public static void Postfix() {
			ModSettings.Instance.Restore();
		}
	}
}
