using Bannerlord.TitleOverhaul.src.main.Core.GamePatches;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements;
using Bannerlord.TitlesForLords.src.main.Core.GamePatches;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig.TitleConfigElements;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig {
	internal class TitleConfiguration : IEquatable<TitleConfiguration> {

		readonly Metadata _metadata;
		readonly ConfigOptions _options;
		internal static readonly string GetFullHeroNameOutputIfNameIsNull = " ";

		public Metadata Metadata {
			get => _metadata;
		} // to avoid readonly property due to JsonSerializer
		public ConfigOptions Options {
			get => _options;
		}
		public LordTitles TitlesForLords { get; set; }
		public NotableOwnedCaravans NotableCaravans { get; set; }

		static TitleProperties GlobalDefault {
			get => ModSettings.Instance.GlobalDefault ?? new TitleProperties();
		}

		[JsonConstructor]
		public TitleConfiguration(Metadata metadata, ConfigOptions options, LordTitles titlesForLords, NotableOwnedCaravans notableCaravans) {
			this._metadata = metadata;
			this._options = options;
			this.TitlesForLords = titlesForLords;
			this.NotableCaravans = notableCaravans;
		}

		public TitleConfiguration(TitleConfiguration config, string newUid, bool isDefault = false) {
			_metadata = new Metadata(config.Metadata, newUid);
			_options = new ConfigOptions(config.Options, isDefault);
			TitlesForLords = config.TitlesForLords != null ? new LordTitles(config.TitlesForLords) : null;
			NotableCaravans = config.NotableCaravans != null ? new NotableOwnedCaravans(config.NotableCaravans) : null;
		}

		private TitleConfiguration(string uid, bool isDefault) {
			_metadata = new Metadata(uid, "", "", new List<Tuple<string, string>>());
			_options = new ConfigOptions(isDefault, false);
			TitlesForLords = LordTitles.CreateEmpty();
			NotableCaravans = NotableOwnedCaravans.CreateEmpty();
		}

		internal void UpdateDueToHeroNameChange(string oldName, string newName) {
			if (WouldUpdateDueToHeroNameChange(oldName, newName)) {
				TitlesForLords.TitlesForCharacters.UpdateDueToHeroNameChange(oldName, newName);
				Metadata.AddRename(oldName, newName);
			}
		}

		internal bool WouldUpdateDueToHeroNameChange(string oldName, string newName) {
			return TitlesForLords?.TitlesForCharacters?.WouldUpdateDueToHeroNameChange(oldName, newName) == true;
		}

		internal void UpdateDueToClanNameChange(string oldName, Clan clan) {
			if (!WouldUpdateDueToClanNameChange(oldName, clan)) {
				return;
			}
			TitlesForLords.TitlesForClans?.UpdateDueToClanNameChange(oldName, clan.Name.ToString());
			foreach (Hero hero in clan.Heroes ?? new MBReadOnlyList<Hero>(new List<Hero>())) {
				TitlesForLords.TitlesForCharacters?.UpdateDueToHeroNameChange(GetFullHeroName(hero).Replace(clan.Name.ToString(), oldName), GetFullHeroName(hero));
			}
			Metadata.AddRename(oldName, clan.Name.ToString());
		}

		internal bool WouldUpdateDueToClanNameChange(string oldClanName, Clan clan) {
			if (TitlesForLords?.TitlesForClans?.WouldUpdateDueToClanNameChange(oldClanName, clan.Name.ToString()) == true) {
				return true;
			}
			foreach (Hero hero in clan.Heroes ?? new MBReadOnlyList<Hero>(new List<Hero>())) {
				if (TitlesForLords?.TitlesForCharacters?.WouldUpdateDueToHeroNameChange(GetFullHeroName(hero).Replace(clan.Name.ToString(), oldClanName),
					GetFullHeroName(hero)) == true) {
					return true;
				}
			}
			return false;
		}

		internal void UpdateDueToKingdomNameChange(string oldName, string newName) {
			if (WouldUpdateDueToKingdomNameChange(oldName, newName)) {
				TitlesForLords.TitlesForKingdoms.UpdateDueToKingdomNameChange(oldName, newName);
				Metadata.AddRename(oldName, newName);
			}
		}

		internal bool WouldUpdateDueToKingdomNameChange(string oldName, string newName) {
			return TitlesForLords?.TitlesForKingdoms?.WouldUpdateDueToKingdomNameChange(oldName, newName) == true;
		}

		internal bool TryApplyToHeroName(TextObject vanillaName, Hero hero, out TextObject newName) {
			if (HasClan(hero) && TitlesForLords?.TryGetTitlePropertiesFor(hero, out TitleProperties properties) == true) {
				var completeProperties = TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(properties, GlobalDefault);
				newName = new TextObject(completeProperties.ApplyToHeroName(hero, vanillaName.ToString()), vanillaName.Attributes);
				return true;
			}
			newName = vanillaName;
			return false;
		}

		internal bool TryApplyOnCampaignMap(MobileParty party, out TextObject newName) {
			if (party.IsVillager) {
				return TryApplyToVillagerParty(party, out newName);
			} else if (party.IsCaravan) {
				return TryApplyToCaravan(party, out newName);
			} else if (HasClan(party.LeaderHero)) {
				return TryApplyToLordParty(party, out newName);
			}
			newName = PartyNameGetterPatch.GetUnmodifiedName(party);
			return false;
		}

		internal bool TryApplyToArmy(Army army, out TextObject newName) {
			Hero leader = army.LeaderParty?.LeaderHero;
			if (HasClan(leader) && TitlesForLords?.TryGetTitlePropertiesFor(leader, out TitleProperties properties) == true) {
				var completeProperties = TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(properties, GlobalDefault);
				newName = new TextObject(completeProperties.ApplyToArmy(army));
				return true;
			}
			newName = ArmyNameGetterPatch.GetUnmodifiedNameWithTitleAdded(army);
			return false;
		}

		private bool TryApplyToLordParty(MobileParty party, out TextObject newName) {
			if (TitlesForLords?.TryGetTitlePropertiesFor(party.LeaderHero, out TitleProperties properties) == true) {
				var completeProperties = TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(properties, GlobalDefault);
				newName = new TextObject(completeProperties.ApplyToLordParty(party),
					PartyNameGetterPatch.GetUnmodifiedName(party).Attributes);
				return true;
			}
			newName = PartyNameGetterPatch.GetUnmodifiedName(party);
			return false;
		}

		private bool TryApplyToVillagerParty(MobileParty party, out TextObject newName) {
			if (HasClan(party.Owner) && TitlesForLords?.TryGetTitlePropertiesFor(party.Owner, out TitleProperties properties) == true) {
				var completeProperties = TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(properties, GlobalDefault);
				newName = new TextObject(completeProperties.ApplyToVillagerParty(party.HomeSettlement, party.Owner, PartyNameGetterPatch.GetUnmodifiedName(party).ToString()), PartyNameGetterPatch.GetUnmodifiedName(party).Attributes);
				return true;
			}
			newName = PartyNameGetterPatch.GetUnmodifiedName(party);
			return false;
		}

		private bool TryApplyToCaravan(MobileParty party, out TextObject newName) {
			if (party.Owner?.IsNotable == true && NotableCaravans?.TryGetCaravanProperties(party.Owner, out CaravanProperties properties) == true) {
				var completePropertes = CaravanProperties.MergeLowerPriorityCaravanPropertiesIntoHigher(properties, GlobalDefault.OwnedCaravans);
				newName = new TextObject(completePropertes.ApplyFor(party.Owner, PartyNameGetterPatch.GetUnmodifiedName(party).ToString()),
					PartyNameGetterPatch.GetUnmodifiedName(party).Attributes);
				return true;
			}
			if (HasClan(party.Owner) && TitlesForLords?.TryGetTitlePropertiesFor(party.Owner, out TitleProperties titleProperties) == true) {
				var completeProperties = TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(titleProperties, GlobalDefault);
				newName = new TextObject(completeProperties.ApplyToCaravan(party.Owner, PartyNameGetterPatch.GetUnmodifiedName(party).ToString()), PartyNameGetterPatch.GetUnmodifiedName(party).Attributes);
				return true;
			}
			newName = PartyNameGetterPatch.GetUnmodifiedName(party);
			return false;
		}

		private bool HasClan(Hero hero) {
			return hero?.Clan != null;
		}

		public override bool Equals(object obj) {
			return this.Equals(obj as TitleConfiguration);
		}

		public bool Equals(TitleConfiguration other) {
			return !(other is null) &&
				   EqualityComparer<Metadata>.Default.Equals(this.Metadata, other.Metadata);
		}

		public override int GetHashCode() {
			return 1353091168 + EqualityComparer<Metadata>.Default.GetHashCode(this.Metadata);
		}

		public static bool operator ==(TitleConfiguration left, TitleConfiguration right) {
			return EqualityComparer<TitleConfiguration>.Default.Equals(left, right);
		}

		public static bool operator !=(TitleConfiguration left, TitleConfiguration right) {
			return !(left == right);
		}

		internal static string GetFullHeroName(Hero hero) {
			return $"{HeroNameGetterPatch.GetUnmodifiedName(hero)} {hero.Clan?.Name}";
		}

		internal static TitleConfiguration CreateEmpty(string uid, bool isDefault) {
			return new TitleConfiguration(uid, isDefault);
		}
	}
}
