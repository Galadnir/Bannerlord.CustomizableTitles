using System;
using System.Collections.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common {
	public class NullableBoolDropdownVM : ViewModel {

		readonly bool? _startingValue;
		readonly Action<bool?> _onChange;

		[DataSourceProperty]
		public bool IsEnabled { get; }
		[DataSourceProperty]
		public string SettingName { get; }
		[DataSourceProperty]
		public HintViewModel Hint { get; }

		[DataSourceProperty]
		public SelectorVM<SelectorItemVM> Dropdown { get; set; }

		public NullableBoolDropdownVM(string settingName, string settingHint, bool? startingValue, Action<bool?> onChange, bool isEnabled) {
			IsEnabled = isEnabled;
			_startingValue = startingValue;
			_onChange = onChange;
			SettingName = settingName;
			Hint = new HintViewModel(new TextObject(settingHint));
			Dropdown = new SelectorVM<SelectorItemVM>(new List<string>() {
				"undefined", "true", "false"
				}, ConvertBoolToIndex(_startingValue), newValue => _onChange(ConvertIndexToBool(newValue.SelectedIndex)));
		}

		public void Reset() {
			Dropdown.SelectedIndex = ConvertBoolToIndex(_startingValue);
		}

		private bool? ConvertIndexToBool(int index) {
			switch (index) {
				case 0:
					return null;
				case 1:
					return true;
				case 2:
					return false;
				default:
					return _startingValue; // should never be reached
			}
		}
		
		private int ConvertBoolToIndex(bool? value) {
			switch (value) {
				case null:
					return 0;
				case true:
					return 1;
				case false:
					return 2;
				default:
					return 0; // never reached
			}
		}
	}
}
