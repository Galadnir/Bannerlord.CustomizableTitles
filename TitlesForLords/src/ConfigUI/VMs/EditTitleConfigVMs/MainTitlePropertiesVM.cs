using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements;
using TaleWorlds.Library;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs {
	internal class MainTitlePropertiesVM : ViewModel {

		readonly TitleProperties _properties;

		[DataSourceProperty]
		public bool IsEditEnabled { get; }
		[DataSourceProperty]
		public NullableBoolDropdownVM OnlyApplyToClanLeader { get; private set; }
		[DataSourceProperty]
		public NullableBoolDropdownVM ApplyToMercenaries { get; private set; }
		[DataSourceProperty]
		public EditStringBarVM TitleBeforeName { get; private set; }
		[DataSourceProperty]
		public EditStringBarVM TitleAfterName { get; private set; }
		[DataSourceProperty]
		public EditStringBarVM NamePrefixToRemove { get; private set; }
		[DataSourceProperty]
		public EditStringBarVM NamePostfixToRemove { get; private set; }
		[DataSourceProperty]
		public EditStringBarVM ChildTitleBeforeName { get; private set; }
		[DataSourceProperty]
		public EditStringBarVM ChildTitleAfterName { get; private set; }
		[DataSourceProperty]
		public EditStringBarVM PartyPrefixOnCampaignMap { get; private set; }
		[DataSourceProperty]
		public EditStringBarVM PartyPostfixOnCampaignMap { get; private set; }
		[DataSourceProperty]
		public EditStringBarVM ArmyPrefixOnCampaignMap { get; private set; }
		[DataSourceProperty]
		public EditStringBarVM ArmyPostfixOnCampaignMap { get; private set; }

		internal MainTitlePropertiesVM(TitleProperties properties, bool isEditable) {
			_properties = properties;
			IsEditEnabled = isEditable;
			InitializeSettings();
		}

		private void InitializeSettings() {
			OnlyApplyToClanLeader = new NullableBoolDropdownVM("Only apply to clan leader", "Whether to apply these properties only to the clan leader. If this is undefiened for a lord, the value 'false' is used.", _properties.OnlyApplyToClanLeader,
				value => _properties.OnlyApplyToClanLeader = value,
				IsEditEnabled);

			ApplyToMercenaries = new NullableBoolDropdownVM("Apply To Mercenaries", "Whether to apply these properties to mercenaries. If this is undefined for a lord (includes mercenaries) the value 'false' is used.", _properties.ApplyToMercenaries,
				value => _properties.ApplyToMercenaries = value, IsEditEnabled);

			TitleBeforeName = new EditStringBarVM(IsEditEnabled, "Title (before name)", "The title of the lord before his name. A space is not automatically inserted after. If 'Title (before name)' and 'Title (after name)' are both undefined for a character, the value provided by the game is used.", _properties.TitleBeforeName,
				value => _properties.TitleBeforeName = value,
				currentValue => !currentValue.EndsWith(" "), "Your current title does not end with a space. A space is not automatically inserted after.");

			TitleAfterName = new EditStringBarVM(IsEditEnabled, "Title (after name)", "The title of the lord after his name. A space is not automatically inserted before. If 'Title (before name)' and 'Title (after name)' are both undefined for a character, the value provided by the game is used.", _properties.TitleAfterName,
				value => _properties.TitleAfterName = value,
				currentValue => !currentValue.StartsWith(" "), "Your current title does not start with a space. A space is not automatically inserted before.");

			NamePrefixToRemove = new EditStringBarVM(IsEditEnabled, "Name Prefix to Remove", "The prefix of a hero's name to remove, e.g. \"woodlands ergeon\" to \"ergeon\" when this is set to \"woodlands \". Note the space in the previous example, spaces following the prefix are not automatically removed. Mainly intended to remove base game name attachments of mercenary clans.", _properties.NamePrefixToRemove,
				value => _properties.NamePrefixToRemove = value,
				currentValue => !currentValue.EndsWith(" "), "Your current prefix does not end with a space. Spaces following your prefix are not automatically removed.");

			NamePostfixToRemove = new EditStringBarVM(IsEditEnabled, "Name Postfix to Remove", "The postfix of a hero's name to remove, e.g. \"ergeon of the woods\" to \"ergeon\" when this is set to \" of the woods\". Note the space in the previous example, spaces preceding the postfix are not automatically removed. Mainly intended to remove base game name attachments of mercenary clans.", _properties.NamePostfixToRemove,
				value => _properties.NamePostfixToRemove = value,
				currentValue => !currentValue.StartsWith(" "), "Your current postfix does not start with a space. Spaces preciding your postfix are not automatically removed.");

			ChildTitleBeforeName = new EditStringBarVM(IsEditEnabled, "Child Title (before name)", "The title of the lord if he/she is a child before his name. A space is not automatically inserted after. If 'Child Title (before name)' and 'Child Title (after name)' are both undefined for a character that is a child, the value provided by the game is used.", _properties.ChildTitleBeforeName,
				value => _properties.ChildTitleBeforeName = value,
				currentValue => !currentValue.EndsWith(" "), "Your current title does not end with a space. A space is not automatically inserted after.");

			ChildTitleAfterName = new EditStringBarVM(IsEditEnabled, "Child Title (after name)", "The title of the lord after his name if he/she is a child. A space is not automatically inserted before. If 'Child Title (before name)' and 'Child Title (after name)' are both undefined for a character that is a child, the value provided by the game is used.", _properties.ChildTitleAfterName,
				value => _properties.ChildTitleAfterName = value,
				currentValue => !currentValue.StartsWith(" "), "Your current title does not start with a space. A space is not automatically inserted before.");

			PartyPrefixOnCampaignMap = new EditStringBarVM(IsEditEnabled, "Party prefix", "The prefix of the lord's party on the campaign map. A space is not automatically inserted after. If 'Party prefix' and 'Party postfix' are both undefined for a party, the value provided by the game is used.", _properties.PartyPrefixOnCampaignMap,
				value => _properties.PartyPrefixOnCampaignMap = value,
				currentValue => !currentValue.EndsWith(" "), "Your current prefix does not end with a space. A space is not automatically inserted after.");

			PartyPostfixOnCampaignMap = new EditStringBarVM(IsEditEnabled, "Party postfix", "The postfix of the lord's party on the campaign map. A space is not automatically inserted before. If 'Party prefix' and 'Party postfix' are both undefined for a party, the value provided by the game is used.", _properties.PartyPostfixOnCampaignMap,
				value => _properties.PartyPostfixOnCampaignMap = value,
				currentValue => !currentValue.StartsWith(" "), "Your current postfix does not start with a space. A space is not automatically inserted before.");

			ArmyPrefixOnCampaignMap = new EditStringBarVM(IsEditEnabled, "Army prefix", "The prefix of an army led by this lord on the campaign map. A space is not automatically inserted after. If 'Army prefix' and 'Army postfix' are both undefined for an army leader, the value provided by the game is used.", _properties.ArmyPrefixOnCampaignMap,
				value => _properties.ArmyPrefixOnCampaignMap = value,
				currentValue => !currentValue.EndsWith(" "), "Your current prefix does not end with a space. A space is not automatically inserted after.");

			ArmyPostfixOnCampaignMap = new EditStringBarVM(IsEditEnabled, "Army postfix", "The postfix of an army led by this lord on the campaign map. A space is not automatically inserted before. If 'Army prefix' and 'Army postfix' are both undefined for an army leader, the value provided by the game is used.", _properties.ArmyPostfixOnCampaignMap,
				value => _properties.ArmyPostfixOnCampaignMap = value,
				currentValue => !currentValue.StartsWith(" "), "Your current postfix does not start with a space. A space is not automatically inserted before.");
		}
	}
}
