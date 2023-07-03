using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.KingdomsAndCulturesView;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.TitleConfigsVM;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig.TitleConfigElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.LordTitlesVMs.TitlesForCharactersVMs {
	internal class EditTitlesForCharactersListVM : DefaultListBaseVM {
		protected override string ID { get; } = "EditTitlesForCharactersList";
		protected override string EntryHint { get; } = "Click to configure settings for this character. The name must match exactly, including the clan name. For example if the name of a character is 'Ergeon'. His clan is named 'fen Seanel'. Then this entry must be called 'Ergeon fen Seanel'. Overwrites default properties defined for all characters listed here.";
		TitlesForCharacters TitlesForCharacters => _config.TitlesForLords.TitlesForCharacters;

		internal override string PathDescriptor => "lords";

		[DataSourceProperty]
		public bool IsAdditionalButtonVisible { get; } = true;
		[DataSourceProperty]
		public string AdditionalButtonText { get; } = "edit default";
		[DataSourceProperty]
		public HintViewModel AdditionalButtonHint { get; } = new HintViewModel(new TextObject("A default that applies to all characters defined here."));

		[DataSourceProperty]
		public string CreateNewEntryName { get; } = "Add settings for a new character";
		[DataSourceProperty]
		public HintViewModel AddNewEntryHint { get; } = new HintViewModel(new TextObject("Creare title properties for a new character. The name of the entry must match exactly, including the clan name. For example if the name of a character is 'Ergeon'. His clan is named 'fen Seanel'. Then this entry must be called 'Ergeon fen Seanel'."));

		protected override IEnumerable<string> EntriesOriginKeys => TitlesForCharacters.Characters.Keys;

		public EditTitlesForCharactersListVM(TitleConfiguration config, bool isEditable, SettingsLayerBaseVM parent, ConfigUIBaseVM baseVM) : base(config, isEditable, parent, baseVM) {
		}

		public override void ExecuteAdditionalButtonPressed() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditTitlePropertiesVM(EditTitlePropertiesVMWidget.CreateFor(TitlesForCharacters.Default, IsEditEnabled),
				"default", BaseVM, this.IsValid);
			var movie = layer.LoadMovie("CTEditTitleProperties", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			NextScreenOpenedViaAdditionalButton = true;
		}
		public override void ExecuteCreateNewEntry() {
			BaseVM.Screen.OpenPopUp(new EditableTextPopUpVM("Enter name of character",
			characterName => {
				if (characterName is null || TitlesForCharacters.Characters.ContainsKey(characterName)) {
					InformationManager.DisplayMessage(new InformationMessage("Did not create. An entry for that name already exists."));
					return;
				}
				TitlesForCharacters.AddPropertiesForCharacter(characterName, IndividualCharacterProperties.CreateEmpty());
				AddToEntries(characterName);
			}, BaseVM.Screen
			));
		}
		protected override bool Rename(string oldCharacterKey, string newCharacterKey) {
			if (TitlesForCharacters.Characters.ContainsKey(newCharacterKey)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not rename. An entry with the entered name already exists."));
				return false;
			}
			IndividualCharacterProperties properties = TitlesForCharacters.Characters[oldCharacterKey];
			TitlesForCharacters.DeletePropertiesForCharacter(oldCharacterKey);
			TitlesForCharacters.AddPropertiesForCharacter(newCharacterKey, properties);
			return true;
		}
		protected override bool Copy(string characterKey, string newCharacterKey) {
			if (newCharacterKey is null || TitlesForCharacters.Characters.ContainsKey(newCharacterKey)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not copy. An entry with the entered name already exists."));
				return false;
			}
			TitlesForCharacters.AddPropertiesForCharacter(newCharacterKey, new IndividualCharacterProperties(TitlesForCharacters.Characters[characterKey]));
			return true;
		}
		protected override void Delete(string characterKey) {
			TitlesForCharacters.DeletePropertiesForCharacter(characterKey);
		}
		protected override void ExecuteSelectEntry(string characterKey, string originalKey) {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditCharacterPropertiesVM(_config, characterKey, characterKey, IsEditEnabled, this, BaseVM);
			var movie = layer.LoadMovie("CTEditCharacterView", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
		}

		internal override bool IsChildValid() {
			return !(NextScreenOpenedWithOriginalKey is null) &&
				ModTitleConfigs.Contains(_config) &&
				ModTitleConfigs[ModTitleConfigs.IndexOf(_config)].TitlesForLords.TitlesForCharacters.Characters.ContainsKey(NextScreenOpenedWithOriginalKey);
		}
	}
}
