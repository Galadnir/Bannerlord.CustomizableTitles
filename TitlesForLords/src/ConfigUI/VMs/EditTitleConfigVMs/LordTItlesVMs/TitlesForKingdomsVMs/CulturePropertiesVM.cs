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
	internal class CulturePropertiesVM : DefaultListBaseVM {

		protected override string ID { get; } = "TitlesForKingdomsSpecificCultureProperties";
		protected override string EntryHint { get; } = "Click to configure the properties of all lords belonging to kingdoms of this culture. The name must match the culture name exactly";

		TitlesForTContainer<KingdomProperties> CulturesContainer => _config.TitlesForLords.TitlesForKingdoms.CultureProperties;

		internal override string PathDescriptor => "Cultures";

		[DataSourceProperty]
		public string CreateNewEntryName { get; } = "Add properties for a new culture";
		[DataSourceProperty]
		public HintViewModel AddNewEntryHint { get; } = new HintViewModel(new TextObject("Create title properties which are applied to all lords belonging to kingdoms of this culture. The name must match the culture name exactly. An entry with that name can't exist yet."));

		protected override IEnumerable<string> EntriesOriginKeys => CulturesContainer.Properties.Keys;

		public CulturePropertiesVM(TitleConfiguration config, bool isEditable, SettingsLayerBaseVM parent, ConfigUIBaseVM baseVM) : base(config, isEditable, parent, baseVM) {
		}

		public override void ExecuteCreateNewEntry() {
			BaseVM.Screen.OpenPopUp(new EditableTextPopUpVM("Enter name of culture",
			cultureName => {
				if (cultureName is null || CulturesContainer.Properties.ContainsKey(cultureName)) {
					InformationManager.DisplayMessage(new InformationMessage("Did not create. An entry for that culture already exists."));
					return;
				}
				CulturesContainer.AddProperty(cultureName, KingdomProperties.CreateEmpty());
				AddToEntries(cultureName);
			}, BaseVM.Screen
			));
		}
		protected override bool Rename(string oldCultureKey, string newCultureKey) {
			if (CulturesContainer.Properties.ContainsKey(newCultureKey)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not rename. An entry with the entered name already exists."));
				return false;
			}
			KingdomProperties properties = CulturesContainer.Properties[oldCultureKey];
			CulturesContainer.DeleteProperty(oldCultureKey);
			CulturesContainer.AddProperty(newCultureKey, properties);
			return true;
		}
		protected override bool Copy(string cultureKey, string newCultureKey) {
			if (newCultureKey is null || CulturesContainer.Properties.ContainsKey(newCultureKey)) {
				InformationManager.DisplayMessage(new InformationMessage("Did not copy. An entry with the entered name already exists."));
				return false;
			}
			CulturesContainer.AddProperty(newCultureKey, new KingdomProperties(CulturesContainer.Properties[cultureKey]));
			return true;
		}
		protected override void Delete(string cultureKey) {
			CulturesContainer.DeleteProperty(cultureKey);
		}
		protected override void ExecuteSelectEntry(string cultureKey, string originalKey) {
			KingdomProperties properties = CulturesContainer.Properties[cultureKey];
			var layer = new GauntletLayer(LayerPriority.Base, "GauntletLayer", true);
			var vm = new KingdomPropertiesVM($"SpecificCulture {_config.Metadata.Uid} {originalKey}", cultureKey, CulturesContainer.Properties[cultureKey], _config, IsEditEnabled,
				this.IsOpenedEntryValid,
				() => ModSettings.Instance.TitleConfigs[ModSettings.Instance.TitleConfigs.IndexOf(_config)].
				TitlesForLords.TitlesForKingdoms.CultureProperties.Properties[NextScreenOpenedWithOriginalKey],
				BaseVM);
			var movie = layer.LoadMovie("CTButtonList", vm);
			BaseVM.PushLayerAndMovie(layer, movie, vm);
		}

		private bool IsOpenedEntryValid() {
			return !(NextScreenOpenedWithOriginalKey is null) &&
				ModTitleConfigs.Contains(_config) &&
				ModTitleConfigs[ModTitleConfigs.IndexOf(_config)].TitlesForLords.TitlesForKingdoms.CultureProperties.Properties.ContainsKey(NextScreenOpenedWithOriginalKey);
		}
	}
}
