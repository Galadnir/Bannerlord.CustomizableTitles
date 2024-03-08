using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer;
using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.LordTitlesVMs {
	internal class EditClanPropertiesVM : SettingsLayerBaseVM {

		readonly bool _isEditable;
		readonly Func<bool> _isValid;
		readonly ClanProperties _clanProperties;
		readonly string _pathDescriptorBeginning;
		Action _onNextScreenOpened;

		bool _isAllRankMembersVisible;
		bool _isParent;
		bool _wasParent;

		string EditForAllMembersButtonText = "Edit for all members";
		string EditForLeaderOnlyButtonText = "Edit for leader only";
		HintViewModel EditForAllMembersHint { get; } = new HintViewModel(new TextObject("Edit properties for the all clan members. These also apply to the clan leader, but can be overwritten for the clan leader."));
		HintViewModel EditForClanLeaderOnlyHint { get; } = new HintViewModel(new TextObject("Edit properties for the clan leader only. Properties defined for all members still apply to the clan leader, but can be overwritten by these."));

		RankMemberSelectionVM AllRankMembers { get; }
		RankMemberSelectionVM ClanLeader { get; }

		[DataSourceProperty]
		public RankMemberSelectionVM CurrentlyEditing => _isAllRankMembersVisible ? AllRankMembers : ClanLeader;
		[DataSourceProperty]
		public string CurrentToggleButtonText => _isAllRankMembersVisible ? EditForLeaderOnlyButtonText : EditForAllMembersButtonText;
		[DataSourceProperty]
		public HintViewModel CurrentToggleHint => _isAllRankMembersVisible ? EditForClanLeaderOnlyHint : EditForAllMembersHint;

		internal override string PathDescriptor => _pathDescriptorBeginning + PathDescriptorEnding;

		string PathDescriptorEnding => _isAllRankMembersVisible ? " (all members)" : " (clan leader)";

		internal EditClanPropertiesVM(string pathDescriptorBeginning, ClanProperties clanProperties, bool isEditable, Func<bool> isValid, ConfigUIBaseVM baseVM) : base(baseVM) {
			_isEditable = isEditable;
			_isValid = isValid;
			_clanProperties = clanProperties;
			_isAllRankMembersVisible = true;
			_pathDescriptorBeginning = pathDescriptorBeginning;
			_onNextScreenOpened = () => { };
			AllRankMembers = new RankMemberSelectionVM("", _clanProperties.ClanMemberDefault, _isEditable, this.IsValid, this.OnNextScreenOpened, BaseVM);
			ClanLeader = new RankMemberSelectionVM("", _clanProperties.ClanLeader, _isEditable, this.IsValid, this.OnNextScreenOpened, BaseVM);
			// this.IsValid as that is passed to the opened screen. Otherwise will become inconsistent, even though it is never directly a top screen
		}

		internal EditClanPropertiesVM(string pathDescriptorBeginning, ClanProperties clanProperties, bool isEditable, Func<bool> isValid, Action OnNextScreenOpened, ConfigUIBaseVM baseVM) : this(pathDescriptorBeginning, clanProperties, isEditable, isValid, baseVM) {
			_onNextScreenOpened = OnNextScreenOpened;
		}

		public void ExecuteToggle() {
			_isAllRankMembersVisible = !_isAllRankMembersVisible;
			OnPropertyChanged(nameof(CurrentlyEditing));
			OnPropertyChanged(nameof(CurrentToggleButtonText));
			OnPropertyChanged(nameof(CurrentToggleHint));
			BaseVM.UpdateCurrentPath();
			BaseVM.DisableForwardInHistory();
		}

		internal override void RestoreNextScreenPostResetAndTransferAdditionalState(SettingsLayerBaseVM newInstanceOfThisVM) {
			if (newInstanceOfThisVM is EditClanPropertiesVM newInstance) {
				newInstance._isParent = _isParent;
				if (newInstance._isAllRankMembersVisible != this._isAllRankMembersVisible) {
					newInstance.ExecuteToggle();
				}
				if (_isParent) {
					if (_isAllRankMembersVisible) {
						AllRankMembers.RestoreNextScreenPostResetAndTransferAdditionalState(newInstance.AllRankMembers);
					} else {
						ClanLeader.RestoreNextScreenPostResetAndTransferAdditionalState(newInstance.ClanLeader);
					}
				}
			}
		}

		internal override void OnIsTopLayer() {
			_isParent = false;
			AllRankMembers.OnIsTopLayer();
			ClanLeader.OnIsTopLayer();
		}

		internal override void OnAfterExecuteForwardOnNewTopScreen() {
			_isParent = _wasParent;
			AllRankMembers.OnAfterExecuteForwardOnNewTopScreen();
			ClanLeader.OnAfterExecuteForwardOnNewTopScreen();
		}

		internal override void OnAfterExecuteBackOnNewTopScreen() {
			_wasParent = true;
			AllRankMembers.OnAfterExecuteBackOnNewTopScreen();
			ClanLeader.OnAfterExecuteBackOnNewTopScreen();
		}

		internal override bool IsValid() {
			return _isValid();
		}

		private void OnNextScreenOpened() {
			_isParent = true;
			_onNextScreenOpened();
		}
	}
}
