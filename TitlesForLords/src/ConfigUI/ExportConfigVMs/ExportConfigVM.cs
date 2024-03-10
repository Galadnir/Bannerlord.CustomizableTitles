using Bannerlord.TitlesForLords.src.main.Core.Settings;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.ExportConfigVMs {
	internal class ExportConfigVM : ViewModel {

		readonly ExportConfigScreen _exportConfigScreen;

		TitleConfiguration _toExport;
		JsonConfigFile _exportFile;

		string _uid;
		string _modName;
		string _newKingdomName;
		string _newCultureName;

		[DataSourceProperty]
		public MBBindingList<ConfigListEntry> ConfigList { get; }
		[DataSourceProperty]
		public string ModName {
			get => _modName;
			set {
				if (_modName != value) {
					_modName = value;
					OnPropertyChangedWithValue(value, nameof(ModName));
				}
			}
		}
		[DataSourceProperty]
		public string UID {
			get => _uid;
			set {
				if (_uid != value) {
					_uid = value;
					OnPropertyChangedWithValue(value, nameof(UID));
				}
			}
		}
		[DataSourceProperty]
		public MBBindingList<ModKingdomOrCultureEntry> ModKingdomsList { get; }
		[DataSourceProperty]
		public MBBindingList<ModKingdomOrCultureEntry> ModCulturesList { get; }

		[DataSourceProperty]
		public string NewKingdomName {
			get => _newKingdomName;
			set {
				if (_newKingdomName != value) {
					_newKingdomName = value;
					OnPropertyChangedWithValue(value, nameof(NewKingdomName));
				}
			}
		}
		[DataSourceProperty]
		public string NewCultureName {
			get => _newCultureName;
			set {
				if (_newCultureName != value) {
					_newCultureName = value;
					OnPropertyChangedWithValue(value, nameof(NewCultureName));
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel EnterUIDHint = new HintViewModel(new TextObject("The unique id of this mod. By providing a new default config for your mod with the same uid, the existing default config is overwritten once it is loaded."));
		[DataSourceProperty]
		public HintViewModel EnterModNameHint = new HintViewModel(new TextObject("The name of this mod. The kingdoms and cultures added for this mod are listed for this name. If there already are known kingdoms and cultures for this mod name, these are automatically added to the lists on the following screen."));

		public ExportConfigVM(ExportConfigScreen exportConfigScreen) {
			_exportConfigScreen = exportConfigScreen;
			ConfigList = new MBBindingList<ConfigListEntry>();
			foreach (var config in ModSettings.Instance.TitleConfigs) {
				ConfigList.Add(new ConfigListEntry(config.Metadata.Name, config.Metadata.Uid, this));
			}
			ModName = string.Empty;
			UID = string.Empty;
			ModKingdomsList = new MBBindingList<ModKingdomOrCultureEntry>();
			ModCulturesList = new MBBindingList<ModKingdomOrCultureEntry>();
			_newKingdomName = string.Empty;
			_newCultureName = string.Empty;
		}

		internal void SelectConfig(string configUid) {
			_toExport = ModSettings.Instance.TitleConfigs.First(config => config.Metadata.Uid == configUid);
			ModName = _toExport.Metadata.SubModule;
			UID = _toExport.Metadata.Uid;
			_exportConfigScreen.CreateSelectedConfigPopUp();
			_exportConfigScreen.ActivateLayer(_exportConfigScreen.InputModNameAndUIDLayer);
		}

		public void ExecuteConfirmModNameAndUID() {
			HashSet<string> modKingdomNames = ModSettings.Instance.SubModuleToKingdoms.Keys.Contains(ModName) ?
				ModSettings.Instance.SubModuleToKingdoms[ModName] : new HashSet<string>();
			HashSet<string> modCultureNames = ModSettings.Instance.SubModuleToCultures.Keys.Contains(ModName) ?
				ModSettings.Instance.SubModuleToCultures[ModName] : new HashSet<string>();
			var copy = new TitleConfiguration(_toExport, UID, true);
			copy.Metadata.SubModule = ModName;
			_exportFile = new JsonConfigFile(ModName, copy, modCultureNames, modKingdomNames);
			CreateKingdomEntries();
			CreateCultureEntries();
			_exportConfigScreen.TearDownSelectedConfigPopUp();
			_exportConfigScreen.ActivateLayer(_exportConfigScreen.AddKingdomsAndCulturesLayer);
		}

		public void ExecuteDiscardInputModNameAndUID() {
			_exportConfigScreen.TearDownSelectedConfigPopUp();
		}

		public void ExecuteAddKingdom() {
			if (_exportFile.Kingdoms.Contains(_newKingdomName)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not add, that kingdom is already listed."));
				return;
			}
			ModKingdomsList.Add(new ModKingdomOrCultureEntry(_newKingdomName, this, true));
			_exportFile.Kingdoms.Add(_newKingdomName);
			NewKingdomName = string.Empty;
		}

		public void ExecuteAddCulture() {
			if (_exportFile.Cultures.Contains(_newCultureName)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not add, that culture is already listed."));
				return;
			}
			ModCulturesList.Add(new ModKingdomOrCultureEntry(_newCultureName, this, false));
			_exportFile.Cultures.Add(_newCultureName);
			NewCultureName = string.Empty;
		}

		public void ExecuteCancel() {
			_exportConfigScreen.Close();
		}

		public void ExecuteExport() {
			string desctopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			File.WriteAllText($"{desctopPath}/{ModSettings.ConfigJsonName}", JsonConvert.SerializeObject(_exportFile, new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore,
			}));
			_exportConfigScreen.Close();
		}

		private void CreateKingdomEntries() {
			foreach (var kingdom in _exportFile.Kingdoms) {
				ModKingdomsList.Add(new ModKingdomOrCultureEntry(kingdom, this, true));
			}
			ModKingdomsList.Sort(Comparer<ModKingdomOrCultureEntry>.Create(
				(o1, o2) => string.Compare(o1.Name, o2.Name)));
		}

		private void CreateCultureEntries() {
			foreach (var culture in _exportFile.Cultures) {
				ModCulturesList.Add(new ModKingdomOrCultureEntry(culture, this, false));
			}
			ModCulturesList.Sort(Comparer<ModKingdomOrCultureEntry>.Create(
				(o1, o2) => string.Compare(o1.Name, o2.Name)));
		}

		private void RemoveKingdomOrCulture(ModKingdomOrCultureEntry entry, bool isKingdom) {
			if (isKingdom) {
				ModKingdomsList.Remove(entry);
				_exportFile.Kingdoms.RemoveWhere(name => name == entry.Name);
			} else {
				ModCulturesList.Remove(entry);
				_exportFile.Cultures.RemoveWhere(name => name == entry.Name);
			}
		}

		public class ConfigListEntry : ViewModel {

			readonly ExportConfigVM _partOfVm;
			readonly string _configUid;

			[DataSourceProperty]
			public string ConfigName { get; }
			[DataSourceProperty]
			public HintViewModel SelectConfigHint { get; } = new HintViewModel(new TextObject("Export this config as a default config for a (total) conversion mod. An existing mod config can be updated by providing the same UID."));

			internal ConfigListEntry(string configName, string configUid, ExportConfigVM partOfVm) {
				ConfigName = configName;
				_configUid = configUid;
				_partOfVm = partOfVm;
			}

			public void ExecuteSelect() {
				_partOfVm.SelectConfig(_configUid);
			}
		}

		public class ModKingdomOrCultureEntry : ViewModel {


			readonly ExportConfigVM _partOfVm;
			readonly bool _isKingdom;

			[DataSourceProperty]
			public string Name { get; }

			internal ModKingdomOrCultureEntry(string name, ExportConfigVM partOfVm, bool isKingdom) {
				Name = name;
				_partOfVm = partOfVm;
				_isKingdom = isKingdom;
			}

			public void ExecuteDelete() {
				_partOfVm.RemoveKingdomOrCulture(this, _isKingdom);
			}
		}
	}
}
