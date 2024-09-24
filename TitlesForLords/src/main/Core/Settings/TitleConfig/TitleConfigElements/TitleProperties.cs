using Bannerlord.TitleOverhaul.src.main.Core.GamePatches;
using Bannerlord.TitlesForLords.src.main.Core.GamePatches;
using Bannerlord.TitlesForLords.src.main.Helper;
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
		public string NamePrefixToRemove { get; set; }
		public string NamePostfixToRemove { get; set; }
		public string ChildTitleBeforeName { get; set; }
		public string ChildTitleAfterName { get; set; }
		public string PartyPrefixOnCampaignMap { get; set; }
		public string PartyPostfixOnCampaignMap { get; set; }
		public string ArmyPrefixOnCampaignMap { get; set; }
		public string ArmyPostfixOnCampaignMap { get; set; }
		public bool? OnlyApplyToClanLeader { get; set; }
		public bool? ApplyToMercenaries { get; set; }
		public VillagerProperties OwnedVillagers { get; set; }
		public CaravanProperties OwnedCaravans { get; set; }

		[JsonConstructor]
		public TitleProperties(string titleBeforeName, string titleAfterName, string namePrefixToRemove, string namePostfixToRemove, string childTitleBeforeName, string childTitleAfterName, string partyPrefixOnCampaignMap, string partyPostfixOnCampaignMap, string armyPrefixOnCampaignMap, string armyPostfixOnCampaignMap, bool? onlyApplyToClanLeader, bool? applyToMercenaries, VillagerProperties ownedVillagers, CaravanProperties ownedCaravans) {
			TitleBeforeName = titleBeforeName;
			TitleAfterName = titleAfterName;
			NamePrefixToRemove = namePrefixToRemove;
			NamePostfixToRemove = namePostfixToRemove;
			ChildTitleBeforeName = childTitleBeforeName;
			ChildTitleAfterName = childTitleAfterName;
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
			this(properties.TitleBeforeName, properties.TitleAfterName, properties.NamePrefixToRemove, properties.NamePostfixToRemove, properties.ChildTitleBeforeName, properties.ChildTitleAfterName, properties.PartyPrefixOnCampaignMap, properties.PartyPostfixOnCampaignMap, properties.ArmyPrefixOnCampaignMap, properties.ArmyPostfixOnCampaignMap,
				 properties.OnlyApplyToClanLeader, properties.ApplyToMercenaries, properties.OwnedVillagers != null ? new VillagerProperties(properties.OwnedVillagers) : null, properties.OwnedCaravans != null ? new CaravanProperties(properties.OwnedCaravans) : null) { }

		internal TitleProperties() {
			TitleBeforeName = null;
			TitleAfterName = null;
			NamePrefixToRemove = null;
			NamePostfixToRemove = null;
			ChildTitleBeforeName = null;
			ChildTitleAfterName = null;
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
			var namePrefixToRemove = higherPriorityProperties.NamePrefixToRemove ?? lowerPriorityProperties.NamePrefixToRemove;
			var namePostfixToRemove = higherPriorityProperties.NamePostfixToRemove ?? lowerPriorityProperties.NamePostfixToRemove;
			var childTitleBeforeName = higherPriorityProperties.ChildTitleBeforeName ?? lowerPriorityProperties.ChildTitleBeforeName;
			var childTitleAfterName = higherPriorityProperties.ChildTitleAfterName ?? lowerPriorityProperties.ChildTitleAfterName;
			var partyPrefixOnCampaignMap = higherPriorityProperties.PartyPrefixOnCampaignMap ?? lowerPriorityProperties.PartyPrefixOnCampaignMap;
			var partyPostfixOnCampaignMap = higherPriorityProperties.PartyPostfixOnCampaignMap ?? lowerPriorityProperties.PartyPostfixOnCampaignMap;
			var armyPrefixOnCampaignMap = higherPriorityProperties.ArmyPrefixOnCampaignMap ?? lowerPriorityProperties.ArmyPrefixOnCampaignMap;
			var armyPostfixOnCampaignMap = higherPriorityProperties.ArmyPostfixOnCampaignMap ?? lowerPriorityProperties.ArmyPostfixOnCampaignMap;
			var onlyApplyToClanLeader = higherPriorityProperties.OnlyApplyToClanLeader ?? lowerPriorityProperties.OnlyApplyToClanLeader;
			var applyToMercenaries = higherPriorityProperties.ApplyToMercenaries ?? lowerPriorityProperties.ApplyToMercenaries;
			var ownedVillagers = VillagerProperties.MergeLowerPriorityVillagerPropertiesIntoHigher(higherPriorityProperties.OwnedVillagers, lowerPriorityProperties.OwnedVillagers);
			var ownedCaravans = CaravanProperties.MergeLowerPriorityCaravanPropertiesIntoHigher(higherPriorityProperties.OwnedCaravans, lowerPriorityProperties.OwnedCaravans);
			return new TitleProperties(titleBeforeName, titleAfterName, namePrefixToRemove, namePostfixToRemove, childTitleBeforeName, childTitleAfterName, partyPrefixOnCampaignMap, partyPostfixOnCampaignMap, armyPrefixOnCampaignMap, armyPostfixOnCampaignMap, onlyApplyToClanLeader, applyToMercenaries, ownedVillagers, ownedCaravans);
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
			if (ShouldApplyPropertiesTo(hero)) {
				if (!hero.IsChild && (!(TitleBeforeName is null) || !(TitleAfterName is null))) {
					return ApplyTitleToLord(hero);
				}
				if (!(ChildTitleBeforeName is null) || !(ChildTitleAfterName is null)) {
					return ApplyTitleToChildOfLord(hero);
				}
			}
			return vanillaName;
		}

		private string ApplyTitleToLord(Hero hero) {
			return $"{TitleBeforeName?.TrimStart() ?? ""}{HeroNameGetterPatch.GetUnmodifiedName(hero).ToString().RemovePrefix(NamePrefixToRemove).RemovePostfix(NamePostfixToRemove)}{TitleAfterName?.TrimEnd() ?? ""}";
		}

		private string ApplyTitleToChildOfLord(Hero hero) {
			return $"{ChildTitleBeforeName?.TrimStart() ?? ""}{HeroNameGetterPatch.GetUnmodifiedName(hero).ToString().RemovePrefix(NamePrefixToRemove).RemovePostfix(NamePostfixToRemove)}{ChildTitleAfterName?.TrimEnd() ?? ""}";
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
			if (heroParty.LeaderHero.IsChild) {
				output.Append(ApplyTitleToChildOfLord(heroParty.LeaderHero));
			} else {
				output.Append(ApplyTitleToLord(heroParty.LeaderHero));
			}
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
			if (army.LeaderParty.LeaderHero.IsChild) {
				output.Append(ApplyTitleToChildOfLord(army.LeaderParty.LeaderHero));
			} else {
				output.Append(ApplyTitleToLord(army.LeaderParty.LeaderHero));
			}
			output.Append(ArmyPostfixOnCampaignMap?.TrimEnd() ?? "");
			return output.ToString();
		}

		public bool Equals(TitleProperties other) {
			return this.Equals(other as object);
		}

		public override bool Equals(object obj) {
			return obj is TitleProperties properties &&
				   this.TitleBeforeName == properties.TitleBeforeName &&
				   this.TitleAfterName == properties.TitleAfterName &&
				   this.NamePrefixToRemove == properties.NamePrefixToRemove &&
				   this.NamePostfixToRemove == properties.NamePostfixToRemove &&
				   this.ChildTitleBeforeName == properties.ChildTitleBeforeName &&
				   this.ChildTitleAfterName == properties.ChildTitleAfterName &&
				   this.PartyPrefixOnCampaignMap == properties.PartyPrefixOnCampaignMap &&
				   this.PartyPostfixOnCampaignMap == properties.PartyPostfixOnCampaignMap &&
				   this.ArmyPrefixOnCampaignMap == properties.ArmyPrefixOnCampaignMap &&
				   this.ArmyPostfixOnCampaignMap == properties.ArmyPostfixOnCampaignMap &&
				   this.OnlyApplyToClanLeader == properties.OnlyApplyToClanLeader &&
				   this.ApplyToMercenaries == properties.ApplyToMercenaries &&
				   EqualityComparer<VillagerProperties>.Default.Equals(this.OwnedVillagers, properties.OwnedVillagers) &&
				   EqualityComparer<CaravanProperties>.Default.Equals(this.OwnedCaravans, properties.OwnedCaravans);
		}

		public override int GetHashCode() {
			var hash = new HashCode();
			hash.Add(this.TitleBeforeName);
			hash.Add(this.TitleAfterName);
			hash.Add(this.NamePrefixToRemove);
			hash.Add(this.NamePostfixToRemove);
			hash.Add(this.ChildTitleBeforeName);
			hash.Add(this.ChildTitleAfterName);
			hash.Add(this.PartyPrefixOnCampaignMap);
			hash.Add(this.PartyPostfixOnCampaignMap);
			hash.Add(this.ArmyPrefixOnCampaignMap);
			hash.Add(this.ArmyPostfixOnCampaignMap);
			hash.Add(this.OnlyApplyToClanLeader);
			hash.Add(this.ApplyToMercenaries);
			hash.Add(this.OwnedVillagers);
			hash.Add(this.OwnedCaravans);
			return hash.ToHashCode();
		}
	}
}