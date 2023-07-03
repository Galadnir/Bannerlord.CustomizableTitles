using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Bannerlord.TitlesForLords.src.main.Core.Settings {

	public enum ModVersion { v1 }
	public enum RulingClanPossibility { Ruler, SpouseOfRuler, ChildOfRuler, Member}
	internal sealed class ModSettings {

		internal static readonly string SavefileLocation = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/Documents/Mount and Blade II Bannerlord/Configs/ModSettings/Global/CustomizableTitles/CustomizableTitlesSettings.json";
		internal static readonly string ConfigJsonsBasePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/../../../";
		internal const string ConfigJsonName = "CustomizableTitlesModConfig.json";
		internal static readonly string SuccessfullyLoadedConfigJsonName = $"LoadedSuccessfully_{ConfigJsonName}";
		internal static readonly string FailedToLoadConfigJsonName = $"{ConfigJsonName}_failed_to_parse.reason.txt";

		static internal ModSettings Instance = new ModSettings();

		internal const ModVersion CurrentVersion = ModVersion.v1;

		ModVersion _loadedVersion;

		List<TitleConfiguration> _titleConfigs; // list for priority, the lower the index the higher the priority as it gets checked earlier
		Dictionary<string, HashSet<string>> _subModuleToCultures;
		Dictionary<string, HashSet<string>> _subModuleToKingdoms;
		Dictionary<string, IDictionary<string, RulingClanPossibility>> _deadSpecialRulingClanMembersPerCampaign;

		internal IList<TitleConfiguration> TitleConfigs {
			get => _titleConfigs;
		}
		internal IList<TitleConfiguration> ActiveTitleConfigs =>
			_titleConfigs.Where(current => current.Options.IsActive).ToList();

		internal IDictionary<string, HashSet<string>> SubModuleToCultures {
			get => _subModuleToCultures;
		}

		internal IDictionary<string, HashSet<string>> SubModuleToKingdoms {
			get => _subModuleToKingdoms;
		}

		internal IDictionary<string, IDictionary<string, RulingClanPossibility>> DeadSpecialRulingClanMembersPerCampaign {
			get => _deadSpecialRulingClanMembersPerCampaign;
		}

		internal bool TrackAllNameChanges { get; set; } // all meaning hero name, clan name, kingdom name
		internal bool CopyConfigOnAnyNameChange { get; set; }
		internal bool UpdateAllConfigsOnAnyNameChange { get; set; }
		internal bool ApplyTitleConfigToPlayer { get; set; }
		internal bool ApplyTitleConfigToPlayerCompanions { get; set; }
		internal bool ApplyToPlayerCaravans { get; set; }

		internal TitleProperties GlobalDefault { get; set; }

		ModSettings() {
			if (IsFirstLoad()) {
				LoadDefault();
			} else {
				LoadFromSavefile();
			}
		}

		private void LoadFromSavefile() {
			using (StreamReader sr = File.OpenText(SavefileLocation)) {
				var loadedSettings = new JsonSerializer().Deserialize<ModSettingsSerializable>(new JsonTextReader(sr));
				_titleConfigs = loadedSettings.TitleConfigs ?? new List<TitleConfiguration>();
				_loadedVersion = loadedSettings.LoadedVersion;
				_subModuleToCultures = loadedSettings.SubModuleToCultures;
				_subModuleToKingdoms = loadedSettings.SubModuleToKingdoms;
				_deadSpecialRulingClanMembersPerCampaign = loadedSettings.DeadSpecialRulingClanMembersPerCampaign ?? new Dictionary<string, IDictionary<string, RulingClanPossibility>>(); // TODO: remove coalesce only there because added to exisiting save for now
				TrackAllNameChanges = loadedSettings.TrackAllNameChanges;
				CopyConfigOnAnyNameChange = loadedSettings.CopyConfigOnAnyNameChange;
				UpdateAllConfigsOnAnyNameChange = loadedSettings.UpdateAllConfigsOnAnyNameChange;
				ApplyTitleConfigToPlayer = loadedSettings.ApplyTitleConfigToPlayer;
				ApplyTitleConfigToPlayerCompanions = loadedSettings.ApplyTitleConfigToPlayerCompanions;
				ApplyToPlayerCaravans = loadedSettings.ApplyToPlayerCaravans;
				GlobalDefault = loadedSettings.GlobalDefault;
			}
		}

		private void LoadDefault() {
			_titleConfigs = ModSettingsDefault.TitleConfigs;
			_loadedVersion = ModSettingsDefault.Version;
			_subModuleToCultures = ModSettingsDefault.SubModuleToCultures;
			_subModuleToKingdoms = ModSettingsDefault.SubModuleToKingdoms;
			_deadSpecialRulingClanMembersPerCampaign = new Dictionary<string, IDictionary<string, RulingClanPossibility>>();
			TrackAllNameChanges = ModSettingsDefault.TrackNameAndClanChanges;
			CopyConfigOnAnyNameChange = ModSettingsDefault.CopyConfigOnNameOrClanChange;
			UpdateAllConfigsOnAnyNameChange = ModSettingsDefault.UpdateAllConfigsOnNameOrClanChange;
			ApplyTitleConfigToPlayer = ModSettingsDefault.ApplyTitleConfigToPlayer;
			ApplyTitleConfigToPlayerCompanions = ModSettingsDefault.ApplyTitleConfigToPlayerCompanions;
			ApplyToPlayerCaravans = ModSettingsDefault.ApplyToPlayerCaravans;
			GlobalDefault = ModSettingsDefault.GlobalDefault;
		}

		private bool IsFirstLoad() {
			return !File.Exists(SavefileLocation);
		}

		internal IList<string> LoadAndSaveNewJsonConfigFiles() {
			var loadedConfigNames = new List<string>();
			foreach (string configJson in Directory.GetFiles(ConfigJsonsBasePath, ConfigJsonName, SearchOption.AllDirectories)) {
				var loadedSuccessfully = true;
				StreamReader sr = File.OpenText(configJson);
				try {
					var jsonConfigFile = new JsonSerializer().Deserialize<JsonConfigFile>(new JsonTextReader(sr));
					if (!(jsonConfigFile.TitleConfig is null)) {
						int indexOfExistingConfig = _titleConfigs.IndexOf(jsonConfigFile.TitleConfig);
						if (indexOfExistingConfig > -1) {
							jsonConfigFile.TitleConfig.Options.IsActive = _titleConfigs[indexOfExistingConfig].Options.IsActive; // keep existing
							_titleConfigs.RemoveAt(indexOfExistingConfig);
							_titleConfigs.Insert(indexOfExistingConfig, jsonConfigFile.TitleConfig);
						} else {
							_titleConfigs.Insert(0, jsonConfigFile.TitleConfig);
						}
						loadedConfigNames.Add(jsonConfigFile.TitleConfig.Metadata.Name);
					}
					if (!(jsonConfigFile.Cultures is null) && !jsonConfigFile.Cultures.IsEmpty()) {
						_subModuleToCultures[jsonConfigFile.SubModule] = jsonConfigFile.Cultures;
					}
					if (!(jsonConfigFile.Kingdoms is null) && !jsonConfigFile.Kingdoms.IsEmpty()) {
						_subModuleToKingdoms[jsonConfigFile.SubModule] = jsonConfigFile.Kingdoms;
					}
				} catch (Exception e) when (e is UnauthorizedAccessException || e is NotSupportedException || e is JsonException) {
					loadedSuccessfully = false;
					string errorMessagePath = configJson.Replace(ConfigJsonName, FailedToLoadConfigJsonName);
					File.Delete(errorMessagePath);
					File.WriteAllText(errorMessagePath, e.Message);
				} finally {
					sr.Dispose();
					if (loadedSuccessfully) { // in this if and not in catch because otherwise file still in use
						string newPath = configJson.Replace(ConfigJsonName, SuccessfullyLoadedConfigJsonName);
						File.Delete(newPath); // Move throws an exception if the file already exists, delete doesn't
						File.Move(configJson, newPath);
					}
				}
			}
			Save();
			return loadedConfigNames;
		}

		internal void Save() {
			FileInfo saveFile = new FileInfo(SavefileLocation);
			saveFile.Directory.Create();
			File.WriteAllText(SavefileLocation, JsonConvert.SerializeObject(new ModSettingsSerializable(), new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore,
			}));
		}

		internal void Restore() {
			LoadFromSavefile();
		}

		internal TitleConfiguration CopyExistingTitleConfig(TitleConfiguration config) {
			var copy = new TitleConfiguration(config, Guid.NewGuid().ToString());
			_titleConfigs.Insert(_titleConfigs.IndexOf(config) + 1, copy); // transfer priority
			return copy;
		}

		internal void DeleteConfig(TitleConfiguration config) {
			if (config?.Options.IsDefault != false) {
				return;
			}
			_titleConfigs.Remove(config);
		}

		internal void IncreasePriority(TitleConfiguration config) {
			int previousIndex = _titleConfigs.IndexOf(config);
			if (config is null || previousIndex <= 0) {
				return;
			}
			_titleConfigs.RemoveAt(previousIndex);
			_titleConfigs.Insert(previousIndex - 1, config);
		}

		internal void DecreasePriority(TitleConfiguration config) {
			int previousIndex = _titleConfigs.IndexOf(config);
			if (config is null || previousIndex == -1 || previousIndex == _titleConfigs.Count - 1) {
				return;
			}
			_titleConfigs.RemoveAt(previousIndex);
			_titleConfigs.Insert(previousIndex + 1, config);
		}

		internal void CreateNewConfig(string uid, string name, bool isDefault = false) {
			var newConfig = TitleConfiguration.CreateEmpty(uid, isDefault);
			newConfig.Metadata.Name = name;
			_titleConfigs.Add(newConfig);
		}

		internal void RulingClanMemberDied(string campaignId, Hero victim, RulingClanPossibility rulerRelationship) {
			if (!_deadSpecialRulingClanMembersPerCampaign.ContainsKey(campaignId)) {
				_deadSpecialRulingClanMembersPerCampaign[campaignId] = new Dictionary<string,  RulingClanPossibility>();
			}
			if (victim?.Id is null) {
				return;
			}
			_deadSpecialRulingClanMembersPerCampaign[campaignId][victim.Id.ToString()] = rulerRelationship;
		}

		private class ModSettingsSerializable {

			public ModVersion LoadedVersion { get; set; }
			public List<TitleConfiguration> TitleConfigs { get; set; }
			public Dictionary<string, HashSet<string>> SubModuleToCultures { get; set; }
			public Dictionary<string, HashSet<string>> SubModuleToKingdoms { get; set; }
			public Dictionary<string, IDictionary<string, RulingClanPossibility>> DeadSpecialRulingClanMembersPerCampaign { get; set; }
			public bool TrackAllNameChanges { get; set; }
			public bool CopyConfigOnAnyNameChange { get; set; }
			public bool UpdateAllConfigsOnAnyNameChange { get; set; }
			public bool ApplyTitleConfigToPlayer { get; set; }
			public bool ApplyTitleConfigToPlayerCompanions { get; set; }
			public bool ApplyToPlayerCaravans { get; set; }

			public TitleProperties GlobalDefault { get; set; }

			[JsonConstructor]
			public ModSettingsSerializable(ModVersion loadedVersion, List<TitleConfiguration> titleConfigs, Dictionary<string, HashSet<string>> subModuleToCultures, Dictionary<string, HashSet<string>> subModuleToKingdoms, Dictionary<string, IDictionary<string, RulingClanPossibility>> deadSpecialRulingClanMembersPerCampaign,bool trackAllNameChanges, bool copyConfigOnAnyNameChange, bool updateAllConfigsOnAnyNameChange, bool applyTitleConfigToPlayer, bool applyTitleConfigToPlayerCompanions, bool applyToPlayerCaravans, TitleProperties globalDefault) {
				this.LoadedVersion = loadedVersion;
				this.TitleConfigs = titleConfigs;
				this.SubModuleToCultures = subModuleToCultures;
				this.SubModuleToKingdoms = subModuleToKingdoms;
				this.DeadSpecialRulingClanMembersPerCampaign = deadSpecialRulingClanMembersPerCampaign;
				this.TrackAllNameChanges = trackAllNameChanges;
				this.CopyConfigOnAnyNameChange = copyConfigOnAnyNameChange;
				this.UpdateAllConfigsOnAnyNameChange = updateAllConfigsOnAnyNameChange;
				this.ApplyTitleConfigToPlayer = applyTitleConfigToPlayer;
				this.ApplyTitleConfigToPlayerCompanions = applyTitleConfigToPlayerCompanions;
				this.ApplyToPlayerCaravans = applyToPlayerCaravans;
				this.GlobalDefault = globalDefault;
			}

			internal ModSettingsSerializable() :
				this(Instance._loadedVersion, Instance._titleConfigs, Instance._subModuleToCultures, Instance._subModuleToKingdoms, Instance._deadSpecialRulingClanMembersPerCampaign, Instance.TrackAllNameChanges, Instance.CopyConfigOnAnyNameChange, Instance.UpdateAllConfigsOnAnyNameChange,
					Instance.ApplyTitleConfigToPlayer, Instance.ApplyTitleConfigToPlayerCompanions,
					Instance.ApplyToPlayerCaravans, Instance.GlobalDefault) { }
		}
	}
}
