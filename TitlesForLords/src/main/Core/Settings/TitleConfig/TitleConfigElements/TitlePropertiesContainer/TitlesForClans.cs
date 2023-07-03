using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer {
	internal class TitlesForClans : ITitlePropertiesContainer {

		readonly IDictionary<string, TitlesForTContainer<ClanProperties>> _clans; // the properties in TitlesForTContainer store kingdom dependent properties

		public ClanProperties Default { get; set; }
		public IReadOnlyDictionary<string, TitlesForTContainer<ClanProperties>> Clans {
			get => new ReadOnlyDictionary<string, TitlesForTContainer<ClanProperties>>(_clans);
		}

		[JsonConstructor]
		public TitlesForClans(IReadOnlyDictionary<string, TitlesForTContainer<ClanProperties>> clans, ClanProperties @default) {
			this._clans = clans != null ? clans.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) : new Dictionary<string, TitlesForTContainer<ClanProperties>>();
			this.Default = @default;
		}

		internal TitlesForClans(TitlesForClans titlesForClans) {
			Default = titlesForClans.Default != null ? new ClanProperties(titlesForClans.Default) : null;
			_clans = titlesForClans._clans.Select(
				x => new KeyValuePair<string, TitlesForTContainer<ClanProperties>>(x.Key,
				new TitlesForTContainer<ClanProperties>(x.Value, properties => new ClanProperties(properties))))
				.ToDictionary(x => x.Key, x => x.Value);
		}

		private TitlesForClans() {
			Default = ClanProperties.CreateEmpty();
			_clans = new Dictionary<string, TitlesForTContainer<ClanProperties>>();
		}

		internal void UpdateDueToClanNameChange(string oldName, string newName) {
			if (WouldUpdateDueToClanNameChange(oldName, newName)) {
				_clans[newName] = new TitlesForTContainer<ClanProperties>(_clans[oldName], clanProperties => new ClanProperties(clanProperties));
			}
		}

		internal bool WouldUpdateDueToClanNameChange(string oldName, string newName) {
			return _clans.ContainsKey(oldName) && !_clans.ContainsKey(newName);
		}

		internal void AddPropertiesForClan(string clanName, TitlesForTContainer<ClanProperties> properties) {
			_clans[clanName] = properties;
		}

		internal void DeletePropertiesForClan(string oldClanKey) {
			if (oldClanKey is null) {
				return;
			}
			_clans.Remove(oldClanKey);
		}

		public bool TryGetTitlePropertiesFor(Hero hero, out TitleProperties titleProperties) {
			if (_clans.ContainsKey(hero.Clan.Name.ToString())) {
				_clans[hero.Clan.Name.ToString()].TryGetTitlePropertiesFor(hero, (Hero lord) => lord.Clan.Kingdom?.Name.ToString(), out TitleProperties outProperties, true);
				if (Default?.TryGetTitlePropertiesFor(hero, out TitleProperties defaultProperties) == true) {
					outProperties = TitleProperties.MergeLowerPriorityTitlePropertiesIntoHigher(outProperties, defaultProperties);
				}
				titleProperties = outProperties ?? new TitleProperties();
				return true;
			}
			titleProperties = null;
			return false;
		}

		internal static TitlesForClans CreateEmpty() {
			return new TitlesForClans();
		}
	}
}
