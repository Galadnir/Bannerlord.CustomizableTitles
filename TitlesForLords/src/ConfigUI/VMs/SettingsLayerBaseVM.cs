using Bannerlord.TitlesForLords.src.main.Core.Settings;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs {
	public abstract class SettingsLayerBaseVM : ViewModel {

		internal ConfigUIBaseVM BaseVM { get; }

		internal abstract string PathDescriptor { get; }

		private protected IList<TitleConfiguration> ModTitleConfigs => ModSettings.Instance.TitleConfigs;

		protected SettingsLayerBaseVM(ConfigUIBaseVM baseVM) {
			this.BaseVM = baseVM;
		}

		public void ExecuteCancel() {
			BaseVM.ExecuteCancel();
		}

		public void ExecuteDone() {
			BaseVM.ExecuteDone();
		}

		internal virtual void RestoreNextScreenPostResetAndTransferAdditionalState(SettingsLayerBaseVM newInstanceOfThisVM) { }

		internal virtual void OnIsTopLayer() { }
		internal virtual void OnAfterExecuteForwardOnNewTopScreen() { }
		internal virtual void OnAfterExecuteBackOnNewTopScreen() { } // but still called before OnIsTopLayer

		internal abstract bool IsValid();
		internal virtual bool IsChildValid() { // for cases where the child can't determine itself whether it's valid, it's possible that this returns true while IsValid returns false for the child, because the child can determine that itself
			return IsValid();
		}

		internal virtual void OnAfterSave() {
		}
	}
}
