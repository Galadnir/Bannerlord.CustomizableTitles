using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements;
using TaleWorlds.CampaignSystem;

namespace Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig {
	internal interface ITitlePropertiesContainer {

		bool TryGetTitlePropertiesFor(Hero hero, out TitleProperties titleProperties);
	}
}
