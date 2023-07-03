using HarmonyLib;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.Library;

namespace Bannerlord.TitleOverhaul.src.main.Core.GamePatches.RefreshPatches {

	[HarmonyPatch(typeof(Kingdom))]
	[HarmonyPatch(nameof(Kingdom.Name), MethodType.Setter)]
	internal static class RefreshPartyNamesOnKingdomNameChange {

		public static void Postfix(Kingdom __instance) {
			foreach (var party in __instance.WarPartyComponents ?? new MBReadOnlyList<WarPartyComponent>(new List<WarPartyComponent>())) {
				party.ClearCachedName();
			}
		}
	}
}
