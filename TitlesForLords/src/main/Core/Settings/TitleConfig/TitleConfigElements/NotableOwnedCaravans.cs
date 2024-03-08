using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements;
using Bannerlord.TitlesForLords.src.main.Core.GamePatches;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig.TitleConfigElements {
	internal class NotableOwnedCaravans {

		readonly IDictionary<string, CaravanProperties> _notableProperties;
		readonly IDictionary<string, CaravanProperties> _kingdomProperties;
		readonly IDictionary<string, CaravanProperties> _cultureProperties;

		public CaravanProperties Default { get; set; }
		public IReadOnlyDictionary<string, CaravanProperties> NotableProperties {
			get => new ReadOnlyDictionary<string, CaravanProperties>(_notableProperties);
		}

		public IReadOnlyDictionary<string, CaravanProperties> KingdomProperties {
			get => new ReadOnlyDictionary<string, CaravanProperties>(_kingdomProperties);
		}

		public IReadOnlyDictionary<string, CaravanProperties> CultureProperties {
			get => new ReadOnlyDictionary<string, CaravanProperties>(_cultureProperties);
		}

		[JsonConstructor]
		public NotableOwnedCaravans(IReadOnlyDictionary<string, CaravanProperties> notableProperties, IReadOnlyDictionary<string, CaravanProperties> kingdomProperties,
			IReadOnlyDictionary<string, CaravanProperties> cultureProperties, CaravanProperties @default) {
			this._notableProperties = notableProperties != null ? notableProperties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) : new Dictionary<string, CaravanProperties>();
			this._kingdomProperties = kingdomProperties != null ? kingdomProperties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) : new Dictionary<string, CaravanProperties>();
			this._cultureProperties = cultureProperties != null ? cultureProperties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) : new Dictionary<string, CaravanProperties>();
			this.Default = @default;
		}

		internal NotableOwnedCaravans(NotableOwnedCaravans notableCaravans) {
			Default = notableCaravans.Default != null ? new CaravanProperties(notableCaravans.Default) : null;
			_notableProperties = notableCaravans._notableProperties
				.Select(keyValuePair => new KeyValuePair<string, CaravanProperties>(keyValuePair.Key, new CaravanProperties(keyValuePair.Value)))
				.ToDictionary(x => x.Key, x => x.Value);
			_kingdomProperties = notableCaravans._kingdomProperties
				.Select(keyValuePair => new KeyValuePair<string, CaravanProperties>(keyValuePair.Key, new CaravanProperties(keyValuePair.Value)))
				.ToDictionary(x => x.Key, x => x.Value);
			_cultureProperties = notableCaravans._cultureProperties
				.Select(keyValuePair => new KeyValuePair<string, CaravanProperties>(keyValuePair.Key, new CaravanProperties(keyValuePair.Value)))
				.ToDictionary(x => x.Key, x => x.Value);
		}

		private NotableOwnedCaravans() {
			Default = new CaravanProperties();
			_notableProperties = new Dictionary<string, CaravanProperties>();
			_kingdomProperties = new Dictionary<string, CaravanProperties>();
			_cultureProperties = new Dictionary<string, CaravanProperties>();
		}


		internal void DeletePropertiesForSpecificNotable(string notableKey) {
			if (notableKey is null) {
				return;
			}
			_notableProperties.Remove(notableKey);
		}

		internal void AddPropertiesForNotable(string notableName, CaravanProperties properties) {
			_notableProperties[notableName] = properties;
		}

		internal void DeletePropertiesForKingdom(string kingdomKey) {
			if (kingdomKey is null) {
				return;
			}
			_kingdomProperties.Remove(kingdomKey);
		}

		internal void AddPropertiesForKingdom(string kingdomKey, CaravanProperties caravanProperties) {
			_kingdomProperties[kingdomKey] = caravanProperties;
		}

		internal void DeletePropertiesForCulture(string cultureKey) {
			if (cultureKey is null) {
				return;
			}
			_cultureProperties.Remove(cultureKey);
		}

		internal void AddPropertiesForCulture(string cultureKey, CaravanProperties caravanProperties) {
			_cultureProperties[cultureKey] = caravanProperties;
		}

		internal bool TryGetCaravanProperties(Hero notable, out CaravanProperties properties) {
			bool success = false;
			CaravanProperties outProperties = null;
			if (!(notable.MapFaction?.Culture is null) && _cultureProperties.ContainsKey(notable.MapFaction.Culture.Name.ToString())) {
				success = true;
				outProperties = _cultureProperties[notable.MapFaction.Culture.Name.ToString()];
			}
			if (!(notable.MapFaction is null) && _kingdomProperties.ContainsKey(notable.MapFaction.Name.ToString())) {
				success = true;
				CaravanProperties kingdomProperties = _kingdomProperties[notable.MapFaction.Name.ToString()];
				outProperties = CaravanProperties.MergeLowerPriorityCaravanPropertiesIntoHigher(kingdomProperties, outProperties);
			}
			if (_notableProperties.ContainsKey(HeroNameGetterPatch.GetUnmodifiedName(notable).ToString())) {
				success = true;
				CaravanProperties notableProperties = _notableProperties[HeroNameGetterPatch.GetUnmodifiedName(notable).ToString()];
				outProperties = CaravanProperties.MergeLowerPriorityCaravanPropertiesIntoHigher(notableProperties, outProperties);
			}
			if (success && Default != null) {
				outProperties = CaravanProperties.MergeLowerPriorityCaravanPropertiesIntoHigher(outProperties, Default);
			}
			properties = outProperties ?? new CaravanProperties();
			return success;
		}

		internal static NotableOwnedCaravans CreateEmpty() {
			return new NotableOwnedCaravans();
		}
	}
}
