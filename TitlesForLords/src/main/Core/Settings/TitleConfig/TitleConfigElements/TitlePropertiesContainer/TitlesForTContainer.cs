using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer {
	internal class TitlesForTContainer<T> where T : ITitlePropertiesContainer {

		readonly IDictionary<string, T> _properties;

		public T Default { get; set; }
		public IReadOnlyDictionary<string, T> Properties {
			get => new ReadOnlyDictionary<string, T>(_properties);
		}

		[JsonConstructor]
		public TitlesForTContainer(IReadOnlyDictionary<string, T> properties, T @default) {
			this._properties = properties != null ? properties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) : new Dictionary<string, T>();
			this.Default = @default;
		}

		internal TitlesForTContainer(TitlesForTContainer<T> container, Func<T, T> CopyT) {
			Default = container.Default?.Equals(default(T)) == false ? CopyT(container.Default) : default;
			_properties = container._properties.ToDictionary(kvp => kvp.Key, kvp => CopyT(kvp.Value));
		}

		private TitlesForTContainer() {
			Default = default;
			_properties = new Dictionary<string, T>();
		}

		internal void AddProperty(string key, T value) {
			_properties[key] = value;
		}

		internal void DeleteProperty(string key) {
			if (key is null) {
				return;
			}
			_properties.Remove(key);
		}

		public bool TryGetTitlePropertiesFor(Hero hero, Func<Hero, string> GetPropertyToCompareTo, out TitleProperties titleProperties, bool returnTrueOnOnlyDefault) {
			bool success = false;
			TitleProperties outProperties = null;
			if (!(GetPropertyToCompareTo(hero) is null) && _properties.ContainsKey(GetPropertyToCompareTo(hero))) {
				success = true;
				_properties[GetPropertyToCompareTo(hero)].TryGetTitlePropertiesFor(hero, out outProperties);
			}
			if (Default?.TryGetTitlePropertiesFor(hero, out TitleProperties defaultProperties) == true) {
				success |= returnTrueOnOnlyDefault;
				outProperties = TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(outProperties, defaultProperties);
			}
			titleProperties = outProperties ?? new TitleProperties();
			return success;
		}

		internal static TitlesForTContainer<T> CreateEmptyWithoutDefault() {
			return new TitlesForTContainer<T>();
		}

		internal static TitlesForTContainer<T> CreateEmptyWithDefault(Func<T> createEmptyT) {
			var container = new TitlesForTContainer<T> {
				Default = createEmptyT()
			};
			return container;
		}
	}
}