using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.LordTitlesVMs.TitlesForClansVMs {
	internal class EditIndividualClanPropertiesVM : SettingsLayerBaseVM {

		enum Command { None, ClanPropertiesOpenedScreen, EditForKingdoms }

		Command _toNextScreen;
		Command _toNextScreenBeforeExecuteBack;

		readonly SettingsLayerBaseVM _parent;
		readonly TitleConfiguration _config;
		readonly bool _isEditable;
		readonly string _clanKey;

		TitlesForTContainer<ClanProperties> Clan => _config.TitlesForLords.TitlesForClans.Clans[_clanKey];

		internal override string PathDescriptor => ClanProperties.PathDescriptor;

		[DataSourceProperty]
		public EditClanPropertiesVM ClanProperties { get; }
		[DataSourceProperty]
		public HintViewModel EditForKingdomsHint { get; } = new HintViewModel(new TextObject("Set properties depending on the kingdom this clan currently belongs to. This overwrites properties defined here directly."));

		public EditIndividualClanPropertiesVM(TitleConfiguration config, string clanKey, string pathDescriptor, bool isEditable, SettingsLayerBaseVM parent, ConfigUIBaseVM baseVM) : base(baseVM) {
			_parent = parent;
			_config = config;
			_clanKey = clanKey;
			_isEditable = isEditable;

			ClanProperties = new EditClanPropertiesVM(pathDescriptor, Clan.Default, _isEditable, this.IsValid, this.ClanPropertiesOpenedScreen, BaseVM);
			// this.IsValid as that is passed to the opened screen. Otherwise will become inconsistent, even though it is never directly a top screen
		}

		public void ExecuteEditForKingdoms() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new IndividualClanKingdomPropertiesVM(_config, _clanKey, _isEditable, this, BaseVM);
			var movie = layer.LoadMovie("CTDefaultList", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditForKingdoms;
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

		internal override void RestoreNextScreenPostResetAndTransferAdditionalState(SettingsLayerBaseVM newInstanceOfThisVM) {
			if (newInstanceOfThisVM is EditIndividualClanPropertiesVM newInstance) {
				newInstance._toNextScreen = _toNextScreen;
				switch (_toNextScreen) {
					case Command.None:
						return;
					case Command.ClanPropertiesOpenedScreen:
						ClanProperties.RestoreNextScreenPostResetAndTransferAdditionalState(newInstance.ClanProperties);
						return;
					case Command.EditForKingdoms:
						newInstance.ExecuteEditForKingdoms();
						return;
				}
			}
		}

		internal override bool IsValid() {
			return _parent.IsChildValid();
		}

		private void ClanPropertiesOpenedScreen() {
			_toNextScreen = Command.ClanPropertiesOpenedScreen;
		}
	}
}
