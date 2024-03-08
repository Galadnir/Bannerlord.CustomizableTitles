using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.TitleConfigsVM;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.NotableOwnedCaravansVMs {
	internal class NotableCaravansEntryPointVM : SettingsLayerBaseVM {

		enum Command { None, EditDefault, EditSpecificNotableOwned, EditKingdomProperties, EditCultureProperties }

		Command _toNextScreen;
		Command _toNextScreenBeforeExecuteBack;

		readonly SettingsLayerBaseVM _parent;
		readonly TitleConfiguration _config;
		readonly bool _isEditable;

		internal override string PathDescriptor => "notable caravans";

		[DataSourceProperty]
		public MBBindingList<ListButtonVM> Buttons { get; }

		internal NotableCaravansEntryPointVM(TitleConfiguration config, bool isEditable, SettingsLayerBaseVM parent, ConfigUIBaseVM baseVM) : base(baseVM) {
			_parent = parent;
			_config = config;
			_isEditable = isEditable;
			const string editDefaultHint = "Default properties to apply to notable owned caravans. To avoid conflicting with conversion mods, these are only applied once a specific notable, kingdom or culture match is found in this configuration for notable owned caravans.\n\nProperties defined in here overwrite undefined properties in the other categories, but are overwritten by defined properties in the other categories.";
			const string editSpecificNotableOwnedHint ="Create properties for the caravan names of a specific notable. Properties defined in here overwrite kingdom and culture properties for a notable.";
			const string editKingdomPropertiesHint = "Create properties for notable owned caravans depending on the kingdom the notable belongs to. Properties defined in here are overwritten by properties for specific notables, but overwrite properties for the culture of a notable.";
			const string editCulturePropertiesHint = "Create properties for notable owned caravans depending on the culture the notable belongs to. Properties defined in here are overwritten by properties defined for kingdoms and by properties defined for specific notables.";
			Buttons = new MBBindingList<ListButtonVM> {
				new ListButtonVM("Edit default", editDefaultHint, ExecuteEditDefault),
				new ListButtonVM("Configure settings for individual notables", editSpecificNotableOwnedHint, ExecuteEditSpecificNotableOwned),
				new ListButtonVM("Configure settings for kingdoms", editKingdomPropertiesHint, ExecuteEditKingdomProperties),
				new ListButtonVM("Configure settings for cultures", editCulturePropertiesHint, ExecuteEditCultureProperties)
			};
		}

		public void ExecuteEditDefault() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditCaravanPropertiesVM(new CaravanPropertiesVM(_config.NotableCaravans.Default, _isEditable), "Default properties", this, BaseVM);
			var movie = layer.LoadMovie("CTEditCaravanProperties", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditDefault;
		}

		public void ExecuteEditSpecificNotableOwned() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditSpecificNotableOwnedVM(_config, _isEditable, this, BaseVM);
			var movie = layer.LoadMovie("CTDefaultList", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditSpecificNotableOwned;
		}

		public void ExecuteEditKingdomProperties() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditNotableCaravansForKingdoms(_config, _isEditable, this, BaseVM);
			var movie = layer.LoadMovie("CTDefaultList", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditKingdomProperties;
		}

		public void ExecuteEditCultureProperties() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditNotableCaravansForCultures(_config, _isEditable, this, BaseVM);
			var movie = layer.LoadMovie("CTDefaultList", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditCultureProperties;
		}

		internal override void RestoreNextScreenPostResetAndTransferAdditionalState(SettingsLayerBaseVM newInstanceOfThisVM) {
			if (newInstanceOfThisVM is NotableCaravansEntryPointVM newInstance) {
				newInstance._toNextScreen = _toNextScreen;
				switch (_toNextScreen) {
					case Command.None:
						return;
					case Command.EditDefault:
						newInstance.ExecuteEditDefault();
						return;
					case Command.EditSpecificNotableOwned:
						newInstance.ExecuteEditSpecificNotableOwned();
						return;
					case Command.EditKingdomProperties:
						newInstance.ExecuteEditKingdomProperties();
						return;
					case Command.EditCultureProperties:
						newInstance.ExecuteEditCultureProperties();
						return;
				}
			}
		}

		internal override void OnIsTopLayer() {
			_toNextScreen = Command.None;
		}

		internal override void OnAfterExecuteForwardOnNewTopScreen() {
			_toNextScreen = _toNextScreenBeforeExecuteBack;
		}

		internal override void OnAfterExecuteBackOnNewTopScreen() {
			_toNextScreenBeforeExecuteBack = _toNextScreen;
		}

		internal override bool IsValid() {
			return _parent.IsValid();
		}
	}
}
