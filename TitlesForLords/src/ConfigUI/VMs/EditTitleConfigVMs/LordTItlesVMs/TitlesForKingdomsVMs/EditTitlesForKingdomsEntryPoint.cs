using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitlesForLords.src.main.Core.Settings;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.LordTitlesVMs.TitlesForKingdomsVMs {
	internal class EditTitlesForKingdomsEntryPoint : SettingsLayerBaseVM {

		enum Command { None, EditDefault, EditForKingdoms, EditForCultures }

		Command _toNextScreen;
		Command _toNextScreenBeforeExecuteBack;

		readonly SettingsLayerBaseVM _parent;
		readonly TitleConfiguration _config;
		readonly bool _isEditable;

		internal override string PathDescriptor => "kingdoms";

		[DataSourceProperty]
		public MBBindingList<ListButtonVM> Buttons { get; }

		internal EditTitlesForKingdomsEntryPoint(TitleConfiguration config, bool isEditable, SettingsLayerBaseVM parent, ConfigUIBaseVM baseVM) : base(baseVM) {
			_parent = parent;
			_config = config;
			_isEditable = isEditable;
			const string editDefaultHint = "Default properties to apply to all kingdoms. To avoid conflicting with conversion mods, these are only applied once there is also a match for the kingdom or the culture in this config.\n\nProperties defined in here overwrite undefined properties in the other categories, but are overwritten by defined properties in the other categories.";
			const string editTitlesForKingdomsHint = "Properties to apply to lords depending on their kingdom. Properties defined in here are overwrite default properties and properties defined for the kingdom culture";
			const string editTitlesForCulturesHint = "Properties to apply to lords depending on the culture of their kingdom. Properties defined in here overwrite default properties, but are overwritten by properties defined for specific kingdoms.";

			Buttons = new MBBindingList<ListButtonVM> {
				new ListButtonVM("Edit default", editDefaultHint, ExecuteEditDefault),
				new ListButtonVM("Edit titles for specific kingdoms", editTitlesForKingdomsHint, ExecuteEditTitlesForKingdoms),
				new ListButtonVM("Edit titles for kingdom culture", editTitlesForCulturesHint, ExecuteEditTitlesForCultures),
			};
		}

		public void ExecuteEditDefault() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new KingdomPropertiesVM($"default for {_config.Metadata.Uid}", "default", _config.TitlesForLords.TitlesForKingdoms.Default, _config, _isEditable, this.IsValid,
				() => ModSettings.Instance.TitleConfigs[ModSettings.Instance.TitleConfigs.IndexOf(_config)].TitlesForLords.TitlesForKingdoms.Default,
				BaseVM);
			var movie = layer.LoadMovie("CTButtonList", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditDefault;
		}

		public void ExecuteEditTitlesForKingdoms() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new SpecificKingdomPropertiesVM(_config, _isEditable, this, BaseVM);
			var movie = layer.LoadMovie("CTDefaultList", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditForKingdoms;
		}

		public void ExecuteEditTitlesForCultures() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new CulturePropertiesVM(_config, _isEditable, this, BaseVM);
			var movie = layer.LoadMovie("CTDefaultList", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditForCultures;
		}

		internal override void RestoreNextScreenPostResetAndTransferAdditionalState(SettingsLayerBaseVM newInstanceOfThisVM) {
			if (newInstanceOfThisVM is EditTitlesForKingdomsEntryPoint newInstance) {
				newInstance._toNextScreen = _toNextScreen;
				switch (_toNextScreen) {
					case Command.None:
						return;
					case Command.EditDefault:
						newInstance.ExecuteEditDefault();
						return;
					case Command.EditForKingdoms:
						newInstance.ExecuteEditTitlesForKingdoms();
						return;
					case Command.EditForCultures:
						newInstance.ExecuteEditTitlesForCultures();
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
