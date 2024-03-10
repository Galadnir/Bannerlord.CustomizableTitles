using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitlesForLords.src.main.Core.Settings;
using System;
using System.Linq;
using TaleWorlds.Library;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigsVM {
	public class TitleConfigurationsVM : SettingsLayerBaseVM {

		bool _isEditSimple;
		
		TitleConfigEntryVM _nextScreenOpenedBy;
		TitleConfigEntryVM _nextScreenOpenedByBeforeExecuteBack;

		internal override string PathDescriptor => "Title Configurations" + (_isEditSimple ? " (Basic)" : " (Expert)");

		[DataSourceProperty]
		public MBBindingList<TitleConfigEntryVM> Entries { get; }

		internal TitleConfigEntryVM NextScreenOpenedBy { set => _nextScreenOpenedBy = value; }

		public TitleConfigurationsVM(ConfigUIBaseVM baseVM, bool isEditSimple) : base(baseVM) {
			_isEditSimple = isEditSimple;
			Entries = new MBBindingList<TitleConfigEntryVM>();
			CreateEntries();
		}

		private void CreateEntries() {
			foreach (var config in ModSettings.Instance.TitleConfigs) {
				Entries.Add(new TitleConfigEntryVM(config, this, BaseVM, _isEditSimple));
			}
			OnPropertyChanged(nameof(Entries));
		}

		public void ExecuteCreateConfig() {
			ConfigUIScreen screen = BaseVM.Screen;
			screen.OpenPopUp(new EditableTextPopUpVM("Enter name of config",
				value => {
					ModSettings.Instance.CreateNewConfig(Guid.NewGuid().ToString(), value);
					RefreshValues();
				},
				screen));
		}

		public override void RefreshValues() {
			base.RefreshValues();
			Entries.Clear();
			CreateEntries();

		}

		internal override void RestoreNextScreenPostResetAndTransferAdditionalState(SettingsLayerBaseVM newInstanceOfThisVM) {
			if (newInstanceOfThisVM is TitleConfigurationsVM newInstance) {
				newInstance._nextScreenOpenedBy = _nextScreenOpenedBy;
				var toOpenNextScreen = newInstance.Entries.FirstOrDefault(entry => entry.Config == _nextScreenOpenedBy?.Config);
				toOpenNextScreen?.ExecuteSelect();
			}
		}

		internal override void OnIsTopLayer() {
			_nextScreenOpenedBy = null;
		}

		internal override void OnAfterExecuteForwardOnNewTopScreen() {
			_nextScreenOpenedBy = _nextScreenOpenedByBeforeExecuteBack;
		}

		internal override void OnAfterExecuteBackOnNewTopScreen() {
			_nextScreenOpenedByBeforeExecuteBack = _nextScreenOpenedBy;
		}

		internal override bool IsValid() {
			return true;
		}
	}
}
