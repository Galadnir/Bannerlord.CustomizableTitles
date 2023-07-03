using Bannerlord.TitleOverhaul.src.main.Core.GamePatches;
using Bannerlord.TitlesForLords.src.main.Core.Settings;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;

namespace Bannerlord.TitlesForLords.src.main.Core.GamePatches {

	[HarmonyPatch(typeof(MobileParty))]
	[HarmonyPatch(nameof(MobileParty.Name), MethodType.Getter)]
	public static class PartyNameGetterPatch {

		public static bool isActive = true;

		static bool modifyNextCall = true;

		public static TextObject Postfix(TextObject name, MobileParty __instance) {
			if (!isActive || !modifyNextCall || !CheckPartyConditions(__instance)) {
				modifyNextCall = true;
				return name;
			}
			foreach (var config in ModSettings.Instance.ActiveTitleConfigs) {
				if (config.TryApplyOnCampaignMap(__instance, out TextObject modifiedName)) {
					return modifiedName;
				}
			}
			return name;
		}

		private static bool CheckPartyConditions(MobileParty party) {
			if (!ModSettings.Instance.ApplyToPlayerCaravans && party.IsCaravan && (party.Owner?.IsHumanPlayerCharacter == true)) {
				return false;
			}
			if (!(party.IsVillager || party.IsCaravan) && (party.LeaderHero is null || !CheckLeaderConditions(party.LeaderHero))) {
				return false;
			}
			if ((party.IsVillager || party.IsCaravan) && (party.Owner is null || !CheckLeaderConditions(party.Owner))) {
				return false;
			}
			return true;
		}

		private static bool CheckLeaderConditions(Hero leader) {
			return GamePatchesHelper.ShouldApplyToHero(leader);
		}

		public static TextObject GetUnmodifiedName(MobileParty party) {
			modifyNextCall = false;
			return party.Name;
		}
	}
}
