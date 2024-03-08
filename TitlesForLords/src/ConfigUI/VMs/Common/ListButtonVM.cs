using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common {
	public class ListButtonVM : ViewModel {

		readonly Action _onClick;

		[DataSourceProperty]
		public string Text { get; }
		[DataSourceProperty]
		public HintViewModel Hint { get; }

		public ListButtonVM(string text, string hint, Action onClick) {
			Text = text;
			Hint = new HintViewModel(new TextObject(hint));
			_onClick = onClick;
		}

		public void ExecuteOnClick() {
			_onClick();
		}

	}
}
