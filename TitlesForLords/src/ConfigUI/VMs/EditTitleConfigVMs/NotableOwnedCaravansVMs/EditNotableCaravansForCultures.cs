using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig.TitleConfigElements;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using System.Collections.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.TitleConfigsVM;
using TaleWorlds.Engine.GauntletUI;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.KingdomsAndCulturesView;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.NotableOwnedCaravansVMs {
	internal class EditNotableCaravansForCultures : DefaultListBaseVM {

		protected override string ID { get; }  = "EditNotableCaravansForCultures";
		protected override string EntryHint { get; } = "Click to configure caravan properties for notables belonging to a kingdom of a certain culture. The name must match the culture name exactly. These entries are sorted alphabetically when opening this screen or after resetting.";

		NotableOwnedCaravans NotableOwnedCaravans => _config.NotableCaravans;

		internal override string PathDescriptor => "Culture notable settings";

		[DataSourceProperty]
		public bool IsAdditionalButtonVisible { get; } = true;
		[DataSourceProperty]
		public string AdditionalButtonText { get; } = "known cultures";

		[DataSourceProperty]
		public string CreateNewEntryName { get; } = "Add settings for the notables in a new culture";
		[DataSourceProperty]
		public HintViewModel AddNewEntryHint { get; } = new HintViewModel(new TextObject("Create caravan properties for the notables in a new culture. The name must match the culture name exactly. An entry with the name can't exist yet."));

		protected override IEnumerable<string> EntriesOriginKeys => NotableOwnedCaravans.CultureProperties.Keys;

		public EditNotableCaravansForCultures(TitleConfiguration config, bool isEditable, SettingsLayerBaseVM parent, ConfigUIBaseVM baseVM) : base(config, isEditable, parent, baseVM) {
		}

		public override void ExecuteAdditionalButtonPressed() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new CulturesAndKingdomsForModsVM(this, BaseVM);
			var movie = layer.LoadMovie("CTCulturesAndKingdomsForMods", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			NextScreenOpenedViaAdditionalButton = true;
		}

		public override void ExecuteCreateNewEntry() {
			BaseVM.Screen.OpenPopUp(new EditableTextPopUpVM("Enter name of culture",
			cultureName => {
				if (cultureName is null || NotableOwnedCaravans.CultureProperties.ContainsKey(cultureName)) {
					InformationManager.DisplayMessage(new InformationMessage("Did not create. An entry for that culture already exists."));
					return;
				}
				NotableOwnedCaravans.AddPropertiesForCulture(cultureName, new CaravanProperties());
				AddToEntries(cultureName);
			}, BaseVM.Screen
			));
		}
		protected override bool Rename(string oldCultureKey, string newCultureKey) {
			if (NotableOwnedCaravans.CultureProperties.ContainsKey(newCultureKey)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not rename. An entry with the entered name already exists."));
				return false;
			}
			CaravanProperties properties = NotableOwnedCaravans.CultureProperties[oldCultureKey];
			NotableOwnedCaravans.DeletePropertiesForCulture(oldCultureKey);
			NotableOwnedCaravans.AddPropertiesForCulture(newCultureKey, properties);
			return true;
		}

		protected override bool Copy(string cultureKey, string newCultureKey) {
			if (newCultureKey is null || NotableOwnedCaravans.CultureProperties.ContainsKey(newCultureKey)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not copy. An entry with the entered name already exists."));
				return false;
			}
			NotableOwnedCaravans.AddPropertiesForCulture(newCultureKey, new CaravanProperties(NotableOwnedCaravans.CultureProperties[cultureKey]));
			return true;
		}
		protected override void Delete(string cultureKey) {
			NotableOwnedCaravans.DeletePropertiesForCulture(cultureKey);
		}

		protected override void ExecuteSelectEntry(string cultureKey, string originalKey) {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditCaravanPropertiesVM(new CaravanPropertiesVM(NotableOwnedCaravans.CultureProperties[cultureKey], IsEditEnabled), cultureKey, this, BaseVM);
			var movie = layer.LoadMovie("CTEditCaravanProperties", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
		}

		internal override bool IsChildValid() {
			return !(NextScreenOpenedWithOriginalKey is null) &&
				ModTitleConfigs.Contains(_config) &&
				ModTitleConfigs[ModTitleConfigs.IndexOf(_config)].NotableCaravans.CultureProperties.ContainsKey(NextScreenOpenedWithOriginalKey);
		}
	}
}
