using Bannerlord.TitlesForLords.src.main.Core.Settings;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace Bannerlord.TitleOverhaul.src.main.Core.GamePatches {

	[HarmonyPatch(typeof(Army))]
	[HarmonyPatch(nameof(Army.Name), MethodType.Getter)]
	public static class ArmyNameGetterPatch {

		public static bool isActive = true;

		static bool modifyNextCall = true;

		public static TextObject Postfix(TextObject name, Army __instance) {
			if (!isActive || !modifyNextCall || __instance.LeaderParty is null || __instance.LeaderParty.LeaderHero is null) {
				modifyNextCall = true;
				return name;
			}
			foreach (var config in ModSettings.Instance.ActiveTitleConfigs) {
				if (config.TryApplyToArmy(__instance, out TextObject modifiedName)) {
					return modifiedName;
				}
			}
			return GetUnmodifiedNameWithTitleAdded(__instance);
		}

		public static TextObject GetUnmodifiedName(Army army) {
			modifyNextCall = false;
			return army.Name;
		}

		public static TextObject GetUnmodifiedNameWithTitleAdded(Army army) {
			var nameWithTitle = army.LeaderParty.LeaderHero.Name;
			GamePatchesHelper.DisableAllGetterPatches();
			var modifiedName = new TextObject(army.Name.ToString().Replace(army.LeaderParty.LeaderHero.Name.ToString(), nameWithTitle.ToString()), army.Name.Attributes);
			GamePatchesHelper.ActivateAllGetterPatches();
			return modifiedName;
		}
	}
}
