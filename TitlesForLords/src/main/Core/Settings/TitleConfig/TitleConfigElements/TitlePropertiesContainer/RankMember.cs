using Newtonsoft.Json;
using TaleWorlds.CampaignSystem;

namespace Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer {
	internal class RankMember : ITitlePropertiesContainer {

		public TitleProperties Default { get; set; }
		public TitleProperties Male { get; set; }
		public TitleProperties Female { get; set; }

		[JsonConstructor]
		public RankMember(TitleProperties @default, TitleProperties male, TitleProperties female) {
			this.Default = @default;
			this.Male = male;
			this.Female = female;
		}

		internal RankMember(RankMember rankMember) {
			Default = rankMember.Default != null ? new TitleProperties(rankMember.Default) : null;
			Male = rankMember.Male != null ? new TitleProperties(rankMember.Male) : null;
			Female = rankMember.Female != null ? new TitleProperties(rankMember.Female) : null;
		}

		private RankMember() {
			Default = new TitleProperties();
			Male = new TitleProperties();
			Female = new TitleProperties();
		}

		public bool TryGetTitlePropertiesFor(Hero hero, out TitleProperties titleProperties) {
			bool success = Default != null;
			if (hero.IsFemale) {
				success |= Female != null;
				titleProperties = TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(Female, Default);
			} else {
				success |= Male != null;
				titleProperties = TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(Male, Default);
			}
			return success;
		}

		internal static RankMember CreateEmpty() {
			return new RankMember();
		}
	}
}
