using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer {
	internal class TitlesForCharacters : ITitlePropertiesContainer {

		readonly IDictionary<string, IndividualCharacterProperties> _characters;

		public TitleProperties Default { get; set; }
		public IReadOnlyDictionary<string, IndividualCharacterProperties> Characters {
			get => new ReadOnlyDictionary<string, IndividualCharacterProperties>(_characters);
		}

		[JsonConstructor]
		public TitlesForCharacters(IReadOnlyDictionary<string, IndividualCharacterProperties> characters, TitleProperties @default) {
			this._characters = characters != null ? characters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) : new Dictionary<string, IndividualCharacterProperties>();
			this.Default = @default;
		}

		internal TitlesForCharacters(TitlesForCharacters titlesForCharacters) {
			Default = titlesForCharacters.Default != null ? new TitleProperties(titlesForCharacters.Default) : null;
			_characters = titlesForCharacters._characters.Select(x =>
				new KeyValuePair<string, IndividualCharacterProperties>(x.Key, new IndividualCharacterProperties(x.Value))).ToDictionary(x => x.Key, x => x.Value);
		}

		private TitlesForCharacters() {
			Default = new TitleProperties();
			_characters = new Dictionary<string, IndividualCharacterProperties>();
		}

		internal void UpdateDueToHeroNameChange(string oldName, string newName) {
			if (WouldUpdateDueToHeroNameChange(oldName, newName)) {
				_characters[newName] = new IndividualCharacterProperties(_characters[oldName]);
			}
		}

		internal bool WouldUpdateDueToHeroNameChange(string oldName, string newName) {
			return _characters.ContainsKey(oldName) && !_characters.ContainsKey(newName);
		}

		internal void AddPropertiesForCharacter(string characterName, IndividualCharacterProperties properties) {
			_characters[characterName] = properties;
		}
		internal void DeletePropertiesForCharacter(string characterKey) {
			if (characterKey is null) {
				return;
			}
			_characters.Remove(characterKey);
		}

		public bool TryGetTitlePropertiesFor(Hero hero, out TitleProperties titleProperties) {
			if (_characters.ContainsKey(TitleConfiguration.GetFullHeroName(hero))) {
				_characters[TitleConfiguration.GetFullHeroName(hero)].TryGetTitlePropertiesFor(hero, out TitleProperties outProperties);
				if (Default != null) {
					outProperties = TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(outProperties, Default);
				}
				titleProperties = outProperties ?? new TitleProperties();
				return true; // always return true, because the character is defined and therefore properties should be applied even if no properties are given (may apply default) 
			}
			titleProperties = null;
			return false;
		}

		internal static TitlesForCharacters CreateEmpty() {
			return new TitlesForCharacters();
		}
	}
}
