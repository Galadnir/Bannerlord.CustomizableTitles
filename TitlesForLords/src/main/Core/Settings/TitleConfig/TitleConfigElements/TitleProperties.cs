using Bannerlord.TitleOverhaul.src.main.Core.GamePatches;
using Bannerlord.TitlesForLords.src.main.Core.GamePatches;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements {
	internal class TitleProperties : ITitlePropertiesContainer, IEquatable<TitleProperties> {

		const bool ApplyToMercenariesDefault = false;
		const bool OnlyApplyToClanLeaderDefault = false;

		public string TitleBeforeName { get; set; }
		public string TitleAfterName { get; set; }
		public string PartyPrefixOnCampaignMap { get; set; }
		public string PartyPostfixOnCampaignMap { get; set; }
		public string ArmyPrefixOnCampaignMap { get; set; }
		public string ArmyPostfixOnCampaignMap { get; set; }
		public bool? OnlyApplyToClanLeader { get; set; }
		public bool? ApplyToMercenaries { get; set; }
		public VillagerProperties OwnedVillagers { get; set; }
		public CaravanProperties OwnedCaravans { get; set; }

		[JsonConstructor]
		public TitleProperties(string titleBeforeName, string titleAfterName, string partyPrefixOnCampaignMap, string partyPostfixOnCampaignMap,
								 string armyPrefixOnCampaignMap, string armyPostfixOnCampaignMap, bool? onlyApplyToClanLeader, bool? applyToMercenaries, VillagerProperties ownedVillagers, CaravanProperties ownedCaravans) {
			TitleBeforeName = titleBeforeName;
			TitleAfterName = titleAfterName;
			PartyPrefixOnCampaignMap = partyPrefixOnCampaignMap;
			PartyPostfixOnCampaignMap = partyPostfixOnCampaignMap;
			ArmyPrefixOnCampaignMap = armyPrefixOnCampaignMap;
			ArmyPostfixOnCampaignMap = armyPostfixOnCampaignMap;
			OnlyApplyToClanLeader = onlyApplyToClanLeader;
			ApplyToMercenaries = applyToMercenaries;
			OwnedVillagers = ownedVillagers;
			OwnedCaravans = ownedCaravans;
		}

		internal TitleProperties(TitleProperties properties) :
			this(properties.TitleBeforeName, properties.TitleAfterName, properties.PartyPrefixOnCampaignMap,
				 properties.PartyPostfixOnCampaignMap, properties.ArmyPrefixOnCampaignMap, properties.ArmyPostfixOnCampaignMap,
				 properties.OnlyApplyToClanLeader, properties.ApplyToMercenaries, properties.OwnedVillagers != null ? new VillagerProperties(properties.OwnedVillagers) : null, properties.OwnedCaravans != null ? new CaravanProperties(properties.OwnedCaravans) : null) {
		}

		internal TitleProperties() {
			TitleBeforeName = null;
			TitleAfterName = null;
			PartyPrefixOnCampaignMap = null;
			PartyPostfixOnCampaignMap = null;
			ArmyPrefixOnCampaignMap = null;
			ArmyPostfixOnCampaignMap = null;
			OnlyApplyToClanLeader = null;
			ApplyToMercenaries = null;
			OwnedVillagers = new VillagerProperties();
			OwnedCaravans = new CaravanProperties();
		}

		internal static TitleProperties MergeLowerPriorityTitlePropertiesIntoHigher(TitleProperties higherPriorityProperties, TitleProperties lowerPriorityProperties) {
			if (higherPriorityProperties == null && lowerPriorityProperties == null) {
				return null;
			}
			if (lowerPriorityProperties == null) {
				return new TitleProperties(higherPriorityProperties);
			}
			if (higherPriorityProperties == null) {
				return new TitleProperties(lowerPriorityProperties);
			}
			var titleBeforeName = higherPriorityProperties.TitleBeforeName ?? lowerPriorityProperties.TitleBeforeName;
			var titleAfterName = higherPriorityProperties.TitleAfterName ?? lowerPriorityProperties.TitleAfterName;
			var partyPrefixOnCampaignMap = higherPriorityProperties.PartyPrefixOnCampaignMap ?? lowerPriorityProperties.PartyPrefixOnCampaignMap;
			var partyPostfixOnCampaignMap = higherPriorityProperties.PartyPostfixOnCampaignMap ?? lowerPriorityProperties.PartyPostfixOnCampaignMap;
			var armyPrefixOnCampaignMap = higherPriorityProperties.ArmyPrefixOnCampaignMap ?? lowerPriorityProperties.ArmyPrefixOnCampaignMap;
			var armyPostfixOnCampaignMap = higherPriorityProperties.ArmyPostfixOnCampaignMap ?? lowerPriorityProperties.ArmyPostfixOnCampaignMap;
			var onlyApplyToClanLeader = higherPriorityProperties.OnlyApplyToClanLeader ?? lowerPriorityProperties.OnlyApplyToClanLeader;
			var applyToMercenaries = higherPriorityProperties.ApplyToMercenaries ?? lowerPriorityProperties.ApplyToMercenaries;
			var ownedVillagers = VillagerProperties.MergeLowerPriorityVillagerPropertiesIntoHigher(higherPriorityProperties.OwnedVillagers, lowerPriorityProperties.OwnedVillagers);
			var ownedCaravans = CaravanProperties.MergeLowerPriorityCaravanPropertiesIntoHigher(higherPriorityProperties.OwnedCaravans, lowerPriorityProperties.OwnedCaravans);
			return new TitleProperties(titleBeforeName, titleAfterName, partyPrefixOnCampaignMap, partyPostfixOnCampaignMap, armyPrefixOnCampaignMap, armyPostfixOnCampaignMap, onlyApplyToClanLeader, applyToMercenaries, ownedVillagers, ownedCaravans);
		}

		private bool ShouldApplyPropertiesTo(Hero hero) {
			if ((OnlyApplyToClanLeader ?? OnlyApplyToClanLeaderDefault) && !hero.Equals(hero.Clan.Leader)) {
				return false;
			} // e.g. lake rats can be employed as mercenaries, but IsClanTypeMercenary is not true, therefore both checks. This allows titles for clan while they are not employed and are therefore bandits.
			if (!(ApplyToMercenaries ?? ApplyToMercenariesDefault) && (hero.Clan?.IsUnderMercenaryService == true || hero.Clan?.IsClanTypeMercenary == true)) {
				return false;
			}
			return true;
		}

		public bool TryGetTitlePropertiesFor(Hero hero, out TitleProperties titleProperties) {
			titleProperties = this;
			return true;
		}

		internal string ApplyToHeroName(Hero hero, string vanillaName) {
			if (ShouldApplyPropertiesTo(hero) && (!(TitleBeforeName is null) || !(TitleAfterName is null))) {
				return ApplyTitleToLord(hero);
			}
			return vanillaName;
		}

		private string ApplyTitleToLord(Hero hero) {
			return $"{TitleBeforeName?.TrimStart() ?? ""}{HeroNameGetterPatch.GetUnmodifiedName(hero)}{TitleAfterName?.TrimEnd() ?? ""}";
		}

		internal string ApplyToLordParty(MobileParty heroParty) {
			if (ShouldApplyPropertiesTo(heroParty.LeaderHero)) {
				return ApplyTitlePropertiesToLordParty(heroParty);
			}
			return PartyNameGetterPatch.GetUnmodifiedName(heroParty).ToString();
		}

		private string ApplyTitlePropertiesToLordParty(MobileParty heroParty) {
			if (PartyPrefixOnCampaignMap is null && PartyPostfixOnCampaignMap is null) {
				return PartyNameGetterPatch.GetUnmodifiedName(heroParty).ToString();
			}
			StringBuilder output = new StringBuilder(100);
			output.Append(PartyPrefixOnCampaignMap?.TrimStart() ?? "");
			output.Append(ApplyTitleToLord(heroParty.LeaderHero));
			output.Append(PartyPostfixOnCampaignMap?.TrimEnd() ?? "");
			return output.ToString();
		}

		internal string ApplyToVillagerParty(Settlement homeSettlement, Hero owner, string vanillaName) {
			if (!(OwnedVillagers is null) && ShouldApplyPropertiesTo(owner)) {
				return OwnedVillagers.Apply(homeSettlement, owner, vanillaName);
			}
			return vanillaName;
		}

		internal string ApplyToCaravan(Hero owner, string vanillaName) {
			if (!(OwnedCaravans is null) && ShouldApplyPropertiesTo(owner)) {
				return OwnedCaravans.ApplyFor(owner, vanillaName);
			}
			return vanillaName;
		}

		internal string ApplyToArmy(Army army) {
			if (ArmyPrefixOnCampaignMap is null && ArmyPostfixOnCampaignMap is null) {
				return ArmyNameGetterPatch.GetUnmodifiedNameWithTitleAdded(army).ToString();
			}
			StringBuilder output = new StringBuilder(100);
			output.Append(ArmyPrefixOnCampaignMap?.TrimStart() ?? "");
			output.Append(ApplyTitleToLord(army.LeaderParty.LeaderHero));
			output.Append(ArmyPostfixOnCampaignMap?.TrimEnd() ?? "");
			return output.ToString();
		}

		public override bool Equals(object obj) {
			return this.Equals(obj as TitleProperties);
		}

		public bool Equals(TitleProperties other) {
			return !(other is null) &&
				   this.TitleBeforeName == other.TitleBeforeName &&
				   this.TitleAfterName == other.TitleAfterName &&
				   this.PartyPrefixOnCampaignMap == other.PartyPrefixOnCampaignMap &&
				   this.PartyPostfixOnCampaignMap == other.PartyPostfixOnCampaignMap &&
				   this.ArmyPrefixOnCampaignMap == other.ArmyPrefixOnCampaignMap &&
				   this.ArmyPostfixOnCampaignMap == other.ArmyPostfixOnCampaignMap &&
				   this.OnlyApplyToClanLeader == other.OnlyApplyToClanLeader &&
				   this.ApplyToMercenaries == other.ApplyToMercenaries &&
				   EqualityComparer<VillagerProperties>.Default.Equals(this.OwnedVillagers, other.OwnedVillagers) &&
				   EqualityComparer<CaravanProperties>.Default.Equals(this.OwnedCaravans, other.OwnedCaravans);
		}

		public override int GetHashCode() {
			int hashCode = -965217632;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.TitleBeforeName);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.TitleAfterName);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.PartyPrefixOnCampaignMap);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.PartyPostfixOnCampaignMap);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.ArmyPrefixOnCampaignMap);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.ArmyPostfixOnCampaignMap);
			hashCode = hashCode * -1521134295 + this.OnlyApplyToClanLeader.GetHashCode();
			hashCode = hashCode * -1521134295 + this.ApplyToMercenaries.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<VillagerProperties>.Default.GetHashCode(this.OwnedVillagers);
			hashCode = hashCode * -1521134295 + EqualityComparer<CaravanProperties>.Default.GetHashCode(this.OwnedCaravans);
			return hashCode;
		}
	}
}