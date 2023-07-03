using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.LordTitlesVMs {
	internal class RankMemberSelectionVM : SettingsLayerBaseVM {
		enum Command { None, EditDefault, EditMale, EditFemale }

		Command _toNextScreen;
		Command _toNextScreenBeforeExecuteBack;

		readonly RankMember _rankMemberSettings;
		readonly bool _isEditable;
		readonly Func<bool> _isValid;
		readonly Action _onNextScreenOpened; // so that clanProperties knows when a screen was opened, because EditClanPropertiesVM embeds this

		internal override string PathDescriptor { get; }

		[DataSourceProperty]
		public MBBindingList<ListButtonVM> Buttons { get; }


		internal RankMemberSelectionVM(string pathDescriptor, RankMember rankMemberSettings, bool isEditable, Func<bool> isValid, ConfigUIBaseVM baseVM) : base(baseVM) {
			_rankMemberSettings = rankMemberSettings;
			_isEditable = isEditable;
			PathDescriptor = pathDescriptor;
			_isValid = isValid;
			_onNextScreenOpened = () => { };
			const string editDefaultHint = "Edit default properties. Overwrite undefined properties in male and in female";
			const string editMaleHint = "Title properties to apply to male lords. Overwrites properties defined in 'Edit default titles'";
			const string editFemaleHint = "Title properties to apply to female lords. Overwrites properties defined in 'Edit default titles'";

			Buttons = new MBBindingList<ListButtonVM> {
				new ListButtonVM("Edit default title properties", editDefaultHint, ExecuteEditDefault),
				new ListButtonVM("Edit title properties for male lords", editMaleHint, ExecuteEditMale),
				new ListButtonVM("Edit title properties for female lords", editFemaleHint, ExecuteEditFemale),
			};
		}

		internal RankMemberSelectionVM(string pathDescriptor, RankMember rankMemberSettings, bool isEditable, Func<bool> isValid, Action onNextScreenOpened, ConfigUIBaseVM baseVM) : this(pathDescriptor, rankMemberSettings, isEditable, isValid, baseVM) {
			_onNextScreenOpened = onNextScreenOpened;
		}

		public void ExecuteEditDefault() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditTitlePropertiesVM(EditTitlePropertiesVMWidget.CreateFor(_rankMemberSettings.Default, _isEditable),
				"default title properties", BaseVM, this.IsChildValid);
			var movie = layer.LoadMovie("CTEditTitleProperties", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditDefault;
			_onNextScreenOpened();
		}

		public void ExecuteEditMale() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditTitlePropertiesVM(EditTitlePropertiesVMWidget.CreateFor(_rankMemberSettings.Male, _isEditable),
				"male properties", BaseVM, this.IsChildValid);
			var movie = layer.LoadMovie("CTEditTitleProperties", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditMale;
			_onNextScreenOpened();

		}

		public void ExecuteEditFemale() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditTitlePropertiesVM(EditTitlePropertiesVMWidget.CreateFor(_rankMemberSettings.Female, _isEditable),
				"female properties", BaseVM, this.IsChildValid);
			var movie = layer.LoadMovie("CTEditTitleProperties", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			_toNextScreen = Command.EditFemale;
			_onNextScreenOpened();

		}

		internal override void RestoreNextScreenPostResetAndTransferAdditionalState(SettingsLayerBaseVM newInstanceOfThisVM) {
			if (newInstanceOfThisVM is RankMemberSelectionVM newInstance) {
				newInstance._toNextScreen = _toNextScreen;
				switch (_toNextScreen) {
					case Command.None:
						return;
					case Command.EditDefault:
						newInstance.ExecuteEditDefault();
						return;
					case Command.EditMale:
						newInstance.ExecuteEditMale();
						return;
					case Command.EditFemale:
						newInstance.ExecuteEditFemale();
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
			return _isValid();
		}
	}
}
