using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitlesForLords.src.main.Core.Settings;
using System;
using System.Linq;
using TaleWorlds.Library;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigsVM {
	public class TitleConfigurationsVM : SettingsLayerBaseVM {

		TitleConfigEntryVM _nextScreenOpenedBy;
		TitleConfigEntryVM _nextScreenOpenedByBeforeExecuteBack;

		internal override string PathDescriptor => "Title Configurations";

		[DataSourceProperty]
		public MBBindingList<TitleConfigEntryVM> Entries { get; }

		internal TitleConfigEntryVM NextScreenOpenedBy { set => _nextScreenOpenedBy = value; }

		public TitleConfigurationsVM(ConfigUIBaseVM baseVM) : base(baseVM) {
			Entries = new MBBindingList<TitleConfigEntryVM>();
			CreateEntries();
		}

		private void CreateEntries() {
			foreach (var config in ModSettings.Instance.TitleConfigs) {
				Entries.Add(new TitleConfigEntryVM(config, this, BaseVM));
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
