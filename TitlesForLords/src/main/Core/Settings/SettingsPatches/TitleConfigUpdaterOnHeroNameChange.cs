using Bannerlord.TitleOverhaul.src.main.Core.Settings.SettingsPatches;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;

namespace Bannerlord.TitlesForLords.src.main.Core.Settings.SettingsPatches {

	[HarmonyPatch]
	internal static class TitleConfigUpdaterOnHeroNameChange {

		public static void Prefix(Hero __instance, out string __state) {
			__state = TitleConfiguration.GetFullHeroName(__instance);
		}

		public static void Postfix(Hero __instance, string __state) {
			string oldName = __state;
			if (!ModSettings.Instance.TrackAllNameChanges || TitleConfiguration.GetFullHeroName(__instance) == TitleConfiguration.GetFullHeroNameOutputIfNameIsNull || oldName == TitleConfiguration.GetFullHeroName(__instance)) {
				return;
			}
			foreach (TitleConfiguration titleConfig in ModSettings.Instance.ActiveTitleConfigs) {
				var currentTitleConfig = titleConfig;
				if (titleConfig.WouldUpdateDueToHeroNameChange(oldName, TitleConfiguration.GetFullHeroName(__instance))) {
					if (TitleConfigUpdaterHelper.CopyConfigOnChangeCondition(titleConfig)) {
						currentTitleConfig = TitleConfigUpdaterHelper.CopyAndChangeConfig(titleConfig);
					}
					currentTitleConfig.UpdateDueToHeroNameChange(oldName, TitleConfiguration.GetFullHeroName(__instance));
					AddCampaignEventReceiversPatch.CurrentReceiver.SaveOnNextInGameSave = true;
					if (!ModSettings.Instance.UpdateAllConfigsOnAnyNameChange) {
						break;
					}
				}
			}
		}

		[HarmonyTargetMethods]
		internal static IEnumerable<MethodBase> Targets() {
			yield return AccessTools.DeclaredMethod(typeof(Hero), nameof(Hero.SetName));
			yield return AccessTools.DeclaredPropertySetter(typeof(Hero), nameof(Hero.Clan));
		}
	}
}
