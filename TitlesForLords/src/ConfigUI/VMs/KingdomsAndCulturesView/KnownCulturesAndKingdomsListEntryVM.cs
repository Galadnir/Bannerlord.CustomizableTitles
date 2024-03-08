using TaleWorlds.Library;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.KingdomsAndCulturesView {

	public class KnownCulturesAndKingdomsListEntryVM : ViewModel {

		[DataSourceProperty]
		public string Mod { get; }

		[DataSourceProperty]
		public string Kingdom { get; }

		[DataSourceProperty]
		public string Culture { get; }

		internal KnownCulturesAndKingdomsListEntryVM(string mod, string kingdom, string culture) {
			this.Mod = mod;
			this.Kingdom = kingdom;
			this.Culture = culture;
		}
	}
}
