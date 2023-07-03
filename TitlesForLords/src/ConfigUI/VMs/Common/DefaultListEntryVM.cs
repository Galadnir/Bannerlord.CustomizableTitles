using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common {
	internal class DefaultListEntryVM : ViewModel {

		readonly DefaultListBaseVM _listBaseVM;

		string _entryKey;

		internal string OriginalKey { get; private set; }

		[DataSourceProperty]
		public bool IsEditEnabled { get; }

		[DataSourceProperty]
		public string EntryKey {
			get => _entryKey;
			set {
				if (_entryKey != value) {
					_entryKey = value;
					OnPropertyChangedWithValue(value, nameof(EntryKey));
				}
			}
		}
		[DataSourceProperty]
		public HintViewModel EntryHint { get; }

		[DataSourceProperty]
		public HintViewModel RenameHint { get; }
		[DataSourceProperty]
		public HintViewModel CopyHint { get; }
		[DataSourceProperty]
		public HintViewModel DeleteHint { get; }

		internal DefaultListEntryVM(bool isEditEnabled, string entryKey, string originalKey, string entryHint, DefaultListBaseVM listBaseVM) {
			IsEditEnabled = isEditEnabled;
			_listBaseVM = listBaseVM;
			EntryKey = entryKey;
			OriginalKey = originalKey;
			EntryHint = new HintViewModel(new TextObject(entryHint));
			RenameHint = new HintViewModel(new TextObject("Change the name of this entry."));
			CopyHint = new HintViewModel(new TextObject("Copy these settings to a new entry with a new name"));
			DeleteHint = new HintViewModel(new TextObject("Delete this entry."));
		}

		public void ExecuteSelect() {
			_listBaseVM.Select(EntryKey, OriginalKey);
		}
		public void ExecuteRename() {
			_listBaseVM.BaseVM.Screen.OpenPopUp(new EditableTextPopUpVM("Enter new name",
				newName => _listBaseVM.ExecuteRename(newName, this),
				_listBaseVM.BaseVM.Screen));
		}
		public void ExecuteCopy() {
			_listBaseVM.BaseVM.Screen.OpenPopUp(new EditableTextPopUpVM("Enter name of copy",
				newName => _listBaseVM.ExecuteCopy(EntryKey, newName),
				_listBaseVM.BaseVM.Screen
			));
		}

		public void ExecuteDelete() {
			_listBaseVM.ExecuteDelete(EntryKey);
		}

		internal void OnAfterSave() {
			OriginalKey = EntryKey;
		}
	}
}
