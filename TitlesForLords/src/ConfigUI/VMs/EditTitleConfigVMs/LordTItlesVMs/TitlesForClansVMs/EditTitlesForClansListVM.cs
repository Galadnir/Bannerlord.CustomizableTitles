using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using System.Collections.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.LordTitlesVMs.TitlesForClansVMs {
	internal class EditTitlesForClansListVM : DefaultListBaseVM {

		protected override string ID { get; } = "EditTitlesForClansList";
		protected override string EntryHint { get; } = "Click to configure settings for this clan. The name must match the clan name exactly. Overwrites default properties defined for all clans listed here.";

		TitlesForClans TitlesForClans => _config.TitlesForLords.TitlesForClans;

		internal override string PathDescriptor => "clans";

		[DataSourceProperty]
		public bool IsAdditionalButtonVisible { get; } = true;
		[DataSourceProperty]
		public string AdditionalButtonText { get; } = "edit default";
		[DataSourceProperty]
		public HintViewModel AdditionalButtonHint { get; } = new HintViewModel(new TextObject("A default that applies to all clans defined here."));

		[DataSourceProperty]
		public string CreateNewEntryName { get; } = "Add properties for a new clan";
		[DataSourceProperty]
		public HintViewModel AddNewEntryHint { get; } = new HintViewModel(new TextObject("Creare title properties for a new clan. The name of the entry must match the clan name exactly."));

		protected override IEnumerable<string> EntriesOriginKeys => TitlesForClans.Clans.Keys;

		public EditTitlesForClansListVM(TitleConfiguration config, bool isEditable, SettingsLayerBaseVM parent, ConfigUIBaseVM baseVM) : base(config, isEditable, parent, baseVM) {
		}

		public override void ExecuteAdditionalButtonPressed() {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditClanPropertiesVM("default", TitlesForClans.Default, IsEditEnabled, this.IsValid, BaseVM);
			var movie = layer.LoadMovie("CTEditClanProperties", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
			NextScreenOpenedViaAdditionalButton = true;
		}
		public override void ExecuteCreateNewEntry() {
			BaseVM.Screen.OpenPopUp(new EditableTextPopUpVM("Enter name of clan",
			clanName => {
				if (clanName is null || TitlesForClans.Clans.ContainsKey(clanName)) {
					InformationManager.DisplayMessage(new InformationMessage("Did not create. An entry for that name already exists."));
					return;
				}
				TitlesForClans.AddPropertiesForClan(clanName, TitlesForTContainer<ClanProperties>.CreateEmptyWithDefault(ClanProperties.CreateEmpty));
				AddToEntries(clanName);
			}, BaseVM.Screen
			));
		}
		protected override bool Rename(string oldClanKey, string newClanKey) {
			if (TitlesForClans.Clans.ContainsKey(newClanKey)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not rename. An entry with the entered name already exists."));
				return false;
			}
			TitlesForTContainer<ClanProperties> properties = TitlesForClans.Clans[oldClanKey];
			TitlesForClans.DeletePropertiesForClan(oldClanKey);
			TitlesForClans.AddPropertiesForClan(newClanKey, properties);
			return true;
		}
		protected override bool Copy(string clanKey, string newClanKey) {
			if (newClanKey is null || TitlesForClans.Clans.ContainsKey(newClanKey)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not copy. An entry with the entered name already exists."));
				return false;
			}
			TitlesForTContainer<ClanProperties> container = TitlesForClans.Clans[clanKey];
			TitlesForClans.AddPropertiesForClan(newClanKey, new TitlesForTContainer<ClanProperties>(container.Properties, container.Default));
			return true;
		}
		protected override void Delete(string clanKey) {
			TitlesForClans.DeletePropertiesForClan(clanKey);
		}
		protected override void ExecuteSelectEntry(string clanKey, string originalKey) {
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new EditIndividualClanPropertiesVM(_config, clanKey, clanKey, IsEditEnabled, this, BaseVM);
			var movie = layer.LoadMovie("CTEditIndividualClanView", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
		}

		internal override bool IsValid() {
			return _parent.IsValid();
		}

		internal override bool IsChildValid() {
			return !(NextScreenOpenedWithOriginalKey is null) &&
				ModTitleConfigs.Contains(_config) &&
				ModTitleConfigs[ModTitleConfigs.IndexOf(_config)].TitlesForLords.TitlesForClans.Clans.ContainsKey(NextScreenOpenedWithOriginalKey);
		}
	}
}
