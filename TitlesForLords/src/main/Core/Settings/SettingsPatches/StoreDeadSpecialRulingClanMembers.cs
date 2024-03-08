using Bannerlord.TitlesForLords.src.main.Core.Settings;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace Bannerlord.TitleOverhaul.src.main.Core.Settings.SettingsPatches {
	internal class StoreDeadSpecialRulingClanMembers : CampaignEventReceiver {

		public override void OnBeforeHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true) {
			base.OnBeforeHeroKilled(victim, killer, detail, showNotification);
			if (victim.Clan?.Kingdom?.RulingClan is null || victim.Clan?.Equals(victim.Clan?.Kingdom?.RulingClan) == false) {
				return;
			}
			Hero ruler = victim.Clan.Kingdom.RulingClan.Leader;
			if (victim.Equals(ruler)) {
				ModSettings.Instance.RulingClanMemberDied(Campaign.Current.UniqueGameId, victim, RulingClanPossibility.Ruler);
			} else if (victim.Equals(ruler.Spouse)) {
				ModSettings.Instance.RulingClanMemberDied(Campaign.Current.UniqueGameId, victim, RulingClanPossibility.SpouseOfRuler);
			} else if (ruler.Children?.Contains(victim) == true) {
				ModSettings.Instance.RulingClanMemberDied(Campaign.Current.UniqueGameId, victim, RulingClanPossibility.ChildOfRuler);
			} else {
				ModSettings.Instance.RulingClanMemberDied(Campaign.Current.UniqueGameId, victim, RulingClanPossibility.Member);
			}
		}
	}
}
