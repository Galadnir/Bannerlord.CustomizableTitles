using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common {
	public class EditableTextPopUpVM : ViewModel {

		readonly Action<string> _onConfirmation;
		string _stringValue;
		readonly ConfigUIScreen _screen;

		[DataSourceProperty]
		public string Title { get; }

		[DataSourceProperty]
		public string StringValue {
			get => _stringValue;
			set {
				if (value != _stringValue) {
					_stringValue = value;
					OnPropertyChangedWithValue(value, nameof(StringValue));
				}
			}
		}

		internal EditableTextPopUpVM(string title, Action<string> onConfirmation, ConfigUIScreen screen) {
			this.Title = title;
			_stringValue = string.Empty;
			_onConfirmation = onConfirmation;
			_screen = screen;
		}

		public void ExecuteConfirm() {
			_screen.ClosePopUp();
			_onConfirmation(_stringValue);
		}

		public void ExecuteDiscard() {
			_screen.ClosePopUp();
		}
	}
}
