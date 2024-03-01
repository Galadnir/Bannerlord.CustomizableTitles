using Bannerlord.TitleOverhaul.src.ConfigUI;
using System;
using TaleWorlds.Library;

namespace Bannerlord.TitlesForLords.src.ConfigUI.VMs.Common {
	internal class InquiryPopUpVM : ViewModel {

		readonly Action _onAccept;
		readonly Action _onDeny;
		readonly ConfigUIScreen _screen;

		[DataSourceProperty]
		public string Title { get; }

		[DataSourceProperty]
		public string StringValue { get; }
		[DataSourceProperty]
		public string AcceptText { get; }
		[DataSourceProperty]
		public string DenyText { get; }

		internal InquiryPopUpVM(string title, string text, string acceptText, string denyText, Action onAccept, Action onDeny, ConfigUIScreen screen) {
			Title = title;
			StringValue = text;
			AcceptText = acceptText;
			DenyText = denyText;
			_onAccept = onAccept;
			_onDeny = onDeny;
			_screen = screen;
		}

		public void ExecuteAccept() {
			_screen.CloseInquiryPopUp();
			_onAccept();
		}

		public void ExecuteDeny() {
			_screen.CloseInquiryPopUp();
			_onDeny();
		}
	}
}
