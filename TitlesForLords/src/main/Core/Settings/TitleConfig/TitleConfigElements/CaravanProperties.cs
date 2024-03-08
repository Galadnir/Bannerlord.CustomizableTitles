using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.CampaignSystem;

namespace Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements {
	internal class CaravanProperties : IEquatable<CaravanProperties> {

		public string CaravanPrefixOnCampaignMap { get; set; }
		public string CaravanPostfixOnCampaignMap { get; set; }
		public bool? ShowNameOfOwner { get; set; }

		[JsonConstructor]
		public CaravanProperties(string caravanPrefixOnCampaignMap, string caravanPostfixOnCampaignMap, bool? showNameOfOwner) {
			this.CaravanPrefixOnCampaignMap = caravanPrefixOnCampaignMap;
			this.CaravanPostfixOnCampaignMap = caravanPostfixOnCampaignMap;
			this.ShowNameOfOwner = showNameOfOwner;
		}

		internal CaravanProperties() {
			CaravanPrefixOnCampaignMap = null;
			CaravanPostfixOnCampaignMap = null;
			ShowNameOfOwner = null;
		}

		internal CaravanProperties(CaravanProperties properties) : this(properties.CaravanPrefixOnCampaignMap, properties.CaravanPostfixOnCampaignMap, properties.ShowNameOfOwner) {
		}

		internal static CaravanProperties MergeLowerPriorityCaravanPropertiesIntoHigher(CaravanProperties higherPriorityProperties, CaravanProperties lowerPriorityProperties) {
			if (higherPriorityProperties == null && lowerPriorityProperties == null) {
				return null;
			}
			if (higherPriorityProperties == null) {
				return new CaravanProperties(lowerPriorityProperties);
			}
			if (lowerPriorityProperties == null) {
				return new CaravanProperties(higherPriorityProperties);
			}
			var prefix = higherPriorityProperties.CaravanPrefixOnCampaignMap ?? lowerPriorityProperties.CaravanPrefixOnCampaignMap;
			var postfix = higherPriorityProperties.CaravanPostfixOnCampaignMap ?? lowerPriorityProperties.CaravanPostfixOnCampaignMap;
			var showNameOfOwner = higherPriorityProperties.ShowNameOfOwner ?? lowerPriorityProperties.ShowNameOfOwner;
			return new CaravanProperties(prefix, postfix, showNameOfOwner);
		}

		public override bool Equals(object obj) {
			return this.Equals(obj as CaravanProperties);
		}

		public bool Equals(CaravanProperties other) {
			return !(other is null) &&
				   this.CaravanPrefixOnCampaignMap == other.CaravanPrefixOnCampaignMap &&
				   this.CaravanPostfixOnCampaignMap == other.CaravanPostfixOnCampaignMap &&
				   this.ShowNameOfOwner == other.ShowNameOfOwner;
		}

		public override int GetHashCode() {
			int hashCode = 173039166;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.CaravanPrefixOnCampaignMap);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.CaravanPostfixOnCampaignMap);
			hashCode = hashCode * -1521134295 + this.ShowNameOfOwner.GetHashCode();
			return hashCode;
		}

		internal string ApplyFor(Hero owner, string vanillaName) {
			if (CaravanPrefixOnCampaignMap is null && CaravanPostfixOnCampaignMap is null) {
				return vanillaName;
			}
			StringBuilder output = new StringBuilder(100);
			output.Append(CaravanPrefixOnCampaignMap?.TrimStart() ?? "");
			if (ShowNameOfOwner == true) {
				output.Append(owner.Name.ToString() ?? "");
			}
			output.Append(CaravanPostfixOnCampaignMap?.TrimEnd() ?? "");
			return output.ToString();
		}
	}
}
