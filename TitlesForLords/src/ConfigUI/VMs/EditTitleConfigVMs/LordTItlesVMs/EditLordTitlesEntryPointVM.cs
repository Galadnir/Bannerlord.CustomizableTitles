using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.LordTitlesVMs.TitlesForCharactersVMs;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.LordTitlesVMs.TitlesForClansVMs;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.LordTitlesVMs.TitlesForKingdomsVMs;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.NotableOwnedCaravansVMs;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.LordTitlesVMs {
	internal class EditLordTitlesEntryPointVM : SettingsLayerBaseVM {

		enum Command { None, EditDefault, EditTitlesForKingdoms, EditTitlesForClans, EditTitlesForCharacters }

		Command _toNextScreen;
		Command _toNextScreenBeforeExecuteBack;

		readonly SettingsLayerBaseVM _parent;
		readonly TitleConfiguration _config;
		readonly bool _isEditable;

		internal override string PathDescriptor => "Lord titles";

		[DataSourceProperty]
		public MBBindingList<ListButtonVM> Buttons { get; }

		internal EditLordTitlesEntryPointVM(TitleConfiguration config, bool isEditable, SettingsLayerBaseVM parent, ConfigUIBaseVM baseVM) : base(baseVM) {
			_parent = parent;
			_config = config;
			_isEditable = isEditable;
			const string editDefaultHint = "Default properties to apply to lords. To avoid conflicting with conversion mods, these are only applied once a kingdom, culture, clan or name match for the lord is found in this configuration.\n\nProperties defined in here overwrite undefined properties in the other categories, but are overwritten by defined properties in the other categories.";
			const string editTitlesForKingdomsHint = "Properties to apply to lords depending on their kingdom or culture. Properties defined in here are overwritten by properties defined for the clan of the lord or directly for the lord.";
			const string editTitlesForClansHint = "Properties to apply to a certain clan. Properties defined in here overwrite properties defined for kingdoms and cultures, but are overwritten by properties defined for a specific lord.";
			const string editTitlesForCharactersHint = "Properties to apply to a specific lord. Properties defined in here overwrite all other properties.";

			Buttons = new MBBindingList<ListButtonVM> {
				new ListButtonVM("Edit default", editDefaultHint, ExecuteEditDefault),
				new ListButtonVM("Edit titles for kingdoms", editTitlesForKingdomsHint, ExecuteEditTitlesForKingdoms),
				new ListButtonVM("Edit titles for clans", editTitlesForClansHint, ExecuteEditTitlesForClans),
				new ListButtonVM("Edit titles for specific lords", editTitlesForCharactersHint, ExecuteEditTitlesForCharacters)
			};
		}

		public void ExecuteEditDefault() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new RankMemberSelectionVM("Edit default properties", _config.TitlesForLords.Default, _isEditable, this.IsValid, BaseVM);
			var movie = layer.LoadMovie("CTButtonList", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditDefault;
		}

		public void ExecuteEditTitlesForKingdoms() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditTitlesForKingdomsEntryPoint(_config, _isEditable, this, BaseVM);
			var movie = layer.LoadMovie("CTButtonList", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditTitlesForKingdoms;
		}

		public void ExecuteEditTitlesForClans() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditTitlesForClansListVM(_config, _isEditable, this, BaseVM);
			var movie = layer.LoadMovie("CTDefaultList", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditTitlesForClans;
		}

		public void ExecuteEditTitlesForCharacters() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditTitlesForCharactersListVM(_config, _isEditable, this, BaseVM);
			var movie = layer.LoadMovie("CTDefaultList", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditTitlesForCharacters;
		}

		internal override void RestoreNextScreenPostResetAndTransferAdditionalState(SettingsLayerBaseVM newInstanceOfThisVM) {
			if (newInstanceOfThisVM is EditLordTitlesEntryPointVM newInstance) {
				newInstance._toNextScreen = _toNextScreen;
				switch (_toNextScreen) {
					case Command.None:
						return;
					case Command.EditDefault:
						newInstance.ExecuteEditDefault();
						return;
					case Command.EditTitlesForKingdoms:
						newInstance.ExecuteEditTitlesForKingdoms();
						return;
					case Command.EditTitlesForClans:
						newInstance.ExecuteEditTitlesForClans();
						return;
					case Command.EditTitlesForCharacters:
						newInstance.ExecuteEditTitlesForCharacters();
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
