using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer;
using Newtonsoft.Json;
using TaleWorlds.CampaignSystem;

namespace Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer {
	internal class LordTitles : ITitlePropertiesContainer {

		public RankMember Default { get; set; }
		public TitlesForKingdoms TitlesForKingdoms { get; set; }
		public TitlesForClans TitlesForClans { get; set; }
		public TitlesForCharacters TitlesForCharacters { get; set; }

		[JsonConstructor]
		public LordTitles(RankMember @default, TitlesForKingdoms titlesForKingdoms, TitlesForClans titlesForClans, TitlesForCharacters titlesForCharacters) {
			this.Default = @default;
			this.TitlesForKingdoms = titlesForKingdoms;
			this.TitlesForClans = titlesForClans;
			this.TitlesForCharacters = titlesForCharacters;
		}

		internal LordTitles(LordTitles lordTitles) {
			Default = lordTitles.Default != null ? new RankMember(lordTitles.Default) : null;
			TitlesForKingdoms = lordTitles.TitlesForKingdoms != null ? new TitlesForKingdoms(lordTitles.TitlesForKingdoms) : null;
			TitlesForClans = lordTitles.TitlesForClans != null ? new TitlesForClans(lordTitles.TitlesForClans) : null;
			TitlesForCharacters = lordTitles.TitlesForCharacters != null ? new TitlesForCharacters(lordTitles.TitlesForCharacters) : null;
		}

		private LordTitles() {
			Default = RankMember.CreateEmpty();
			TitlesForKingdoms = TitlesForKingdoms.CreateEmpty();
			TitlesForClans = TitlesForClans.CreateEmpty();
			TitlesForCharacters = TitlesForCharacters.CreateEmpty();
		}

		public bool TryGetTitlePropertiesFor(Hero hero, out TitleProperties titleProperties) {
			TitleProperties characterProperties = null;
			TitleProperties clanProperties = null;
			bool success = TitlesForCharacters?.TryGetTitlePropertiesFor(hero, out characterProperties) == true;
			success |= TitlesForClans?.TryGetTitlePropertiesFor(hero, out clanProperties) == true;
			TitleProperties outProperties = null;
			if (success) {
				outProperties = TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(characterProperties, clanProperties);
			}
			if (hero.Clan.Kingdom != null && TitlesForKingdoms?.TryGetTitlePropertiesFor(hero, out TitleProperties kingdomProperties) == true) {
				success = true;
				outProperties = TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(outProperties, kingdomProperties);

			}
			if (success && Default?.TryGetTitlePropertiesFor(hero, out TitleProperties defaultProperties) == true) {
				outProperties = TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(outProperties, defaultProperties);
			}
			titleProperties = outProperties ?? new TitleProperties();
			return success;
		}

		internal static LordTitles CreateEmpty() {
			return new LordTitles();
		}
	}
}
