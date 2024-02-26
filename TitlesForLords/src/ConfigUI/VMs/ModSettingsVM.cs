using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigsVM;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.KingdomsAndCulturesView;
using Bannerlord.TitlesForLords.src.main.Core.Settings;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs {
	public class ModSettingsVM : SettingsLayerBaseVM {

		enum Command { None, EditConfigs, ViewKingdomsAndCultures, }

		Command _toNextScreen;
		Command _toNextScreenBeforeExecuteBack;

		internal override string PathDescriptor => string.Empty;

		[DataSourceProperty]
		public MBBindingList<CheckboxWithHintVM> CheckboxSettings { get; }

		internal ModSettingsVM(ConfigUIBaseVM baseVM) : base(baseVM) {
			CheckboxSettings = new MBBindingList<CheckboxWithHintVM>();
			CreateCheckboxSettingsVMs();
		}

		public void ExecuteEditConfigs() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new TitleConfigurationsVM(BaseVM);
			var movie = layer.LoadMovie("CTTitleConfigurations", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditConfigs;
		}

		public void ExecuteViewKingdomsAndCultures() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new CulturesAndKingdomsForModsVM(this, BaseVM);
			var movie = layer.LoadMovie("CTCulturesAndKingdomsForMods", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.ViewKingdomsAndCultures;
		}

		internal override void RestoreNextScreenPostResetAndTransferAdditionalState(SettingsLayerBaseVM newInstanceOfThisVM) {
			if (newInstanceOfThisVM is ModSettingsVM newInstance) {
				newInstance._toNextScreen = _toNextScreen;
				switch (_toNextScreen) {
					case Command.EditConfigs:
						newInstance.ExecuteEditConfigs();
						return;
					case Command.ViewKingdomsAndCultures:
						newInstance.ExecuteViewKingdomsAndCultures();
						return;
					case Command.None:
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
			return true;
		}

		private void CreateCheckboxSettingsVMs() {
			var applyToPlayerSetting = new CheckboxWithHintVM("Apply to Player", ModSettings.Instance.ApplyTitleConfigToPlayer, "If this is set, a title is also added to your hero's name if applicable", value => ModSettings.Instance.ApplyTitleConfigToPlayer = value);

			var applyToPlayerCompanionsSetting = new CheckboxWithHintVM("Apply to Player Companions", ModSettings.Instance.ApplyTitleConfigToPlayerCompanions, "If this is set, a title is also added to your companions if applicable", value => ModSettings.Instance.ApplyTitleConfigToPlayerCompanions = value);

			var applyToPlayerCaravansSetting = new CheckboxWithHintVM("Apply to Player Caravans", ModSettings.Instance.ApplyToPlayerCaravans, "If this is set, the names of your caravans are altered, if applicable", value => ModSettings.Instance.ApplyToPlayerCaravans = value);

			var trackNameChangesSetting = new CheckboxWithHintVM("Track Name Changes", ModSettings.Instance.TrackAllNameChanges, "If this is set, (an) existing configuration(s) is/are updated to account for in-game name changes like first name changes, clan name changes, hero clan changes and kingdom name changes. (If a default configuration would be updated, it is always copied beforehand", value => ModSettings.Instance.TrackAllNameChanges = value);

			var copyConfigOnNameChangeSetting = new CheckboxWithHintVM("Copy Configuration on Name Change", ModSettings.Instance.CopyConfigOnAnyNameChange, "Only has an effect if 'Track Name Changes' is set. If this is set, the updated configurations are not updated in place, but are instead copied beforehand", value => ModSettings.Instance.CopyConfigOnAnyNameChange = value);

			var updateAllConfingsOnNameChangeSetting = new CheckboxWithHintVM("Update all Configurations on Name Change", ModSettings.Instance.UpdateAllConfigsOnAnyNameChange, "Only has an effect if 'Track Name Changes' is set. If this is set, all configurations with a match for the hero, whose name changed, are updated. If this is not set, only the configuration the current title is fetched from is updated.", value => ModSettings.Instance.UpdateAllConfigsOnAnyNameChange = value);

			CheckboxSettings.Add(applyToPlayerSetting);
			CheckboxSettings.Add(applyToPlayerCompanionsSetting);
			CheckboxSettings.Add(applyToPlayerCaravansSetting);
			CheckboxSettings.Add(trackNameChangesSetting);
			CheckboxSettings.Add(copyConfigOnNameChangeSetting);
			CheckboxSettings.Add(updateAllConfingsOnNameChangeSetting);
		}
	}
}
