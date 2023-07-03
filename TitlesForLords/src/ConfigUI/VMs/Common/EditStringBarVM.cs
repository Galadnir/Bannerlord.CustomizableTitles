using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.TwoDimension;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common {
	public class EditStringBarVM : ViewModel {

		string _currentValue;
		string _stringValue;
		bool _isUndefined;
		bool _isWarningEnabled;
		readonly Action<string> _onChange;
		readonly Func<string, bool> _warningCondition;

		[DataSourceProperty]
		public bool IsEnabled { get; }

		[DataSourceProperty]
		public string SettingName { get; }
		[DataSourceProperty]
		public bool IsUndefined {
			get => _isUndefined;
			private set {
				if (value != _isUndefined) {
					_isUndefined = value;
					OnPropertyChangedWithValue(value, nameof(IsUndefined));
				}
			}
		}
		[DataSourceProperty]
		public bool IsWarningEnabled {
			get => _isWarningEnabled;
			private set {
				if (value != _isWarningEnabled) {
					_isWarningEnabled = value;
					OnPropertyChangedWithValue(value, nameof(IsWarningEnabled));
				}
			}
		}
		[DataSourceProperty]
		public string StringValue {
			get => _stringValue;
			set {
				if (value == _stringValue && value == _currentValue) {
					return;
				}
				_stringValue = value;
				_currentValue = value;
				IsUndefined = _currentValue is null;
				ManageWarning();
				_onChange(_currentValue);
				OnPropertyChangedWithValue(value, nameof(StringValue));

			}
		}
		[DataSourceProperty]
		public HintViewModel SettingHint { get; }
		[DataSourceProperty]
		public HintViewModel SetEmptyHint { get; }
		[DataSourceProperty]
		public HintViewModel UndefineHint { get; }
		[DataSourceProperty]
		public HintViewModel WarningHint { get; }

		public EditStringBarVM(bool isEnabled, string settingName, string settingHint, string currentValue, Action<string> onChange, Func<string, bool> warningCondition, string warningHint) {
			IsEnabled = isEnabled;
			SettingName = settingName;
			_onChange = onChange;
			_warningCondition = warningCondition;
			SettingHint = new HintViewModel(new TextObject(settingHint));
			WarningHint = new HintViewModel(new TextObject(warningHint));
			_currentValue = currentValue;
			_stringValue = _currentValue;
			if (currentValue is null) {
				IsUndefined = true;
			}
			SetEmptyHint = new HintViewModel(new TextObject("Sets this property to be empty. This is different from an undefined property. An undefined property is overwritten by more general settings. This overwrites more general settings with an empty value. This can be used, to reset default values for subsections."));
			UndefineHint = new HintViewModel(new TextObject("Sets this property to be undefined. An undefined property is overwritten by more general properties that apply to a character."));
			ManageWarning();
		}

		public void ExecuteSetEmpty() {
			StringValue = "";
		}

		public void ExecuteUndefine() {
			_stringValue = "";
			OnPropertyChangedWithValue(_stringValue, nameof(StringValue));
			// using StringValue = null causes an immediate callback which ends up setting _currentValue to "". Therefore changed to the correct value afterwards
			// also doesn't change when only using OnPropertyChanged instead of OnPropertyChangedWithValue in setter
			_currentValue = null;
			IsUndefined = true;
			IsWarningEnabled = false;
			_onChange(_currentValue);
		}

		private void ManageWarning() {
			if (!string.IsNullOrEmpty(_currentValue)) {
				IsWarningEnabled = _warningCondition(_currentValue);
			} else {
				IsWarningEnabled = false;
			}
		}

	}
}
