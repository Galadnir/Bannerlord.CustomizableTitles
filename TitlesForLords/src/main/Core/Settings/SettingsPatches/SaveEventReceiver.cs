using Bannerlord.TitleOverhaul.src.main.Core.GamePatches;
using Bannerlord.TitlesForLords.src.main.Core.Settings;
using TaleWorlds.CampaignSystem;

namespace Bannerlord.TitleOverhaul.src.main.Core.Settings.SettingsPatches {
	internal class SaveEventReceiver : CampaignEventReceiver {

		internal bool SaveOnNextInGameSave { get; set; } = false;

		public override void OnBeforeSave() {
			GamePatchesHelper.DisableAllGetterPatches();
		}

		public override void OnSaveOver(bool isSuccessful, string saveName) {
			if (isSuccessful && SaveOnNextInGameSave) {
				ModSettings.Instance.Save();
				SaveOnNextInGameSave = false;
			}
			GamePatchesHelper.ActivateAllGetterPatches();
		}
	}
}
