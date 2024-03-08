using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig.TitleConfigElements;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using System.Collections.Generic;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.TitleConfigsVM;
using TaleWorlds.Engine.GauntletUI;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.KingdomsAndCulturesView;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.NotableOwnedCaravansVMs {
	internal class EditNotableCaravansForKingdoms : DefaultListBaseVM {

		protected override string ID { get; } = "EditNotableCaravansForKingdoms";
		protected override string EntryHint { get; } = "Click to configure caravan properties for notables in this kingdom. The name must match the kingdom name exactly. These entries are sorted alphabetically when opening this screen or after resetting.";

		NotableOwnedCaravans NotableOwnedCaravans => _config.NotableCaravans;

		internal override string PathDescriptor => "Kingdom notable settings";

		[DataSourceProperty]
		public bool IsAdditionalButtonVisible { get; } = true;
		[DataSourceProperty]
		public string AdditionalButtonText { get; } = "known kingdoms";
		[DataSourceProperty]
		public string CreateNewEntryName { get; } = "Add settings for the notables in a new kingdom.";
		[DataSourceProperty]
		public HintViewModel AddNewEntryHint { get; } = new HintViewModel(new TextObject("Create caravan properties for the notables in a new kingdom. The name must match the kingdom name exactly. An entry with the name can't exist yet."));

		protected override IEnumerable<string> EntriesOriginKeys => NotableOwnedCaravans.KingdomProperties.Keys;

		public EditNotableCaravansForKingdoms(TitleConfiguration config, bool isEditable, SettingsLayerBaseVM parent, ConfigUIBaseVM baseVM) : base(config, isEditable, parent, baseVM) {
		}

		public override void ExecuteAdditionalButtonPressed() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new CulturesAndKingdomsForModsVM(this, BaseVM);
			var movie = layer.LoadMovie("CTCulturesAndKingdomsForMods", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			NextScreenOpenedViaAdditionalButton = true;
		}

		public override void ExecuteCreateNewEntry() {
			BaseVM.Screen.OpenPopUp(new EditableTextPopUpVM("Enter name of kingdom",
			kingdomName => {
				if (kingdomName is null || NotableOwnedCaravans.KingdomProperties.ContainsKey(kingdomName)) {
					InformationManager.DisplayMessage(new InformationMessage("Did not create. An entry for that kingdom already exists."));
					return;
				}
				NotableOwnedCaravans.AddPropertiesForKingdom(kingdomName, new CaravanProperties());
				AddToEntries(kingdomName);
			}, BaseVM.Screen
			));
		}

		protected override bool Rename(string oldKingdomKey, string newKingdomKey) {
			if (NotableOwnedCaravans.KingdomProperties.ContainsKey(newKingdomKey)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not rename. An entry with the entered name already exists."));
				return false;
			}
			CaravanProperties properties = NotableOwnedCaravans.KingdomProperties[oldKingdomKey];
			NotableOwnedCaravans.DeletePropertiesForKingdom(oldKingdomKey);
			NotableOwnedCaravans.AddPropertiesForKingdom(newKingdomKey, properties);
			return true;
		}

		protected override bool Copy(string kingdomKey, string newKingdomKey) {
			if (newKingdomKey is null || NotableOwnedCaravans.KingdomProperties.ContainsKey(newKingdomKey)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not copy. An entry with the entered name already exists."));
				return false;
			}
			NotableOwnedCaravans.AddPropertiesForKingdom(newKingdomKey, new CaravanProperties(NotableOwnedCaravans.KingdomProperties[kingdomKey]));
			return true;
		}

		protected override void Delete(string kingdomKey) {
			NotableOwnedCaravans.DeletePropertiesForKingdom(kingdomKey);
		}

		protected override void ExecuteSelectEntry(string kingdomKey, string originalKey) {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditCaravanPropertiesVM(new CaravanPropertiesVM(NotableOwnedCaravans.KingdomProperties[kingdomKey], IsEditEnabled), kingdomKey, this, BaseVM);
			var movie = layer.LoadMovie("CTEditCaravanProperties", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
		}

		internal override bool IsChildValid() {
			return !(NextScreenOpenedWithOriginalKey is null) &&
				ModTitleConfigs.Contains(_config) &&
				ModTitleConfigs[ModTitleConfigs.IndexOf(_config)].NotableCaravans.KingdomProperties.ContainsKey(NextScreenOpenedWithOriginalKey);
		}
	}
}
