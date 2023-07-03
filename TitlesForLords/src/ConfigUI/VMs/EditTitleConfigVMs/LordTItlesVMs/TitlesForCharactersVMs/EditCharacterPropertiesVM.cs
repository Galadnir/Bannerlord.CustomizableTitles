using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer;
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

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.LordTitlesVMs.TitlesForCharactersVMs {
	internal class EditCharacterPropertiesVM : SettingsLayerBaseVM {

		enum Command { None, EditForKingdoms, EditForClans }

		Command _toNextScreen;
		Command _toNextScreenBeforeExecuteBack;

		readonly SettingsLayerBaseVM _parent;
		readonly TitleConfiguration _config;
		readonly bool _isEditable;
		readonly string _characterKey;


		IndividualCharacterProperties Character => _config.TitlesForLords.TitlesForCharacters.Characters[_characterKey];

		internal override string PathDescriptor { get; }

		[DataSourceProperty]
		public EditTitlePropertiesVMWidget CharacterProperties { get; }
		[DataSourceProperty]
		public HintViewModel EditForKingdomsHint { get; } = new HintViewModel(new TextObject("Set properties depending on the kingdom this character currently belongs to. This overwrites properties defined here directly, but these are overwritten by properties defined for the character's current clan."));
		[DataSourceProperty]
		public HintViewModel EditForClansHint { get; } = new HintViewModel(new TextObject("Set properties depending on the clan this character currently belongs to. This is only useful if 'track name changes' is enabled. This overwrites properties defined here directly and properties defined for the character's current kingdom."));

		public EditCharacterPropertiesVM(TitleConfiguration config, string characterKey, string pathDescriptor, bool isEditable, SettingsLayerBaseVM parent, ConfigUIBaseVM baseVM) : base(baseVM) {
			_parent = parent;
			_config = config;
			_characterKey = characterKey;
			PathDescriptor = pathDescriptor;
			_isEditable = isEditable;

			CharacterProperties = EditTitlePropertiesVMWidget.CreateFor(Character.Default, _isEditable);
		}

		public void ExecuteEditForKingdoms() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new CharacterKingdomPropertiesVM(_config, _characterKey, _isEditable, this, BaseVM);
			var movie = layer.LoadMovie("CTDefaultList", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditForKingdoms;
		}

		public void ExecuteEditForClans() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new CharacterClanPropertiesVM(_config, _characterKey, _isEditable, this, BaseVM);
			var movie = layer.LoadMovie("CTDefaultList", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditForClans;
		}

		internal override void RestoreNextScreenPostResetAndTransferAdditionalState(SettingsLayerBaseVM newInstanceOfThisVM) {
			if (newInstanceOfThisVM is EditCharacterPropertiesVM newInstance) {
				newInstance._toNextScreen = _toNextScreen;
				switch (_toNextScreen) {
					case Command.None:
						return;
					case Command.EditForKingdoms:
						newInstance.ExecuteEditForKingdoms();
						return;
					case Command.EditForClans:
						newInstance.ExecuteEditForClans();
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
			return _parent.IsChildValid();
		}
	}
}
