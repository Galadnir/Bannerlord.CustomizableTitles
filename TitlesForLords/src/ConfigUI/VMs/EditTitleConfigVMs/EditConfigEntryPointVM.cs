using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.LordTitlesVMs;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.NotableOwnedCaravansVMs;
using Bannerlord.TitlesForLords.src.main.Core.Settings;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs {
	internal class EditConfigEntryPointVM : SettingsLayerBaseVM {

		enum Command { None, EditLordTitles, EditNotableOwnedCaravans, ViewAutomaticRenames }

		Command _toNextScreen;
		Command _toNextScreenBeforeExecuteBack;

		readonly TitleConfiguration _config;

		internal override string PathDescriptor => _config.Metadata.Name + (_config.Options.IsDefault ? " (default)" : "");

		[DataSourceProperty]
		public MBBindingList<ListButtonVM> Buttons { get; }

		internal EditConfigEntryPointVM(TitleConfiguration config, ConfigUIBaseVM baseVM) : base(baseVM) {
			_config = config;

			const string editLordTitlesHint = "Edit the titles for lords, the names of their villagers and the names of their caravans for this configuration. If this is a default configuration, the rules can only be viewed, not edited.";
			const string editNotableOwnedCaravansHint = "Edit the names of notable owned caravans for this configuration. If this is a default configuration, the rules can only be viewed, not edited.";
			const string automaticRenamesHint = "View renames additional rules were generated for in this configuration. For example after you defined titles only for your character and then rename him, that transition is shown here.";
			Buttons = new MBBindingList<ListButtonVM> {
				new ListButtonVM("Configure settings for lords", editLordTitlesHint, ExecuteEditLordTitles),
				new ListButtonVM("Configure settings for notable owned caravans", editNotableOwnedCaravansHint, ExecuteEditNotableOwnedCaravans),
				new ListButtonVM("View automatic renames in this config", automaticRenamesHint, ExecuteViewAutomaticRenames)
			};

		}

		public void ExecuteEditLordTitles() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditLordTitlesEntryPointVM(_config, !_config.Options.IsDefault, this, BaseVM);
			var movie = layer.LoadMovie("CTButtonList", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditLordTitles;
		}

		public void ExecuteEditNotableOwnedCaravans() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new NotableCaravansEntryPointVM(_config, !_config.Options.IsDefault, this, BaseVM);
			var movie = layer.LoadMovie("CTButtonList", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditNotableOwnedCaravans;
		}

		public void ExecuteViewAutomaticRenames() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new ViewAutomaticRenamesVM(_config.Metadata.AutomaticRenames, this, BaseVM);
			var movie = layer.LoadMovie("CTViewAutomaticRenames", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.ViewAutomaticRenames;
		}

		internal override void RestoreNextScreenPostResetAndTransferAdditionalState(SettingsLayerBaseVM newInstanceOfThisVM) {
			if (newInstanceOfThisVM is EditConfigEntryPointVM newInstance) {
				newInstance._toNextScreen = _toNextScreen;
				switch (_toNextScreen) {
					case Command.None:
						return;
					case Command.EditLordTitles:
						newInstance.ExecuteEditLordTitles();
						return;
					case Command.EditNotableOwnedCaravans:
						newInstance.ExecuteEditNotableOwnedCaravans();
						return;
					case Command.ViewAutomaticRenames:
						newInstance.ExecuteViewAutomaticRenames();
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
			return ModSettings.Instance.TitleConfigs.Contains(_config);
		}
	}
}
