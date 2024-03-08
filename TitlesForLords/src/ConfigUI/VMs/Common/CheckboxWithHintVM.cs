using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common {
	public class CheckboxWithHintVM : ViewModel {

		bool _boolValue;
		readonly Action<bool> _onValueChanged;

		[DataSourceProperty]
		public bool IsSettingVisible { get => true; } // for used MCM property

		[DataSourceProperty]
		public bool IsBool { get => true; } // for used MCM property

		[DataSourceProperty]
		public bool IsEnabled { // if the checkbox is enabled; for used MCM property
			get => true;
		}
		[DataSourceProperty]
		public bool BoolValue {
			get => _boolValue;
			set {
				if (_boolValue != value) {
					_boolValue = value;
					_onValueChanged(value);
					OnPropertyChangedWithValue(value, nameof(BoolValue));
				}
			}
		}
		[DataSourceProperty]
		public string SettingName { get; }
		[DataSourceProperty]
		public HintViewModel Hint { get; }

		public CheckboxWithHintVM(string settingName, bool boolValue, string hintText, Action<bool> onValueChanged) {
			SettingName = settingName;
			_boolValue = boolValue;
			Hint = new HintViewModel(new TextObject(hintText));
			_onValueChanged = onValueChanged;
		}
	}
}
