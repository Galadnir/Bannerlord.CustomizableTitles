using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements;
using System.Collections.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using TaleWorlds.Engine.GauntletUI;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.LordTitlesVMs.TitlesForCharactersVMs {
	internal class CharacterClanPropertiesVM : DefaultListBaseVM {
		protected override string ID { get; } = "CharacterClanProperties";
		protected override string EntryHint { get; } = "Click to configure the properties of this character, when he belongs to this clan. The name must match the clan name exactly";

		readonly string _characterKey;

		TitlesForTContainer<TitleProperties> CharacterClanProperties => _config.TitlesForLords.TitlesForCharacters.Characters[_characterKey].SpecificClansContainer;

		internal override string PathDescriptor => "Clan properties";

		[DataSourceProperty]
		public string CreateNewEntryName { get; } = "Add properties for a new clan for this character";
		[DataSourceProperty]
		public HintViewModel AddNewEntryHint { get; } = new HintViewModel(new TextObject("Create title properties which are applied while this character belongs to this clan. The name must match the clan name exactly. An entry with that name can't exist yet."));

		[DataSourceProperty]
		public bool IsAdditionalButtonVisible { get; } = true;
		[DataSourceProperty]
		public string AdditionalButtonText { get; } = "edit default";
		[DataSourceProperty]
		public HintViewModel AdditionalButtonHint { get; } = new HintViewModel(new TextObject("A default that applies to all clans defined here for this character."));

		protected override IEnumerable<string> EntriesOriginKeys => CharacterClanProperties.Properties.Keys;

		public CharacterClanPropertiesVM(TitleConfiguration config, string characterKey, bool isEditable, SettingsLayerBaseVM parent, ConfigUIBaseVM baseVM) : base(config, isEditable, parent, baseVM, false) {
			_characterKey = characterKey;
			CreateStartingEntries();
		}

		public override void ExecuteAdditionalButtonPressed() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditTitlePropertiesVM(EditTitlePropertiesVMWidget.CreateFor(CharacterClanProperties.Default, IsEditEnabled),
				"default",
				BaseVM, this.IsValid);
			var movie = layer.LoadMovie("CTEditTitleProperties", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			NextScreenOpenedViaAdditionalButton = true;
		}
		public override void ExecuteCreateNewEntry() {
			BaseVM.Screen.OpenPopUp(new EditableTextPopUpVM("Enter name of kingdom",
			clanName => {
				if (clanName is null || CharacterClanProperties.Properties.ContainsKey(clanName)) {
					InformationManager.DisplayMessage(new InformationMessage("Did not create. An entry for that clan already exists."));
					return;
				}
				CharacterClanProperties.AddProperty(clanName, new TitleProperties());
				AddToEntries(clanName);
			}, BaseVM.Screen
			));
		}
		protected override bool Rename(string oldClanKey, string newClanKey) {
			if (CharacterClanProperties.Properties.ContainsKey(newClanKey)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not rename. An entry with the entered name already exists."));
				return false;
			}
			TitleProperties properties = CharacterClanProperties.Properties[oldClanKey];
			CharacterClanProperties.DeleteProperty(oldClanKey);
			CharacterClanProperties.AddProperty(newClanKey, properties);
			return true;
		}
		protected override bool Copy(string clanKey, string newClanKey) {
			if (newClanKey is null || CharacterClanProperties.Properties.ContainsKey(newClanKey)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not copy. An entry with the entered name already exists."));
				return false;
			}
			CharacterClanProperties.AddProperty(newClanKey, new TitleProperties(CharacterClanProperties.Properties[clanKey]));
			return true;
		}
		protected override void Delete(string clanKey) {
			CharacterClanProperties.DeleteProperty(clanKey);
		}
		protected override void ExecuteSelectEntry(string clanKey, string originalKey) {
			var properties = CharacterClanProperties.Properties[clanKey];
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditTitlePropertiesVM(EditTitlePropertiesVMWidget.CreateFor(properties, IsEditEnabled),
				clanKey, BaseVM, this.IsOpenedEntryValid);
			var movie = layer.LoadMovie("CTEditTitleProperties", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
		}

		private bool IsOpenedEntryValid() {
			return !(NextScreenOpenedWithOriginalKey is null) &&
				ModTitleConfigs.Contains(_config) &&
				ModTitleConfigs[ModTitleConfigs.IndexOf(_config)].TitlesForLords.TitlesForCharacters.Characters.ContainsKey(_characterKey) &&
				ModTitleConfigs[ModTitleConfigs.IndexOf(_config)].TitlesForLords.TitlesForCharacters.Characters[_characterKey].
				SpecificClansContainer.Properties.ContainsKey(NextScreenOpenedWithOriginalKey);
		}
	}
}
