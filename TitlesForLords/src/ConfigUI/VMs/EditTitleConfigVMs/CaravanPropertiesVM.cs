using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs {
	internal class CaravanPropertiesVM : ViewModel {

		readonly CaravanProperties _properties;

		[DataSourceProperty]
		public bool IsEditEnabled { get; }

		[DataSourceProperty]
		public NullableBoolDropdownVM ShowNameOfOwner { get; private set; }
		[DataSourceProperty]
		public EditStringBarVM PrefixBar { get; private set; }
		[DataSourceProperty]
		public EditStringBarVM PostfixBar { get; private set; }

		internal CaravanPropertiesVM(CaravanProperties properties, bool isEditable) {
			_properties = properties;
			IsEditEnabled = isEditable;
			InitializeSettings();
		}

		private void InitializeSettings() {
			ShowNameOfOwner = new NullableBoolDropdownVM("Show name of owner", "Whether to show the name of the caravan owner inbetween the prefix and postfix", _properties.ShowNameOfOwner,
				value => _properties.ShowNameOfOwner = value, IsEditEnabled);

			PrefixBar = new EditStringBarVM(IsEditEnabled, "Caravan Prefix", "Set the prefix of a caravan party on the Campaign Map. A space is not automatically inserted after. If 'Caravan Prefix' and 'Caravan Postfix' are both undefined for the owner of the caravan, the value provided by the game is used.", _properties.CaravanPrefixOnCampaignMap,
				value => _properties.CaravanPrefixOnCampaignMap = value,
				stringValue => !stringValue.EndsWith(" "),
				"Warning: Your prefix doesn't end with a space. A space is not automatically inserted after the prefix");

			PostfixBar = new EditStringBarVM(IsEditEnabled, "Caravan Postfix", "Set the postfix of a caravan party on the Campaign Map. A space is not automatically inserted before. If 'Caravan Prefix' and 'Caravan Postfix' are both undefined for the owner of the caravan, the value provided by the game is used.", _properties.CaravanPostfixOnCampaignMap,
				value => _properties.CaravanPostfixOnCampaignMap = value,
				stringValue => !stringValue.StartsWith(" "),
				"Warning: Your postfix doesn't start with a space. A space is not automatically inserted before the postfix");
		}
	}
}
