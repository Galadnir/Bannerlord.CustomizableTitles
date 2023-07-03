using HarmonyLib;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.ViewModelCollection.Conversation;

namespace Bannerlord.TitlesForLords.src.main.Core.GamePatches {

	[HarmonyPatch(typeof(MissionConversationVM))]
	[HarmonyPatch(nameof(MissionConversationVM.Refresh))] // this no longer worked using the getter for some reason, therefore now i actually change the field after the VM refreshes
	public static class ConversationNameLabelPatch {

		public static bool isActive = true;

		public static void Postfix(MissionConversationVM __instance, ConversationManager ____conversationManager) {
			if (isActive && !(____conversationManager.OneToOneConversationHero is null)) {
				__instance.CurrentCharacterNameLbl = ____conversationManager.OneToOneConversationHero.Name.ToString();
			}
		}
	}
}
