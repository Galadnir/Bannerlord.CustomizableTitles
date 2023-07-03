using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.LordTitlesVMs {
	internal class EditTitlePropertiesVM : SettingsLayerBaseVM {

		readonly Func<bool> _isValid;

		internal override string PathDescriptor { get; }

		[DataSourceProperty]
		public EditTitlePropertiesVMWidget TitlePropertiesWidget { get; }

		public EditTitlePropertiesVM(EditTitlePropertiesVMWidget titlePropertieWidget, string pathDescriptor, ConfigUIBaseVM baseVM, Func<bool> isValid) : base(baseVM) {
			TitlePropertiesWidget = titlePropertieWidget;
			PathDescriptor = pathDescriptor;
			_isValid = isValid;
		}

		internal override bool IsValid() {
			return _isValid();
		}
	}
}
