using Bannerlord.TitleOverhaul.src.main.Core.Settings.SettingsPatches;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party.PartyComponents;

namespace Bannerlord.TitlesForLords.src.main.Core.Settings.SettingsPatches {

	[HarmonyPatch(typeof(Clan))]
	[HarmonyPatch(nameof(Clan.ChangeClanName))]
	internal static class TitleConfigUpdaterOnClanNameChange {

		public static void Prefix(Clan __instance, out string __state) {
			__state = __instance.Name?.ToString();
		}

		public static void Postfix(Clan __instance, string __state) {
			if (__state is null ||  __instance.Name is null || __state == __instance.Name.ToString() || !ModSettings.Instance.TrackAllNameChanges) {
				return;
			}
			string oldClanName = __state;
			foreach (TitleConfiguration titleConfig in ModSettings.Instance.ActiveTitleConfigs) {
				var currentTitleConfig = titleConfig;
				if (titleConfig.WouldUpdateDueToClanNameChange(oldClanName, __instance)) {
					if (TitleConfigUpdaterHelper.CopyConfigOnChangeCondition(titleConfig)) {
						currentTitleConfig = TitleConfigUpdaterHelper.CopyAndChangeConfig(titleConfig);
					}
					currentTitleConfig.UpdateDueToClanNameChange(oldClanName, __instance);
					AddCampaignEventReceiversPatch.CurrentReceiver.SaveOnNextInGameSave = true;
					if (!ModSettings.Instance.UpdateAllConfigsOnAnyNameChange) {
						break;
					}
				}
			}
			foreach (PartyComponent party in __instance.WarPartyComponents) {
				party.ClearCachedName(); // doesn't automatically happen on clan name change but may happen with this mod
			}
		}
	}
}
