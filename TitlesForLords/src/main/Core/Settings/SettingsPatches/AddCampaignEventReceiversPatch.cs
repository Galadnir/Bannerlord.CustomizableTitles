using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace Bannerlord.TitleOverhaul.src.main.Core.Settings.SettingsPatches {

	[HarmonyPatch(typeof(Campaign))]
	[HarmonyPatch("OnInitialize")]
	internal static class AddCampaignEventReceiversPatch {

		internal static SaveEventReceiver CurrentReceiver { get; private set; } = new SaveEventReceiver();

		public static void Postfix() {
			if (!(Campaign.Current is null)) {
				var newReceiver = new SaveEventReceiver();
				Campaign.Current.AddCampaignEventReceiver(newReceiver);
				CurrentReceiver.SaveOnNextInGameSave = false; // in case these still exist if the same save is loaded again without closing the game
				CurrentReceiver = newReceiver;
				Campaign.Current.AddCampaignEventReceiver(new StoreDeadSpecialRulingClanMembers());
			}
		}

	}
}
