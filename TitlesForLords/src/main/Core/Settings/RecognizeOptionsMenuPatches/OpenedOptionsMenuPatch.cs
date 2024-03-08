using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace Bannerlord.TitlesForLords.src.main.Core.Settings.RecognizeOptionsMenuPatches {

	[HarmonyPatch]
	internal static class OpenedOptionsMenuPatch {

		public static void Postfix() {
			TitlesForLordsSubModule.IsOptionsMenuOpen = true;
		}

		[HarmonyTargetMethods]
		internal static IEnumerable<MethodBase> Targets() {
			return AccessTools.GetDeclaredConstructors(typeof(OptionsVM))
				.Cast<MethodBase>();
		}
	}
}
