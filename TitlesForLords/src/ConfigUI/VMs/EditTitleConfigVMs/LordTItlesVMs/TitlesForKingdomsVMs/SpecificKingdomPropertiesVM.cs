using Bannerlord.TitleOverhaul.src.ConfigUI.VMs.Common;
using Bannerlord.TitlesForLords.main.Core.Settings.TitleConfig.TitleConfigElements.TitlePropertiesContainer;
using Bannerlord.TitlesForLords.src.main.Core.Settings;
using Bannerlord.TitlesForLords.src.main.Core.Settings.TitleConfig;
using System.Collections.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.TitleOverhaul.src.ConfigUI.VMs.EditTitleConfigVMs.LordTitlesVMs.TitlesForKingdomsVMs {
	internal class SpecificKingdomPropertiesVM : DefaultListBaseVM {

		protected override string ID { get; } = "TitlesForKingdomsSpecificKingdomProperties";
		protected override string EntryHint { get; } = "Click to configure the properties of all lords belonging to this kingdom. The name must match the kingdom name exactly";

		TitlesForTContainer<KingdomProperties> SpecificKingdomsContainer => _config.TitlesForLords.TitlesForKingdoms.ExplicitKingdomProperties;

		internal override string PathDescriptor => "Kingdoms";

		[DataSourceProperty]
		public string CreateNewEntryName { get; } = "Add properties for a new kingdom";
		[DataSourceProperty]
		public HintViewModel AddNewEntryHint { get; } = new HintViewModel(new TextObject("Create title properties which are applied to all lords belonging to this kingdom. The name must match the kingdom name exactly. An entry with that name can't exist yet."));

		protected override IEnumerable<string> EntriesOriginKeys => SpecificKingdomsContainer.Properties.Keys;

		public SpecificKingdomPropertiesVM(TitleConfiguration config, bool isEditable, SettingsLayerBaseVM parent, ConfigUIBaseVM baseVM) : base(config, isEditable, parent, baseVM) {
		}
		public override void ExecuteCreateNewEntry() {
			BaseVM.Screen.OpenPopUp(new EditableTextPopUpVM("Enter name of kingdom",
			kingdomName => {
				if (kingdomName is null || SpecificKingdomsContainer.Properties.ContainsKey(kingdomName)) {
					InformationManager.DisplayMessage(new InformationMessage("Did not create. An entry for that kingdom already exists."));
					return;
				}
				SpecificKingdomsContainer.AddProperty(kingdomName, KingdomProperties.CreateEmpty());
				AddToEntries(kingdomName);
			}, BaseVM.Screen
			));
		}
		protected override bool Rename(string oldKingdomKey, string newKingdomKey) {
			if (SpecificKingdomsContainer.Properties.ContainsKey(newKingdomKey)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not rename. An entry with the entered name already exists."));
				return false;
			}
			KingdomProperties properties = SpecificKingdomsContainer.Properties[oldKingdomKey];
			SpecificKingdomsContainer.DeleteProperty(oldKingdomKey);
			SpecificKingdomsContainer.AddProperty(newKingdomKey, properties);
			return true;
		}
		protected override bool Copy(string kingdomKey, string newKingdomKey) {
			if (newKingdomKey is null || SpecificKingdomsContainer.Properties.ContainsKey(newKingdomKey)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not copy. An entry with the entered name already exists."));
				return false;
			}
			SpecificKingdomsContainer.AddProperty(newKingdomKey, new KingdomProperties(SpecificKingdomsContainer.Properties[kingdomKey]));
			return true;
		}
		protected override void Delete(string kingdomKey) {
			SpecificKingdomsContainer.DeleteProperty(kingdomKey);
		}
		protected override void ExecuteSelectEntry(string kingdomKey, string originalKey) {
			KingdomProperties properties = SpecificKingdomsContainer.Properties[kingdomKey];
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new KingdomPropertiesVM($"SpecificKingdoms {_config.Metadata.Uid} {originalKey}", kingdomKey, SpecificKingdomsContainer.Properties[kingdomKey], _config, IsEditEnabled,
				this.IsOpenedEntryValid,
				() => ModSettings.Instance.TitleConfigs[ModSettings.Instance.TitleConfigs.IndexOf(_config)].
				TitlesForLords.TitlesForKingdoms.ExplicitKingdomProperties.Properties[NextScreenOpenedWithOriginalKey],
				BaseVM);
			var movie = layer.LoadMovie("CTButtonList", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
		}

		private bool IsOpenedEntryValid() {
			return !(NextScreenOpenedWithOriginalKey is null) &&
				ModTitleConfigs.Contains(_config) &&
				ModTitleConfigs[ModTitleConfigs.IndexOf(_config)].TitlesForLords.TitlesForKingdoms.ExplicitKingdomProperties.Properties.ContainsKey(NextScreenOpenedWithOriginalKey);
		}
	}
}
