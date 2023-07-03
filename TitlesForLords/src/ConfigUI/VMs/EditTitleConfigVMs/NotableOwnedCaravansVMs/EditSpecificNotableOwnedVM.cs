using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.TitleConfigsVM;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig.TitleConfigElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.NotableOwnedCaravansVMs {
	internal class EditSpecificNotableOwnedVM : DefaultListBaseVM {

		protected override string ID { get; } = "EditSpecificNotableOwnedVM";
		protected override string EntryHint { get; } =  "Click to configure caravan properties for this notable. The name must match exactly. These entries are sorted alphabetically when opening this screen or after resetting.";

		NotableOwnedCaravans NotableOwnedCaravans => _config.NotableCaravans;

		internal override string PathDescriptor => "notable settings";

		[DataSourceProperty]
		public string CreateNewEntryName { get; } = "Add settings for a new notable.";
		[DataSourceProperty]
		public HintViewModel AddNewEntryHint { get; }  = new HintViewModel(new TextObject("Create caravan properties for a new notable. The name must match exactly. An entry with the name can't exist yet."));

		protected override IEnumerable<string> EntriesOriginKeys => NotableOwnedCaravans.NotableProperties.Keys;

		public EditSpecificNotableOwnedVM(TitleConfiguration config, bool isEditable, SettingsLayerBaseVM parent, ConfigUIBaseVM baseVM) : base(config, isEditable, parent, baseVM) {
		}

		public override void ExecuteCreateNewEntry() {
			BaseVM.Screen.OpenPopUp(new EditableTextPopUpVM("Enter name",
				notableName => {
					if (notableName is null || NotableOwnedCaravans.NotableProperties.ContainsKey(notableName)) {
						InformationManager.DisplayMessage(new InformationMessage("Did not create. An entry for that name already exists."));
						return;
					}
					NotableOwnedCaravans.AddPropertiesForNotable(notableName, new CaravanProperties());
					AddToEntries(notableName);
				}, BaseVM.Screen
				));
		}

		protected override void ExecuteSelectEntry(string notableKey, string originalKey) {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditCaravanPropertiesVM(new CaravanPropertiesVM(NotableOwnedCaravans.NotableProperties[notableKey], IsEditEnabled), notableKey, this, BaseVM);
			var movie = layer.LoadMovie("CTEditCaravanProperties", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
		}

		protected override bool Rename(string oldName, string newName) {
			if (NotableOwnedCaravans.NotableProperties.ContainsKey(newName)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not rename. An entry with the entered name already exists."));
				return false;
			}
			CaravanProperties properties = NotableOwnedCaravans.NotableProperties[oldName];
			NotableOwnedCaravans.DeletePropertiesForSpecificNotable(oldName);
			NotableOwnedCaravans.AddPropertiesForNotable(newName, properties);
			return true;
		}
		protected override bool Copy(string notableKey, string newName) {
			if (newName is null || NotableOwnedCaravans.NotableProperties.ContainsKey(newName)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not copy. An entry with the entered name already exists."));
				return false;
			}
			NotableOwnedCaravans.AddPropertiesForNotable(newName, new CaravanProperties(NotableOwnedCaravans.NotableProperties[notableKey]));
			return true;
		}

		protected override void Delete(string notableKey) {
			NotableOwnedCaravans.DeletePropertiesForSpecificNotable(notableKey);
		}

		internal override bool IsChildValid() {
			return !(NextScreenOpenedWithOriginalKey is null) &&
				ModTitleConfigs.Contains(_config) &&
				ModTitleConfigs[ModTitleConfigs.IndexOf(_config)].NotableCaravans.NotableProperties.ContainsKey(NextScreenOpenedWithOriginalKey);
		}
	}
}
