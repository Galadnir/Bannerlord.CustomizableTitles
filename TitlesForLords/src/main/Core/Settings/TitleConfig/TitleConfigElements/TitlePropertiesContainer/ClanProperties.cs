using Newtonsoft.Json;
using System;
using TaleWorlds.CampaignSystem;

namespace Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer {
	internal class ClanProperties : ITitlePropertiesContainer {

		public RankMember ClanMemberDefault { get; set; }
		public RankMember ClanLeader { get; set; }

		[JsonConstructor]
		public ClanProperties(RankMember clanMemberDefault, RankMember clanLeader) {
			this.ClanMemberDefault = clanMemberDefault;
			this.ClanLeader = clanLeader;
		}

		internal ClanProperties(ClanProperties properties) {
			ClanMemberDefault = properties.ClanMemberDefault != null ? new RankMember(properties.ClanMemberDefault) : null;
			ClanLeader = properties.ClanLeader != null ? new RankMember(properties.ClanLeader) : null;
		}

		private ClanProperties() {
			ClanMemberDefault = RankMember.CreateEmpty();
			ClanLeader = RankMember.CreateEmpty();
		}

		public bool TryGetTitlePropertiesFor(Hero hero, out TitleProperties titleProperties) {
			TitleProperties memberProperties = null;
			TitleProperties leaderProperties = null;
			bool success = ClanMemberDefault?.TryGetTitlePropertiesFor(hero, out memberProperties) == true;
			if (hero.Equals(hero.Clan.Leader) && ClanLeader != null) {
				success |= ClanLeader.TryGetTitlePropertiesFor(hero, out leaderProperties);
			}
			titleProperties = TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(leaderProperties, memberProperties) ?? new TitleProperties();
			return success;
		}

		internal static ClanProperties CreateEmpty() {
			return new ClanProperties();
		}
	}
}
