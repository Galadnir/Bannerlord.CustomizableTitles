using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;

namespace Bannerlord.TitlesForLords.src.main.Core.Settings.SettingsPatches {
	internal static class TitleConfigUpdaterHelper {

		const string NameOfConfigPostfixOnChange = " generated";

		internal static TitleConfiguration CopyAndChangeConfig(TitleConfiguration config) {
			TitleConfiguration copy = ModSettings.Instance.CopyExistingTitleConfig(config);
			if (!copy.Metadata.Name.EndsWith(NameOfConfigPostfixOnChange)) {
				copy.Metadata.Name += NameOfConfigPostfixOnChange;
			}
			config.Options.IsActive = false;
			return copy;
		}

		internal static bool CopyConfigOnChangeCondition(TitleConfiguration config) {
			return config.Options.IsDefault || ModSettings.Instance.CopyConfigOnAnyNameChange;
		}
	}
}