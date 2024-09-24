using Bannerlord.TitlesForLords.src.main.Core.GamePatches;
using Bannerlord.TitlesForLords.src.main.Core.Settings;
using TaleWorlds.CampaignSystem;

namespace Bannerlord.TitleOverhaul.src.main.Core.GamePatches {
	public static class GamePatchesHelper {

		public static void DisableAllGetterPatches() {
			ArmyNameGetterPatch.isActive = false;
			ConversationNameLabelPatch.isActive = false;
			HeroNameGetterPatch.isActive = false;
			PartyNameGetterPatch.isActive = false;
		}

		public static void ActivateAllGetterPatches() {
			ArmyNameGetterPatch.isActive = true;
			ConversationNameLabelPatch.isActive = true;
			HeroNameGetterPatch.isActive = true;
			PartyNameGetterPatch.isActive = true;
		}

		internal static bool ShouldApplyToHero(Hero hero) {
			if (!ModSettings.Instance.ApplyTitleConfigToPlayer && hero.IsHumanPlayerCharacter) {
				return false;
			}
			if (!ModSettings.Instance.ApplyTitleConfigToPlayerCompanions && hero.IsPlayerCompanion) {
				return false;
			}
			return true;
		}
	}
}
