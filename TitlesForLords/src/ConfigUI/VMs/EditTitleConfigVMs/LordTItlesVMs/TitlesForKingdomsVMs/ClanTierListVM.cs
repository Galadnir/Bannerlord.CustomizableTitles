using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.LordTitlesVMs.TitlesForClansVMs;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.LordTitlesVMs.TitlesForKingdomsVMs {
	internal class ClanTierListVM : DefaultListBaseVM {

		readonly string _idEnding;
		readonly Func<KingdomProperties> _getPostResetProperties;

		protected override string ID => "EditClanTierList" + _idEnding;

		protected override string EntryHint { get; } = "Click to configure settings for this clans of this clan tier.";

		KingdomProperties ClanTiersContainer { get; }

		internal override string PathDescriptor => "clan tiers";

		[DataSourceProperty]
		public string CreateNewEntryName { get; } = "Add properties for a new clan tier";
		[DataSourceProperty]
		public HintViewModel AddNewEntryHint { get; } = new HintViewModel(new TextObject("Create title properties for a new clan tier. Properties for that tier cannot be defined yet."));

		protected override IEnumerable<string> EntriesOriginKeys => ClanTiersContainer.ClanTiers.Keys.Select(i => i.ToString());

		public ClanTierListVM(string idEnding, TitleConfiguration config, KingdomProperties clanTiersContainer, bool isEditable, Func<KingdomProperties> getPostResetProperties, SettingsLayerBaseVM parent, ConfigUIBaseVM baseVM) : base(config, isEditable, parent, baseVM, false, false) {
			_idEnding = idEnding;
			ClanTiersContainer = clanTiersContainer;
			_getPostResetProperties = getPostResetProperties;
			CreateKeyRenamesDict();
			CreateStartingEntries();
		}
		public override void ExecuteCreateNewEntry() {
			BaseVM.Screen.OpenPopUp(new EditableTextPopUpVM("Enter tier",
			clanTier => {
				bool tryParseResult = int.TryParse(clanTier, out int tier);
				if (clanTier is null || !tryParseResult || ClanTiersContainer.ClanTiers.ContainsKey(tier)) {
					InformationManager.DisplayMessage(new InformationMessage("Did not create. An entry for that tier already exists or the entered value was not a whole number."));
					return;
				}
				ClanTiersContainer.AddTierProperties(tier, ClanProperties.CreateEmpty());
				AddToEntries(clanTier);
			}, BaseVM.Screen
			));
		}

		protected override bool Rename(string oldTierKey, string newTierKey) {
			if (!int.TryParse(newTierKey, out int newTier) || ClanTiersContainer.ClanTiers.ContainsKey(newTier)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not rename. An entry with the entered name already exists or the entered value was not a whole number."));
				return false;
			}
			int oldTier = int.Parse(oldTierKey);
			ClanProperties properties = ClanTiersContainer.ClanTiers[oldTier];
			ClanTiersContainer.DeleteTierProperties(oldTier);
			ClanTiersContainer.AddTierProperties(newTier, properties);
			return true;
		}
		protected override bool Copy(string tierKey, string newTierKey) {
			if (newTierKey is null || !int.TryParse(newTierKey, out int newTier) || ClanTiersContainer.ClanTiers.ContainsKey(newTier)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not copy. An entry with the entered name already exists or the entered value was not a whole number."));
				return false;
			}
			int tier = int.Parse(tierKey);
			ClanProperties properties = ClanTiersContainer.ClanTiers[tier];
			ClanTiersContainer.AddTierProperties(newTier, new ClanProperties(properties));
			return true;
		}
		protected override void Delete(string tierKey) {
			ClanTiersContainer.DeleteTierProperties(int.Parse(tierKey));
		}
		protected override void ExecuteSelectEntry(string tierKey, string originalKey) {
			int tier = int.Parse(tierKey);
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditClanPropertiesVM($"tier {tierKey}", ClanTiersContainer.ClanTiers[tier], IsEditEnabled, this.IsChildValid, BaseVM);
			var movie = layer.LoadMovie("CTEditClanProperties", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
		}

		internal override bool IsChildValid() {
			if (!_parent.IsValid() || !int.TryParse(NextScreenOpenedWithOriginalKey, out int nextScreenOpenedForTier)) {
				return false;
			}
			KingdomProperties postResetProperties = _getPostResetProperties();
			return postResetProperties.ClanTiers.ContainsKey(nextScreenOpenedForTier);
		}
	}
}
