using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.TitleConfigsVM {
	internal class EditCaravanPropertiesVM : SettingsLayerBaseVM {
		readonly SettingsLayerBaseVM _parent;

		internal override string PathDescriptor { get; }

		[DataSourceProperty]
		public CaravanPropertiesVM Properties { get; }


		public EditCaravanPropertiesVM(CaravanPropertiesVM properties, string pathDescriptor, SettingsLayerBaseVM parent, ConfigUIBaseVM baseVM) : base(baseVM) {
			Properties = properties;
			_parent = parent;
			PathDescriptor = pathDescriptor;
		}

		internal override bool IsValid() {
			return _parent.IsChildValid();
		}
	}
}
