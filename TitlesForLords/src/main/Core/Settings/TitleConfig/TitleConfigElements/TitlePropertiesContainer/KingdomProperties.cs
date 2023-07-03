using Bannerlord.TitlesForLords.src.main.Core.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.CampaignSystem;


namespace Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer {
	internal class KingdomProperties : ITitlePropertiesContainer {

		readonly IDictionary<int, ClanProperties> _clanTiers;

		public ClanProperties Default { get; set; }
		public RankMember RulerProperties { get; set; }
		public RankMember SpouseOfRulerProperties { get; set; }
		public RankMember ChildOfRulerProperties { get; set; }
		public ClanProperties RulingClan { get; set; }
		public IReadOnlyDictionary<int, ClanProperties> ClanTiers {
			get => new ReadOnlyDictionary<int, ClanProperties>(_clanTiers);
		}

		[JsonConstructor]
		public KingdomProperties(IReadOnlyDictionary<int, ClanProperties> clanTiers, ClanProperties @default, RankMember rulerProperties, RankMember spouseOfRulerProperties, RankMember childOfRulerProperties, ClanProperties rulingClan) {
			this._clanTiers = clanTiers != null ? clanTiers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) : new Dictionary<int, ClanProperties>(); // receives IReadOnlyDictionary so JsonSerializer can find it
			this.Default = @default;
			this.RulerProperties = rulerProperties;
			this.SpouseOfRulerProperties = spouseOfRulerProperties;
			this.ChildOfRulerProperties = childOfRulerProperties;
			this.RulingClan = rulingClan;
		}

		internal KingdomProperties(KingdomProperties properties) {
			Default = properties.Default != null ? new ClanProperties(properties.Default) : null;
			RulerProperties = properties.RulerProperties != null ? new RankMember(properties.RulerProperties) : null;
			SpouseOfRulerProperties = properties.SpouseOfRulerProperties != null ? new RankMember(properties.SpouseOfRulerProperties) : null;
			ChildOfRulerProperties = properties.ChildOfRulerProperties != null ? new RankMember(properties.ChildOfRulerProperties) : null;
			RulingClan = properties.RulingClan != null ? new ClanProperties(properties.RulingClan) : null;
			_clanTiers = properties._clanTiers
				.Select(keyValuePair => new KeyValuePair<int, ClanProperties>(keyValuePair.Key, new ClanProperties(keyValuePair.Value)))
				.ToDictionary(x => x.Key, x => x.Value);
		}

		internal void DeleteTierProperties(int oldTier) {
			_clanTiers.Remove(oldTier);
		}
		internal void AddTierProperties(int tier, ClanProperties kingdomProperties) {
			_clanTiers[tier] = kingdomProperties;
		}

		private KingdomProperties() {
			Default = ClanProperties.CreateEmpty();
			RulerProperties = RankMember.CreateEmpty();
			SpouseOfRulerProperties = RankMember.CreateEmpty();
			ChildOfRulerProperties = RankMember.CreateEmpty();
			RulingClan = ClanProperties.CreateEmpty();
			_clanTiers = new Dictionary<int, ClanProperties>();
		}

		public bool TryGetTitlePropertiesFor(Hero hero, out TitleProperties titleProperties) {
			TitleProperties outProperties = null;
			bool success = false;
			Hero ruler = hero.Clan.Kingdom.RulingClan?.Leader;
			IDictionary<string, RulingClanPossibility> deadSpecialRulingClanMembers = new Dictionary<string, RulingClanPossibility>();
			if (!(Campaign.Current?.UniqueGameId is null) && ModSettings.Instance.DeadSpecialRulingClanMembersPerCampaign.ContainsKey(Campaign.Current.UniqueGameId)) {
				deadSpecialRulingClanMembers = ModSettings.Instance.DeadSpecialRulingClanMembersPerCampaign[Campaign.Current.UniqueGameId];
			}
			bool keyContained = deadSpecialRulingClanMembers.ContainsKey(hero.Id.ToString());
			if (hero.Equals(ruler) || (keyContained && deadSpecialRulingClanMembers[hero.Id.ToString()] == RulingClanPossibility.Ruler)) {
				success |= RulerProperties?.TryGetTitlePropertiesFor(hero, out outProperties) == true;

			} else if (hero.Equals(ruler.Spouse) || (keyContained  && deadSpecialRulingClanMembers[hero.Id.ToString()] == RulingClanPossibility.SpouseOfRuler)) {
				success |= SpouseOfRulerProperties?.TryGetTitlePropertiesFor(hero, out outProperties) == true;
			} else if (ruler?.Children?.Contains(hero) == true || (keyContained && deadSpecialRulingClanMembers[hero.Id.ToString()] == RulingClanPossibility.ChildOfRuler)) {
				success |= ChildOfRulerProperties?.TryGetTitlePropertiesFor(hero, out outProperties) == true;
			}
			if ((hero.Clan.Equals(hero.Clan.Kingdom.RulingClan) || keyContained) && RulingClan?.TryGetTitlePropertiesFor(hero, out TitleProperties rulingClanProperties) == true) {
				success = true;
				outProperties = TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(outProperties, rulingClanProperties);
			}
			if (_clanTiers.ContainsKey(hero.Clan.Tier)) {
				success = true;
				_clanTiers[hero.Clan.Tier].TryGetTitlePropertiesFor(hero, out TitleProperties clanTierProperties);
				outProperties = TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(outProperties, clanTierProperties);
			}
			if (Default?.TryGetTitlePropertiesFor(hero, out TitleProperties defaultProperties) == true) {
				success = true;
				outProperties = TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(outProperties, defaultProperties);
			}
			titleProperties = outProperties ?? new TitleProperties();
			return success;
		}

		internal static KingdomProperties CreateEmpty() {
			return new KingdomProperties();
		}
	}
}
