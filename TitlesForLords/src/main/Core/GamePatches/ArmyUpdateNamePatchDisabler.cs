﻿using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace Bannerlord.TitleOverhaul.src.main.Core.GamePatches {

	[HarmonyPatch(typeof(Army))]
	[HarmonyPatch(nameof(Army.UpdateName))]
	internal static class ArmyUpdateNamePatchDisabler {

		public static void Prefix() {
			GamePatchesHelper.DisableAllGetterPatches();
		}

		public static void Postfix() {
			GamePatchesHelper.ActivateAllGetterPatches();
		}

	}
}
