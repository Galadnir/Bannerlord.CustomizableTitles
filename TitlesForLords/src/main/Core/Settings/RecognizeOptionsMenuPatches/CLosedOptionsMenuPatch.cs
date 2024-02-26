using HarmonyLib;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace Bannerlord.TitlesForLords.src.main.Core.Settings.RecognizeOptionsMenuPatches {

	[HarmonyPatch(typeof(OptionsVM))]
	[HarmonyPatch("CloseScreen")]
	internal static class ClosedOptionsMenuPatch {

		public static void Postfix() {
			TitlesForLordsSubModule.IsOptionsMenuOpen = false;
		}
	}
}
