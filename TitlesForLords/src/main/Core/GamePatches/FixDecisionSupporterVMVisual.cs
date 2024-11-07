using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;

namespace Bannerlord.TitlesForLords.src.main.Core.GamePatches {


	[HarmonyPatch(typeof(DecisionSupporterVM))]
	[HarmonyPatch(MethodType.Constructor)]
	[HarmonyPatch(new Type[] { typeof(TextObject), typeof(string), typeof(Clan), typeof(Supporter.SupportWeights) })]
	public static class FixDecisionSupporterVMVisual {
		public static void Postfix(DecisionSupporterVM __instance, TextObject ____nameObj, ref Hero ____hero) {
			____hero = Hero.FindFirst((Hero H) => H.Name.ToString() == ____nameObj.ToString());
			if (____hero != null) {
				__instance.Visual = new ImageIdentifierVM(CampaignUIHelper.GetCharacterCode(____hero.CharacterObject));
			} else {
				__instance.Visual = new ImageIdentifierVM();
			}
		}
	}
}
