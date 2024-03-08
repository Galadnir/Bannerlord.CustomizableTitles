using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs {
	public class ViewAutomaticRenamesVM : SettingsLayerBaseVM {

		readonly SettingsLayerBaseVM _parent;

		internal override string PathDescriptor => "view automatic renames";

		[DataSourceProperty]
		public MBBindingList<AutomaticRename> AutomaticRenames { get; }

		public ViewAutomaticRenamesVM(IEnumerable<Tuple<string, string>> automaticRenames, SettingsLayerBaseVM parent, ConfigUIBaseVM baseVM) : base(baseVM) {
			_parent = parent;
			AutomaticRenames = new MBBindingList<AutomaticRename>();
			foreach (var rename in automaticRenames) {
				AutomaticRenames.Add(new AutomaticRename(rename));
			}
		}

		internal override bool IsValid() {
			return _parent.IsValid();
		}

		public class AutomaticRename : ViewModel {

			[DataSourceProperty]
			public string Text { get; }

			internal AutomaticRename(Tuple<string, string> rename) {
				Text = $"rules from {rename.Item1} were copied over to {rename.Item2}";
			}
		}
	}
}
