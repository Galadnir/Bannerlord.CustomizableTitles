using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.Core;

namespace Bannerlord.TitlesForLords.src.main.Core.Settings {

	// patch notes:
	// moved save location
	// track active configs based on user ==> if savefile was already converted, only default configs active now
	// config menu now openable in-game
	// hotkeys now configurable

	public enum ModVersion { v1, v2 }
	public enum RulingClanPossibility { Ruler, SpouseOfRuler, ChildOfRuler, Member }
	internal sealed class ModSettings {

		internal static string MBBannerlordSteamID = "261550";

		internal static readonly string V1SavefileLocation = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\CustomizableTitlesSettings.json";

#if DEBUG
		internal static readonly string SavefileLocation = $@"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\Mount and Blade II Bannerlord\Mods\CustomizableTitles--Debug\CustomizableTitlesSettings.json";
		internal static readonly string McmModFolderPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\Mount and Blade II Bannerlord\Configs\ModSettings\Global\CustomizableTitles--Debug";
#else
		internal static readonly string SavefileLocation = $@"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\Mount and Blade II Bannerlord\Mods\CustomizableTitles\CustomizableTitlesSettings.json";
		internal static readonly string McmModFolderPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\Mount and Blade II Bannerlord\Configs\ModSettings\Global\CustomizableTitles";
#endif

		internal static readonly string UserActiveConfigsFile = $@"{McmModFolderPath}\activeConfigs.json";

		internal static readonly string ConfigJsonsBasePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\..\..\..\";
		internal const string ConfigJsonName = "CustomizableTitlesModConfig.json";
		internal static readonly string SuccessfullyLoadedConfigJsonName = $"LoadedSuccessfully_{ConfigJsonName}";
		internal static readonly string FailedToLoadConfigJsonName = $"{ConfigJsonName}_failed_to_parse.reason.txt";

		static internal ModSettings Instance = new ModSettings();

		internal const ModVersion CurrentVersion = ModVersion.v2;

		ModVersion _loadedVersion;

		List<TitleConfiguration> _titleConfigs; // list for priority, the lower the index the higher the priority as it gets checked earlier
		Dictionary<string, HashSet<string>> _subModuleToCultures;
		Dictionary<string, HashSet<string>> _subModuleToKingdoms;
		Dictionary<string, IDictionary<string, RulingClanPossibility>> _deadSpecialRulingClanMembersPerCampaign;

		HashSet<string> _lastListedKingdoms;
		HashSet<string> _lastListedCultures;

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
			if (_loadedVersion == ModVersion.v1) {
				ConvertToV2();
			}
			LoadWhichConfigsActive();
		}

		internal void RegisterLastListedKingdomsAndCultures(HashSet<string> kingdoms, HashSet<string> cultures) {
			_lastListedKingdoms = kingdoms;
			_lastListedCultures = cultures;
		}

