using Bannerlord.TitlesForLords.src.main.Core.Settings;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.KingdomsAndCulturesView {
	public class CulturesAndKingdomsForModsVM : SettingsLayerBaseVM {

		SettingsLayerBaseVM _parent;

		internal override string PathDescriptor => "Known Kingdom and Culture names";

		[DataSourceProperty]
		public MBBindingList<KnownCulturesAndKingdomsListPerModVM> ListPerMod { get; }

		public CulturesAndKingdomsForModsVM(SettingsLayerBaseVM parent, ConfigUIBaseVM baseVM) : base(baseVM) {
			_parent = parent;
			ListPerMod = new MBBindingList<KnownCulturesAndKingdomsListPerModVM>();

			var allMods = new HashSet<string>(ModSettings.Instance.SubModuleToKingdoms.Keys
				.Union(ModSettings.Instance.SubModuleToCultures.Keys));
			foreach (var mod in allMods) {
				var kingdomsForMod = ModSettings.Instance.SubModuleToKingdoms.ContainsKey(mod) ?
					ModSettings.Instance.SubModuleToKingdoms[mod] : new HashSet<string>();
				var culturesForMod = ModSettings.Instance.SubModuleToCultures.ContainsKey(mod) ?
					ModSettings.Instance.SubModuleToCultures[mod] : new HashSet<string>();
				ListPerMod.Add(new KnownCulturesAndKingdomsListPerModVM(mod, kingdomsForMod, culturesForMod));
			}
		}

		internal override bool IsValid() {
			return _parent.IsValid();
		}
	}
}
