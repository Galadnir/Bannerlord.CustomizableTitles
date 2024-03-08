using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer;
using System.Collections.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using TaleWorlds.Engine.GauntletUI;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.LordTitlesVMs.TitlesForClansVMs {
	internal class IndividualClanKingdomPropertiesVM : DefaultListBaseVM {

		protected override string ID { get; } = "IndividualClanKingdomProperties";

		protected override string EntryHint { get; } = "Click to configure the properties of this clan, when he belongs to this kingdom. The name must match the kingdom name exactly";

		readonly string _clanKey;
		TitlesForTContainer<ClanProperties> Clan => _config.TitlesForLords.TitlesForClans.Clans[_clanKey];

		internal override string PathDescriptor => "Kingdom properties";

		[DataSourceProperty]
		public string CreateNewEntryName { get; } = "Add properties for a new kingdom for this clan";
		[DataSourceProperty]
		public HintViewModel AddNewEntryHint { get; } = new HintViewModel(new TextObject("Create title properties which are applied while this clan belongs to this kingdom. The name must match the kingdom name exactly. An entry with that name can't exist yet."));

		protected override IEnumerable<string> EntriesOriginKeys => Clan.Properties.Keys;

		public IndividualClanKingdomPropertiesVM(TitleConfiguration config, string clanKey, bool isEditable, SettingsLayerBaseVM parent, ConfigUIBaseVM baseVM) : base(config, isEditable, parent, baseVM, false) {
			_clanKey = clanKey;
			CreateStartingEntries();
		}
		public override void ExecuteCreateNewEntry() {
			BaseVM.Screen.OpenPopUp(new EditableTextPopUpVM("Enter name of kingdom",
			kingdomName => {
				if (kingdomName is null || Clan.Properties.ContainsKey(kingdomName)) {
					InformationManager.DisplayMessage(new InformationMessage("Did not create. An entry for that kingdom already exists."));
					return;
				}
				Clan.AddProperty(kingdomName, ClanProperties.CreateEmpty());
				AddToEntries(kingdomName);
			}, BaseVM.Screen
			));
		}
		protected override bool Rename(string oldKingdomKey, string newKingdomKey) {
			if (Clan.Properties.ContainsKey(newKingdomKey)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not rename. An entry with the entered name already exists."));
				return false;
			}
			ClanProperties properties = Clan.Properties[oldKingdomKey];
			Clan.DeleteProperty(oldKingdomKey);
			Clan.AddProperty(newKingdomKey, properties);
			return true;
		}
		protected override bool Copy(string kingdomKey, string newKingdomKey) {
			if (newKingdomKey is null || Clan.Properties.ContainsKey(newKingdomKey)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not copy. An entry with the entered name already exists."));
				return false;
			}
			Clan.AddProperty(newKingdomKey, new ClanProperties(Clan.Properties[kingdomKey]));
			return true;
		}
		protected override void Delete(string kingdomKey) {
			Clan.DeleteProperty(kingdomKey);
		}
		protected override void ExecuteSelectEntry(string kingdomKey, string originalKey) {
			ClanProperties properties = Clan.Properties[kingdomKey];
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditClanPropertiesVM(kingdomKey, properties, IsEditEnabled, this.IsOpenedEntryValid, BaseVM);
			var movie = layer.LoadMovie("CTEditClanProperties", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
		}

		private bool IsOpenedEntryValid() {
			return !(NextScreenOpenedWithOriginalKey is null) &&
				ModTitleConfigs.Contains(_config) &&
				ModTitleConfigs[ModTitleConfigs.IndexOf(_config)].TitlesForLords.TitlesForClans.Clans.ContainsKey(_clanKey) &&
				ModTitleConfigs[ModTitleConfigs.IndexOf(_config)].TitlesForLords.TitlesForClans.Clans[_clanKey].
				Properties.ContainsKey(NextScreenOpenedWithOriginalKey);
		}
	}
}
