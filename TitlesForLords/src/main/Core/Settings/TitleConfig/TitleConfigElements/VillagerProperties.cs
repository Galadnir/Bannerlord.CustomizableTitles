using Bannerlord.TitlesForLords.src.main.Core.GamePatches;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;

namespace Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements {
	internal class VillagerProperties : IEquatable<VillagerProperties> {

		private const string DefaultInfix = " ";

		public string VillagerPrefixOnCampaignMap { get; set; }
		public string VillagerInfixOnCampaignMap { get; set; }
		public string VillagerPostfixOnCampaignMap { get; set; }
		public bool? ShowHomeSettlementOfVillagers { get; set; }
		public bool? ShowOwnerOfVillagers { get; set; }
		public bool? ShowHomeSettlementBeforeOwner { get; set; }

		[JsonConstructor]
		public VillagerProperties(string villagerPrefixOnCampaignMap, string villagerInfixOnCampaignMap, string villagerPostfixOnCampaignMap, bool? showHomeSettlementOfVillagers, bool? showOwnerOfVillagers, bool? showHomeSettlementBeforeOwner) {
			this.VillagerPrefixOnCampaignMap = villagerPrefixOnCampaignMap;
			this.VillagerInfixOnCampaignMap = villagerInfixOnCampaignMap;
			this.VillagerPostfixOnCampaignMap = villagerPostfixOnCampaignMap;
			this.ShowHomeSettlementOfVillagers = showHomeSettlementOfVillagers;
			this.ShowOwnerOfVillagers = showOwnerOfVillagers;
			this.ShowHomeSettlementBeforeOwner = showHomeSettlementBeforeOwner;
		}

		internal VillagerProperties(VillagerProperties properties) : this(properties.VillagerPrefixOnCampaignMap, properties.VillagerInfixOnCampaignMap, properties.VillagerPostfixOnCampaignMap, properties.ShowHomeSettlementOfVillagers, properties.ShowOwnerOfVillagers, properties.ShowHomeSettlementBeforeOwner) {
		}

		internal VillagerProperties() : this(null, null, null, null, null, null) { }

		internal static VillagerProperties MergeLowerPriorityVillagerPropertiesIntoHigher(VillagerProperties higherPriorityProperties, VillagerProperties lowerPriorityProperties) {
			if (higherPriorityProperties == null && lowerPriorityProperties == null) {
				return null;
			}
			if (higherPriorityProperties == null) {
				return new VillagerProperties(lowerPriorityProperties);
			}
			if (lowerPriorityProperties == null) {
				return new VillagerProperties(higherPriorityProperties);
			}
			var villagerPrefixOnCampaignMap = higherPriorityProperties.VillagerPrefixOnCampaignMap ?? lowerPriorityProperties.VillagerPrefixOnCampaignMap;
			var villagerInfixOnCampaignMap = higherPriorityProperties.VillagerInfixOnCampaignMap ?? lowerPriorityProperties.VillagerInfixOnCampaignMap;
			var villagerPostfixOnCampaignMap = higherPriorityProperties.VillagerPostfixOnCampaignMap ?? lowerPriorityProperties.VillagerPostfixOnCampaignMap;
			var showHomeSettlementOfVillagers = higherPriorityProperties.ShowHomeSettlementOfVillagers ?? lowerPriorityProperties.ShowHomeSettlementOfVillagers;
			var showOwnerOfVillagers = higherPriorityProperties.ShowOwnerOfVillagers ?? lowerPriorityProperties.ShowOwnerOfVillagers;
			var showHomeSettlementBeforeOwner = higherPriorityProperties.ShowHomeSettlementBeforeOwner ?? lowerPriorityProperties.ShowHomeSettlementBeforeOwner;
			return new VillagerProperties(villagerPrefixOnCampaignMap, villagerInfixOnCampaignMap, villagerPostfixOnCampaignMap, showHomeSettlementOfVillagers, showOwnerOfVillagers, showHomeSettlementBeforeOwner);
		}

		public override bool Equals(object obj) {
			return this.Equals(obj as VillagerProperties);
		}

		public bool Equals(VillagerProperties other) {
			return !(other is null) &&
				   this.VillagerPrefixOnCampaignMap == other.VillagerPrefixOnCampaignMap &&
				   this.VillagerInfixOnCampaignMap == other.VillagerInfixOnCampaignMap &&
				   this.VillagerPostfixOnCampaignMap == other.VillagerPostfixOnCampaignMap &&
				   this.ShowHomeSettlementOfVillagers == other.ShowHomeSettlementOfVillagers &&
				   this.ShowOwnerOfVillagers == other.ShowOwnerOfVillagers &&
				   this.ShowHomeSettlementBeforeOwner == other.ShowHomeSettlementBeforeOwner;
		}

		public override int GetHashCode() {
			int hashCode = 432650594;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.VillagerPrefixOnCampaignMap);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.VillagerInfixOnCampaignMap);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.VillagerPostfixOnCampaignMap);
			hashCode = hashCode * -1521134295 + this.ShowHomeSettlementOfVillagers.GetHashCode();
			hashCode = hashCode * -1521134295 + this.ShowOwnerOfVillagers.GetHashCode();
			hashCode = hashCode * -1521134295 + this.ShowHomeSettlementBeforeOwner.GetHashCode();
			return hashCode;
		}

		internal string Apply(Settlement homeSettlement, Hero owner, string vanillaName) {
			if (VillagerPrefixOnCampaignMap is null &&
				VillagerPostfixOnCampaignMap is null) {
				return vanillaName;
			}
			StringBuilder output = new StringBuilder(100);
			output.Append(VillagerPrefixOnCampaignMap?.TrimStart() ?? "");

			if (ShowOwnerOfVillagers == true && ShowHomeSettlementOfVillagers == true) {
				if (ShowHomeSettlementBeforeOwner == true) {
					output.Append(homeSettlement?.Name.ToString() ?? "");
					output.Append(VillagerInfixOnCampaignMap ?? DefaultInfix);
					output.Append(owner.Name.ToString());
				} else {
					output.Append(owner.Name.ToString());
					output.Append(VillagerInfixOnCampaignMap ?? DefaultInfix);
					output.Append(homeSettlement?.Name.ToString() ?? "");
				}
			} else {
				if (ShowOwnerOfVillagers == true) {
					output.Append(owner.Name.ToString());
				}
				if (ShowHomeSettlementOfVillagers == true) {
					output.Append(homeSettlement?.Name.ToString() ?? "");
				}
			}
			output.Append(VillagerPostfixOnCampaignMap?.TrimEnd() ?? "");
			return output.ToString();
		}
	}
}
