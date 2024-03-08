using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs;
using Bannerlord.TitlesForLords.src.main.Core.Settings;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigsVM {
	public class TitleConfigEntryVM : ViewModel {

		ConfigUIBaseVM _baseVM;
		
		string _configName;
		readonly TitleConfigurationsVM _titleConfigurationsVM;

		internal TitleConfiguration Config { get; }

		[DataSourceProperty]
		public string ConfigName {
			get => _configName;
			set {
				if (value != _configName) {
					_configName = value;
					OnPropertyChangedWithValue(value, nameof(ConfigName));
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel EntryHint { get; } = new HintViewModel(new TextObject("Active configurations are searched by this mod for titles to apply.\n" +
				"If there are multiple active configurations, higher priority configurations are checked first to find a title. The order of this list is the priority.\n" +
				"Default (default) configurations cannot be edited/renamed/deleted."));
		[DataSourceProperty]
		public HintViewModel IsActiveHint { get; } = new HintViewModel(new TextObject("If this is true, this mod searches this configuration for titles to apply\nIf there are multiple active configurations, higher priority configurations are checked first to find a title."));
		[DataSourceProperty]
		public HintViewModel RenameHint { get; } = new HintViewModel(new TextObject("Change the name of this configuration. Default configurations cannot be renamed"));
		[DataSourceProperty]
		public HintViewModel CopyHint { get; } = new HintViewModel(new TextObject("Copy this config under a new name. If a default configuration is copied, the copy is not a default configuration and can therefore be edited."));
		[DataSourceProperty]
		public HintViewModel DeleteHint { get; } = new HintViewModel(new TextObject("Delete this config."));
		[DataSourceProperty]
		public HintViewModel PriorityHint { get; } = new HintViewModel(new TextObject("Edit the priority of this configuration. Configurations are checked for titles to apply in order of their priority."));

		[DataSourceProperty]
		public bool IsNonDefaultActionPermitted { get => !this.Config.Options.IsDefault; }
		[DataSourceProperty]
		public bool IsActive {
			get => this.Config.Options.IsActive;
			set {
				if (value != this.Config.Options.IsActive) {
					this.Config.Options.IsActive = value;
					OnPropertyChangedWithValue(value, nameof(IsActive));
				}
			}
		}

		internal TitleConfigEntryVM(TitleConfiguration config, TitleConfigurationsVM titleConfigurationsVM, ConfigUIBaseVM baseVM) {
			this.Config = config;
			ConfigName = config.Metadata.Name;
			if (config.Options.IsDefault) {
				ConfigName += " (default)";
			}
			this._titleConfigurationsVM = titleConfigurationsVM;
			this._baseVM = baseVM;
		}

		public void ExecuteSelect() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditConfigEntryPointVM(this.Config, _titleConfigurationsVM.BaseVM);
			var movie = layer.LoadMovie("CTButtonList", vm);
			_titleConfigurationsVM.BaseVM.PushLayerAndMovie(layer, movie, vm);
			_titleConfigurationsVM.NextScreenOpenedBy = this;
		}

		public void ExecuteDelete() {
			ModSettings.Instance.DeleteConfig(this.Config);
			DeleteHint.ExecuteEndHint();
			_titleConfigurationsVM.RefreshValues();
			_baseVM.DisableForwardInHistory();
		}
		public void ExecuteCopy() {
			ConfigUIScreen screen = _titleConfigurationsVM.BaseVM.Screen;
			screen.OpenPopUp(new EditableTextPopUpVM("Enter name of copy",
				value => {
					var copiedConfig = ModSettings.Instance.CopyExistingTitleConfig(this.Config);
					copiedConfig.Metadata.Name = value;
					ConfigName = value;
					_titleConfigurationsVM.RefreshValues();
				},
				screen));

		}

		public void ExecuteRename() {
			ConfigUIScreen screen = _titleConfigurationsVM.BaseVM.Screen;
			screen.OpenPopUp(new EditableTextPopUpVM("Enter new name",
				value => {
					this.Config.Metadata.Name = value;
					ConfigName = value;
					_baseVM.DisableForwardInHistory();
				},
				screen));
		}

		public void ExecutePriorityUp() {
			ModSettings.Instance.IncreasePriority(this.Config);
			_titleConfigurationsVM.RefreshValues();
		}

		public void ExecutePriorityDown() {
			ModSettings.Instance.DecreasePriority(this.Config);
			_titleConfigurationsVM.RefreshValues();
		}
	}
}