		private void LoadFromSavefile() {
			using (StreamReader sr = File.OpenText(SavefileLocation)) {
				var loadedSettings = new JsonSerializer().Deserialize<ModSettingsSerializable>(new JsonTextReader(sr));
				_titleConfigs = loadedSettings.TitleConfigs ?? new List<TitleConfiguration>();
				_loadedVersion = loadedSettings.LoadedVersion;
				_subModuleToCultures = loadedSettings.SubModuleToCultures;
				_subModuleToKingdoms = loadedSettings.SubModuleToKingdoms;
				_deadSpecialRulingClanMembersPerCampaign = loadedSettings.DeadSpecialRulingClanMembersPerCampaign;
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
			if (File.Exists(V1SavefileLocation) && !File.Exists(SavefileLocation)) {
				new FileInfo(SavefileLocation).Directory.Create();
				File.Move(V1SavefileLocation, SavefileLocation);
			}
			return !File.Exists(SavefileLocation);
		}

		private void ConvertToV2() {
			if (File.Exists(UserActiveConfigsFile)) {
				return;
			}
			SaveUserActiveConfigs();
			var defaultConfig = _titleConfigs.FirstOrDefault(config => config?.Metadata.Uid == "TitleOverhaulDefaultConfig");
			if (!(defaultConfig is null)) {
				defaultConfig.TitlesForLords.Default = RankMember.CreateEmpty();
			}
			_loadedVersion = ModVersion.v2;
		}

		private void SaveUserActiveConfigs() {
			var activeConfigIDs = ActiveTitleConfigs.Select(config => config.Metadata.Uid);
			new FileInfo(UserActiveConfigsFile).Directory.Create();
			File.WriteAllText(UserActiveConfigsFile, JsonConvert.SerializeObject(activeConfigIDs));
		}

		private void LoadWhichConfigsActive() {
			if (!File.Exists(UserActiveConfigsFile)) {
				foreach (var config in _titleConfigs) {
					config.Options.IsActive = config.Options.IsDefault;
				}
				return;
			}
			StreamReader sr = File.OpenText(UserActiveConfigsFile);
			try {
				var userActiveConfigIDs = new JsonSerializer().Deserialize<List<string>>(new JsonTextReader(sr));
				foreach (var config in _titleConfigs) {
					config.Options.IsActive = userActiveConfigIDs.Contains(config.Metadata.Uid);
				}
			} catch (Exception e) when (e is UnauthorizedAccessException || e is NotSupportedException || e is JsonException) {

			} finally {
				sr.Close();
			}
		}

		internal IList<string> LoadAndSaveNewJsonConfigFiles() {
			var loadedConfigNames = new List<string>();
			foreach (string configJson in GetAllConfigJsonFiles()) {
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
			SaveUserActiveConfigs();
			if (!(Campaign.Current is null)) {
				foreach (PartyComponent party in Campaign.Current.MobileParties.Select(x => x.PartyComponent)) {
					party.ClearCachedName();
				}
			}
		}

		internal void Restore() {
			LoadFromSavefile();
			if (!(_lastListedCultures is null)) { // as the settings are restored upon entering the config menu, the last listed cultures and kingdoms have to be set here
				_subModuleToCultures["last listed kingdoms and cultures"] = _lastListedCultures;
			}
			if (!(_lastListedKingdoms is null)) {
				_subModuleToKingdoms["last listed kingdoms and cultures"] = _lastListedKingdoms;
			}
		}

		internal TitleConfiguration CopyExistingTitleConfig(TitleConfiguration config) {
			var copy = new TitleConfiguration(config, Guid.NewGuid().ToString());
			_titleConfigs.Insert(_titleConfigs.IndexOf(config) + 1, copy); // transfer priority
			return copy;
		}

		internal void DeleteConfig(TitleConfiguration config) {
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
				_deadSpecialRulingClanMembersPerCampaign[campaignId] = new Dictionary<string, RulingClanPossibility>();
			}
			if (victim?.Id is null) {
				return;
			}
			_deadSpecialRulingClanMembersPerCampaign[campaignId][victim.Id.ToString()] = rulerRelationship;
		}

		private IEnumerable<string> GetAllConfigJsonFiles() {
			string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			IEnumerable<string> additionalConfigs = new string[0];
			if (assemblyPath.Contains("workshop") && assemblyPath.Contains(MBBannerlordSteamID)) {
				string gameModulesDirectory = $"{ConfigJsonsBasePath}../../../common/Mount & Blade II Bannerlord/Modules/";
				if (Directory.Exists(gameModulesDirectory)) {
					additionalConfigs = Directory.GetFiles(gameModulesDirectory, ConfigJsonName, SearchOption.AllDirectories);
				}
			} else {
				string workshopDirectory = $"{ConfigJsonsBasePath}../../../workshop/content/{MBBannerlordSteamID}/";
				if (Directory.Exists(workshopDirectory)) {
					additionalConfigs = Directory.GetFiles(workshopDirectory, ConfigJsonName, SearchOption.AllDirectories);
				}
			}
			return Directory.GetFiles(ConfigJsonsBasePath, ConfigJsonName, SearchOption.AllDirectories).Union(additionalConfigs);
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
			public ModSettingsSerializable(ModVersion loadedVersion, List<TitleConfiguration> titleConfigs, Dictionary<string, HashSet<string>> subModuleToCultures, Dictionary<string, HashSet<string>> subModuleToKingdoms, Dictionary<string, IDictionary<string, RulingClanPossibility>> deadSpecialRulingClanMembersPerCampaign, bool trackAllNameChanges, bool copyConfigOnAnyNameChange, bool updateAllConfigsOnAnyNameChange, bool applyTitleConfigToPlayer, bool applyTitleConfigToPlayerCompanions, bool applyToPlayerCaravans, TitleProperties globalDefault) {
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
