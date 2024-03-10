using Bannerlord.TitleOverhaul.src.ConfigUI.VMs;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.LordTitlesVMs.TitlesForKingdomsVMs;
using Bannerlord.TitlesForLords.src.main.Core.Settings;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;

namespace Bannerlord.TitlesForLords.src.ConfigUI.VMs.EditTitleConfigVMs.SimpleEditor {
	internal class SimpleEditConfigEntryPointVM : SettingsLayerBaseVM {

		enum Command { None, EditDefaultTitles, EditCultureTitles, EditKingdomTitles }

		Command _toNextScreen;
		Command _toNextScreenBeforeExecuteBack;

		readonly TitleConfiguration _config;

		internal override string PathDescriptor => _config.Metadata.Name + (_config.Options.IsDefault ? " (default)" : "");

		[DataSourceProperty]
		public MBBindingList<ListButtonVM> Buttons { get; }

		[DataSourceProperty]
		public string Note { get; } = "NOTE: The default titles are only applied to a lord/lady if an entry for their kingdom or culture exists. This is done to avoid unintentionally interacting with total conversion mods.";

		internal SimpleEditConfigEntryPointVM(TitleConfiguration config, ConfigUIBaseVM baseVM) : base(baseVM) {
			_config = config;

			const string editDefaultHint = "Default settings for the ruler title and the clan tier dependent titles. These are applied if the corresponding titles for a character are undefined for both their culture and their kingdom.";

			const string editCultureTitlesHint = "Edit titles based on a character's culture. Titles defined here overwrite default titles, but are overwritten by titles defined for the kingdom a character belongs to.";

			const string editKingdomTitlesHint = "Edit titles based on the kingdom a character belongs to. Titles defined here overwrite default titles and culture dependent titles.";

			Buttons = new MBBindingList<ListButtonVM> {
				new ListButtonVM("Edit Default Titles", editDefaultHint, ExecuteEditDefault),
				new ListButtonVM("Edit Culture Titles", editCultureTitlesHint, ExecuteEditCultureTitles),
				new ListButtonVM("Edit Kingdom Titles", editKingdomTitlesHint, ExecuteEditKingdomTitles)
			};
		}
		public void ExecuteEditDefault() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new SimpleEditTitlesVM(_config.TitlesForLords.TitlesForKingdoms.Default, "Edit Default Titles", BaseVM, this.IsValid, !_config.Options.IsDefault);
			var movie = layer.LoadMovie("CTSimpleEditTitles", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditDefaultTitles;
		}

		public void ExecuteEditKingdomTitles() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new SpecificKingdomPropertiesVM(_config, !_config.Options.IsDefault, this, BaseVM, true);
			var movie = layer.LoadMovie("CTDefaultList", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditKingdomTitles;
		}

		public void ExecuteEditCultureTitles() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new CulturePropertiesVM(_config, !_config.Options.IsDefault, this, BaseVM, true);
			var movie = layer.LoadMovie("CTDefaultList", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditCultureTitles;
		}

		internal override void RestoreNextScreenPostResetAndTransferAdditionalState(SettingsLayerBaseVM newInstanceOfThisVM) {
			if (newInstanceOfThisVM is SimpleEditConfigEntryPointVM newInstance) {
				newInstance._toNextScreen = _toNextScreen;
				switch (_toNextScreen) {
					case Command.None:
						return;
					case Command.EditDefaultTitles:
						newInstance.ExecuteEditDefault();
						return;
					case Command.EditKingdomTitles:
						newInstance.ExecuteEditKingdomTitles();
						return;
					case Command.EditCultureTitles:
						newInstance.ExecuteEditCultureTitles();
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
