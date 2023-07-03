using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs {
	internal class VillagerPropertiesVM : ViewModel {

		readonly VillagerProperties _properties;

		[DataSourceProperty]
		public bool IsEditEnabled { get; }

		[DataSourceProperty]
		public NullableBoolDropdownVM ShowHomeSettlementOfVillagers { get; private set; }
		[DataSourceProperty]
		public NullableBoolDropdownVM ShowOwnerOfVillagers { get; private set; }
		[DataSourceProperty]
		public NullableBoolDropdownVM ShowHomeSettlementBeforeOwner { get; private set; }
		[DataSourceProperty]
		public EditStringBarVM PrefixBar { get; private set; }
		[DataSourceProperty]
		public EditStringBarVM InfixBar { get; private set; }
		[DataSourceProperty]
		public EditStringBarVM PostfixBar { get; private set; }

		internal VillagerPropertiesVM(VillagerProperties properties, bool isEditable) {
			_properties = properties;
			IsEditEnabled = isEditable;
			InitializeSettings();
		}

		private void InitializeSettings() {
			ShowHomeSettlementOfVillagers = new NullableBoolDropdownVM("Show home settlement of villagers", "Whether to include the home settlement of the villager party in their name on the campaign map.", _properties.ShowHomeSettlementOfVillagers,
				value => _properties.ShowHomeSettlementOfVillagers = value, IsEditEnabled);

			ShowOwnerOfVillagers = new NullableBoolDropdownVM("Show owner of villagers", "Whether to include the lord of their home settlement in their name on the campaign map.", _properties.ShowOwnerOfVillagers,
				value => _properties.ShowOwnerOfVillagers = value, IsEditEnabled);

			ShowHomeSettlementBeforeOwner = new NullableBoolDropdownVM("Show home settlement before owner", "Whether to show the home settlement before the owner. The infix is shown inbetween. If no infix is defined, a space is inserted automatically between them. If this is undefined for a villager party, the owner is shown first.", _properties.ShowHomeSettlementBeforeOwner,
				value => _properties.ShowHomeSettlementBeforeOwner = value, IsEditEnabled);

			PrefixBar = new EditStringBarVM(IsEditEnabled, "Villager Party Prefix", "The prefix of a villager party on the campaign map. A space is not automatically inserted after. If 'Villager Party Prefix' and 'Villager Party Postfix' are both undefined for the owning lord of a villager party, the value provided by the game is used.", _properties.VillagerPrefixOnCampaignMap,
				value => _properties.VillagerPrefixOnCampaignMap = value,
				currentValue => !currentValue.EndsWith(" "),
				"Your current prefix does not end with a space, a space is not automatically inserted after.");

			InfixBar = new EditStringBarVM(IsEditEnabled, "Villager Party Infix", "Only used if both 'Show home settlement of villagers' and 'Show owner of villagers' are true, ignored otherwise. The infix shown between the owner and the home settlement. A space is not automatically inserted before or after", _properties.VillagerInfixOnCampaignMap,
				value => _properties.VillagerInfixOnCampaignMap = value,
				currentValue => !currentValue.EndsWith(" ") || !currentValue.StartsWith(" "),
				"Your current infix does not end with a space or does not begin with a space. A space is not automatically inserted before or after");

			PostfixBar = new EditStringBarVM(IsEditEnabled, "Villager Party Postfix", "The postfix of the villager party name on the campaign map. A space is not automatically inserted before. If 'Villager Party Prefix' and 'Villager Party Postfix' are both undefined for the owning lord of a villager party, the value provided by the game is used.", _properties.VillagerPostfixOnCampaignMap,
				value => _properties.VillagerPostfixOnCampaignMap = value,
				currentValue => !currentValue.StartsWith(" "),
				"Your current postfix does not end with a space, a space is not automatically inserted before.");

		}
	}
}
