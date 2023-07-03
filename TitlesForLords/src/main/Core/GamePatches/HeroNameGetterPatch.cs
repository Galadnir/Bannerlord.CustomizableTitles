using Bannerlord.TitleOverhaul.src.main.Core.GamePatches;
using Bannerlord.TitlesForLords.src.main.Core.Settings;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace Bannerlord.TitlesForLords.src.main.Core.GamePatches {

	[HarmonyPatch(typeof(Hero))]
	[HarmonyPatch(nameof(Hero.Name), MethodType.Getter)]
	public static class HeroNameGetterPatch {

		public static bool isActive = true;

		static bool modifyNextCall = true;

		public static TextObject Postfix(TextObject name, Hero __instance) {
			if (!isActive || !modifyNextCall || !ShouldApplyToHero(__instance)) {
				modifyNextCall = true;
				return name;
			}
			foreach (var config in ModSettings.Instance.ActiveTitleConfigs) {
				if (config.TryApplyToHeroName(name, __instance, out TextObject modifiedName)) {
					return modifiedName;
				}
			}
			return name;
		}

		private static bool ShouldApplyToHero(Hero hero) {
			return GamePatchesHelper.ShouldApplyToHero(hero);
		}

		public static TextObject GetUnmodifiedName(Hero hero) {
			modifyNextCall = false;
			return hero.Name;
		}
	}
}
