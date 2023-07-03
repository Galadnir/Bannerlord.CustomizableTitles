using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
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
	internal class CharacterKingdomPropertiesVM : DefaultListBaseVM {

		protected override string ID { get; } = "CharacterKingdomProperties";
		protected override string EntryHint { get; } = "Click to configure the properties of this character, when he belongs to this kingdom. The name must match the kingdom name exactly";

		readonly string _characterKey;

		TitlesForTContainer<TitleProperties> CharacterKingdomProperties => _config.TitlesForLords.TitlesForCharacters.Characters[_characterKey].SpecificKingdomsContainer;

		internal override string PathDescriptor => "Kingdom properties";

		[DataSourceProperty]
		public string CreateNewEntryName { get; } = "Add properties for a new kingdom for this character";
		[DataSourceProperty]
		public HintViewModel AddNewEntryHint { get; } = new HintViewModel(new TextObject("Create title properties which are applied while this character belongs to this kingdom. The name must match the kingdom name exactly. An entry with that name can't exist yet."));

		[DataSourceProperty]
		public bool IsAdditionalButtonVisible { get; } = true;
		[DataSourceProperty]
		public string AdditionalButtonText { get; } = "edit default";
		[DataSourceProperty]
		public HintViewModel AdditionalButtonHint { get; } = new HintViewModel(new TextObject("A default that applies to all kingdoms defined here for this character."));

		protected override IEnumerable<string> EntriesOriginKeys => CharacterKingdomProperties.Properties.Keys;

		public CharacterKingdomPropertiesVM(TitleConfiguration config, string characterKey, bool isEditable, SettingsLayerBaseVM parent, ConfigUIBaseVM baseVM) : base(config, isEditable, parent, baseVM, false) {
			_characterKey = characterKey;
			CreateStartingEntries();
		}

		public override void ExecuteAdditionalButtonPressed() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditTitlePropertiesVM(EditTitlePropertiesVMWidget.CreateFor(CharacterKingdomProperties.Default, IsEditEnabled),
				"default",
				BaseVM, this.IsValid);
			var movie = layer.LoadMovie("CTEditTitleProperties", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			NextScreenOpenedViaAdditionalButton = true;
		}
		public override void ExecuteCreateNewEntry() {
			BaseVM.Screen.OpenPopUp(new EditableTextPopUpVM("Enter name of kingdom",
			kingdomName => {
				if (kingdomName is null || CharacterKingdomProperties.Properties.ContainsKey(kingdomName)) {
					InformationManager.DisplayMessage(new InformationMessage("Did not create. An entry for that kingdom already exists."));
					return;
				}
				CharacterKingdomProperties.AddProperty(kingdomName, new TitleProperties());
				AddToEntries(kingdomName);
			}, BaseVM.Screen
			));
		}
		protected override bool Rename(string oldKingdomKey, string newKingdomKey) {
			if (CharacterKingdomProperties.Properties.ContainsKey(newKingdomKey)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not rename. An entry with the entered name already exists."));
				return false;
			}
			TitleProperties properties = CharacterKingdomProperties.Properties[oldKingdomKey];
			CharacterKingdomProperties.DeleteProperty(oldKingdomKey);
			CharacterKingdomProperties.AddProperty(newKingdomKey, properties);
			return true;
		}
		protected override bool Copy(string kingdomKey, string newKingdomKey) {
			if (newKingdomKey is null || CharacterKingdomProperties.Properties.ContainsKey(newKingdomKey)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not copy. An entry with the entered name already exists."));
				return false;
			}
			CharacterKingdomProperties.AddProperty(newKingdomKey, new TitleProperties(CharacterKingdomProperties.Properties[kingdomKey]));
			return true;
		}
		protected override void Delete(string kingdomKey) {
			CharacterKingdomProperties.DeleteProperty(kingdomKey);
		}
		protected override void ExecuteSelectEntry(string kingdomKey, string originalKey) {
			TitleProperties properties = CharacterKingdomProperties.Properties[kingdomKey];
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditTitlePropertiesVM(EditTitlePropertiesVMWidget.CreateFor(properties, IsEditEnabled),
				kingdomKey, BaseVM, this.IsOpenedEntryValid);
			var movie = layer.LoadMovie("CTEditTitleProperties", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
		}

		private bool IsOpenedEntryValid() {
			return !(NextScreenOpenedWithOriginalKey is null) &&
				ModTitleConfigs.Contains(_config) &&
				ModTitleConfigs[ModTitleConfigs.IndexOf(_config)].TitlesForLords.TitlesForCharacters.Characters.ContainsKey(_characterKey) &&
				ModTitleConfigs[ModTitleConfigs.IndexOf(_config)].TitlesForLords.TitlesForCharacters.Characters[_characterKey].
				SpecificKingdomsContainer.Properties.ContainsKey(NextScreenOpenedWithOriginalKey);
		}
	}
}
