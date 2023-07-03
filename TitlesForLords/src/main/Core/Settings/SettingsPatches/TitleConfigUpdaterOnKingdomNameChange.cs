using Bannerlord.TitlesForLords.src.main.Core.Settings;
using Bannerlord.TitlesForLords.src.main.Core.Settings.SettingsPatches;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace Bannerlord.TitleOverhaul.src.main.Core.Settings.SettingsPatches {

	[HarmonyPatch(typeof(Kingdom))]
	[HarmonyPatch(nameof(Kingdom.Name), MethodType.Setter)]
	internal static class TitleConfigUpdaterOnKingdomNameChange {

		public static void Prefix(Kingdom __instance, out string __state) {
			__state = __instance.Name?.ToString();
		}

		public static void Postfix(Kingdom __instance, string __state) {
			if (__state is null || __instance.Name is null || __state == __instance.Name.ToString() || !ModSettings.Instance.TrackAllNameChanges) { // when loading same kingdom name is apparently set again
				return;
			}
			string oldName = __state;
			foreach (TitleConfiguration titleConfig in ModSettings.Instance.ActiveTitleConfigs) {
				var currentTitleConfig = titleConfig;
				if (titleConfig.WouldUpdateDueToKingdomNameChange(oldName, __instance.Name.ToString())) {
					if (TitleConfigUpdaterHelper.CopyConfigOnChangeCondition(titleConfig)) {
						currentTitleConfig = TitleConfigUpdaterHelper.CopyAndChangeConfig(titleConfig);
					}
					currentTitleConfig.UpdateDueToKingdomNameChange(oldName, __instance.Name.ToString());
					AddCampaignEventReceiversPatch.CurrentReceiver.SaveOnNextInGameSave = true;
					if (!ModSettings.Instance.UpdateAllConfigsOnAnyNameChange) {
						break;
					}
				}
			}
		}
	}
}
