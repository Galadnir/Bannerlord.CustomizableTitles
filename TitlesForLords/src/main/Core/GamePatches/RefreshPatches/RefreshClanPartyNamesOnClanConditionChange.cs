using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.Library;

namespace Bannerlord.TitleOverhaul.src.main.Core.GamePatches.RefreshPatches {

	[HarmonyPatch]
	internal static class RefreshClanPartyNamesOnClanConditionChange {

		public static void Postfix(Clan __instance) {
			foreach (PartyComponent party in __instance.WarPartyComponents ?? new MBReadOnlyList<WarPartyComponent>(new List<WarPartyComponent>())) {
				party.ClearCachedName();
			}
		}

		[HarmonyTargetMethods]
		internal static IEnumerable<MethodBase> Targets() {
			yield return AccessTools.DeclaredPropertySetter(typeof(Clan), nameof(Clan.Kingdom));
			yield return AccessTools.DeclaredPropertySetter(typeof(Clan), nameof(Clan.Tier));
		}
	}
}
