using System.Collections.Generic;
using TaleWorlds.Library;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.KingdomsAndCulturesView {
	public class KnownCulturesAndKingdomsListPerModVM : ViewModel {

		[DataSourceProperty]
		public MBBindingList<KnownCulturesAndKingdomsListEntryVM> Entries { get; }

		public KnownCulturesAndKingdomsListPerModVM(string mod, IEnumerable<string> kingdomsForMod, IEnumerable<string> culturesForMod) {
			Entries = new MBBindingList<KnownCulturesAndKingdomsListEntryVM>();

			var sortedKingdomsForMod = new List<string>(kingdomsForMod);
			sortedKingdomsForMod.Sort();
			var sortedCulturesForMod = new List<string>(culturesForMod);
			sortedCulturesForMod.Sort();
			EqualizeLength(sortedKingdomsForMod, sortedCulturesForMod);

			Entries.Add(new KnownCulturesAndKingdomsListEntryVM(mod, sortedKingdomsForMod[0], sortedCulturesForMod[0]));
			for (int i = 1; i <  sortedKingdomsForMod.Count; i++) {
				Entries.Add(new KnownCulturesAndKingdomsListEntryVM(string.Empty, sortedKingdomsForMod[i], sortedCulturesForMod[i]));
			}
		}

		private void EqualizeLength(List<string> sortedKingdomsForMod, List<string> sortedCulturesForMod) {
			while (sortedKingdomsForMod.Count < sortedCulturesForMod.Count) {
				sortedKingdomsForMod.Add(string.Empty);
			}
			while (sortedCulturesForMod.Count < sortedKingdomsForMod.Count) {
				sortedCulturesForMod.Add(string.Empty);
			}
		}
	}
}
